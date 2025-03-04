using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Logic.Players
{
    public class Computer : Player
    {
        private enum ComputerSkillLevel
        {
            Ape = 0,
            Low = 5,
            Medium = 10,
            High = 20,
        }

        [Header("References")]
        [SerializeField] private Board board;

        [Header("Settings")]
        [SerializeField] private ComputerSkillLevel skillLevel = ComputerSkillLevel.Medium;
        [SerializeField] private int thinkTimeMs = 3000;

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

        /// <summary>
        /// Get calculations from stockfish process and make move
        /// </summary>
        public override async void AllowMakeMove()
        {
            base.AllowMakeMove();

            bool isLoaded = await _isStockfishLoaded.Task;

            if (!isLoaded)
            {
                return;
            }

            string moveString = await GetMoveString();

            if (moveString == null)
            {
                return;
            }

            var(moveFromString, moveToString) = ExtractMoveSquaresAddressesFromString(moveString);

            GetSquaresAndPieceAndMakeMove(moveFromString, moveToString);
        }

        private void GetSquaresAndPieceAndMakeMove(string moveFrom, string moveTo)
        {
            Square moveFromSquare = board.GetSquare(moveFrom);
            Square moveToSquare = board.GetSquare(moveTo);

            Piece movePiece = moveFromSquare.GetPiece();
            if (movePiece == null)
            {
                Debug.LogError("Piece not found");
                return;
            }

            // Move
            if (movePiece.CanMoveTo(moveToSquare))
            {
                _ = CommandInvoker.MoveTo(moveFromSquare.GetPiece(), moveToSquare);
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
        }

        private static (string, string) ExtractMoveSquaresAddressesFromString(string move)
        {
            // Extract move form string
            string moveFrom = move.Substring(9, 2);
            string moveTo = move.Substring(11, 2);
            Debug.Log($"Best Move: {moveFrom}{moveTo}");
            return (moveFrom, moveTo);
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

        private async void Start()
        {
            StartStockfish();
            await SetupStockfish();
            await StartNewGame();
            DeclareReady();
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
            Debug.Log($"Skill Level = {skillLevel}");
            await PostCommand($"setoption name Skill Level value {(int)skillLevel}");
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

            string goCommand = $"go movetime {thinkTimeMs}";
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
                await Task.Delay(100, token);
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