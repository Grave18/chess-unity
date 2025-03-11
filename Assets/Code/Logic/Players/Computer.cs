using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Logic.Players
{
    public class Computer : Player
    {
        public enum ComputerSkillLevel
        {
            Ape = 0,
            Low = 5,
            Medium = 10,
            High = 20,
        }

        private readonly Board _board;
        private readonly ComputerSkillLevel _skillLevel;
        private readonly int _thinkTimeMs;

        // Stockfish
        private Process _process;
        private readonly ProcessStartInfo _startInfo = new()
        {
            FileName = "stockfish-windows-x86-64-avx2.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        private readonly TaskCompletionSource<bool> _isStockfishLoaded = new();

        private PieceType _promotion = PieceType.None;

        public Computer(Game game, CommandInvoker commandInvoker, Board board, ComputerSkillLevel computerSkillLevel,
            int thinkTimeMs)
        :base(game, commandInvoker)
        {
            _board = board;
            _skillLevel = computerSkillLevel;
            _thinkTimeMs = thinkTimeMs;
        }

        public override async void Start()
        {
            StartStockfish();
            await SetupStockfish();
            await StartNewGame();
            DeclareReady();
        }

        public override Task<PieceType> RequestPromotedPiece()
        {
            return Task.FromResult(_promotion);
        }

        /// <summary>
        /// Get calculations from stockfish process and make move
        /// </summary>
        public override async void AllowMakeMove()
        {
            bool isLoaded = await _isStockfishLoaded.Task;
            if (!isLoaded) return;

            string moveString = await GetMoveString();
            if (moveString == null) return;

            ExtractSquareAddressesAndPromotionFrom(moveString,
                out string moveFrom, out string moveTo, out string promotion);

            SetPromotionPiece(promotion);

            GetSquaresAndPieceAndMakeMove(moveFrom, moveTo);
        }

        private async Task<string> GetMoveString()
        {
            Game.StartThink();

            string move = null;
            try
            {
                move = await CalculateMove(Application.exitCancellationToken);
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Move was canceled");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            finally
            {
                Game.EndThink();
            }

            return move;
        }

        private static void ExtractSquareAddressesAndPromotionFrom(string move,
            out string moveFrom, out string moveTo, out string promotion)
        {
            // Extract move form string
            moveFrom = move.Substring(9, 2);
            moveTo = move.Substring(11, 2);
            promotion = move.Substring(13, 1);

            // Log
            string message = $"Best Move: {moveFrom}{moveTo}";
            if (promotion != " ")
            {
                message += $". Promotion to: {promotion}";
            }

            Debug.Log(message);
        }

        private void SetPromotionPiece(string promotion)
        {
            _promotion = promotion switch
            {
                "q" => PieceType.Queen,
                "r" => PieceType.Rook,
                "b" => PieceType.Bishop,
                "n" => PieceType.Knight,
                _ => PieceType.None,
            };
        }

        private void GetSquaresAndPieceAndMakeMove(string moveFrom, string moveTo)
        {
            Square moveFromSquare = _board.GetSquare(moveFrom);
            Square moveToSquare = _board.GetSquare(moveTo);

            Piece movePiece = moveFromSquare.GetPiece();
            if (movePiece == null)
            {
                Debug.LogError("Piece not found");
                Debug.Log($"MoveFromSquare: {moveFromSquare.name}");
                Debug.DebugBreak();
                return;
            }

            // Move
            if (movePiece.CanMoveTo(moveToSquare, out MoveInfo moveInfo))
            {
                _ = CommandInvoker.MoveTo(moveFromSquare.GetPiece(), moveToSquare, moveInfo);
            }
            // Castling
            else if (movePiece is King king && king.CanCastlingAt(moveToSquare, out CastlingInfo castlingInfo))
            {
                _ = CommandInvoker.Castling(king, moveToSquare, castlingInfo.Rook, castlingInfo.RookSquare, castlingInfo.NotationTurnType);
            }
            // Eat
            else if (movePiece.CanEatAt(moveToSquare, out CaptureInfo captureInfo))
            {
                _ = CommandInvoker.EatAt(movePiece, moveToSquare, captureInfo);
            }
            else
            {
                Debug.LogError("Move not found");
                Debug.Log($"MovePiece: {movePiece.name}, MoveFrom: {moveFromSquare.name}, MoveTo: {moveToSquare.name}");
                Debug.DebugBreak();
            }
        }

        private void StartStockfish()
        {
            _process = Process.Start(_startInfo);
        }

        private async Task SetupStockfish()
        {
            // Enable uci mode
            await PostCommand("uci");
            await ReadAnswer("uciok", Application.exitCancellationToken);

            // Set Threads
            int logicalProcessorsCount = SystemInfo.processorCount;
            Debug.Log($"Logical processors count = {logicalProcessorsCount}");
            await PostCommand($"setoption name Threads value {logicalProcessorsCount/2}");

            // Set Skill Level
            Debug.Log($"Skill Level = {_skillLevel}");
            await PostCommand($"setoption name Skill Level value {(int)_skillLevel}");
        }

        private async Task StartNewGame()
        {
            await PostCommand("ucinewgame");
            await PostCommand("isready");
            await ReadAnswer("readyok", Application.exitCancellationToken);
        }

        private void DeclareReady()
        {
            Debug.Log("Computer is ready");
            _isStockfishLoaded.SetResult(true);
        }

        private async Task<string> CalculateMove(CancellationToken token)
        {
            Debug.Log("Computer calculate move...");

            string positionCommand = $"position startpos {CommandInvoker.GetUciMoves()}";
            Debug.Log(positionCommand);
            await PostCommand(positionCommand, token);

            string goCommand = $"go movetime {_thinkTimeMs}";
            await PostCommand(goCommand, token);

            string output = await ReadAnswer("bestmove", token);

            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            Debug.Log("Computer end calculate move");

            return output;
        }

        private async Task<string> ReadAnswer(string find, CancellationToken token)
        {
            if (_process is { HasExited: true })
            {
                Debug.Log("Can not read. Process is null or exited");
                return null;
            }

            StreamReader reader = _process.StandardOutput;

            string output = string.Empty;
            while (!token.IsCancellationRequested && !output.Contains(find))
            {
                await Task.Delay(25, token);
                output = await reader.ReadLineAsync();
            }

            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            return output;
        }

        private async Task PostCommand(string command)
        {
            if (_process is { HasExited: true })
            {
                Debug.Log($"Cannot execute {command}. Process is null or exited");
                return;
            }

            StreamWriter writer = _process.StandardInput;
            await writer.WriteLineAsync(command);
        }

        private async Task PostCommand(string command, CancellationToken token)
        {
            await PostCommand(command);

            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
        }

        private void OnDestroy()
        {
            _ = PostCommand("quit");
        }
    }
}