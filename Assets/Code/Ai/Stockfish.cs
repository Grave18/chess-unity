using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Info;
using GameAndScene.Initialization;
using Logic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Ai
{
    public sealed class Stockfish : IDisposable
    {
        private readonly Board _board;
        private readonly Game _game;
        private readonly CommandInvoker _commandInvoker;

        private PlayerSettings _playerSettings;

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

        public Stockfish(Board board, Game game, CommandInvoker commandInvoker)
        {
            _board = board;
            _game = game;
            _commandInvoker = commandInvoker;
        }

        public void Dispose()
        {
            _process?.Dispose();
        }

        public async Task Start()
        {
            StartStockfish();
            await SetupStockfish();
            await StartNewGame();
            DeclareReady();
        }

        public async Task<AiCalculationsResult> GetAiResult(PlayerSettings playerSettings)
        {
            _playerSettings = playerSettings;

            bool isLoaded = await _isStockfishLoaded.Task;
            if (!isLoaded) return null;

            string moveString = await GetMoveString();
            if (moveString == null) return null;

            ExtractSquareAddressesAndPromotionFrom(moveString, out string moveFrom, out string moveTo, out string promotion);

            return GetAICalculationsResult(moveFrom, moveTo, promotion);
        }

        private async Task<string> GetMoveString()
        {
            _game.StartThink();

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
                _game.EndThink();
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
            string message = $"Best Move: <color=cyan>{moveFrom}{moveTo}</color>";
            if (promotion != " ")
            {
                message += $". Promotion to: {promotion}";
            }

            Debug.Log(message);
        }

        private AiCalculationsResult GetAICalculationsResult(string moveFrom, string moveTo, string promotion)
        {
            Square moveFromSquare = _board.GetSquare(moveFrom);
            Square moveToSquare = _board.GetSquare(moveTo);

            PieceType promotionType = promotion switch
            {
                "q" => PieceType.Queen,
                "r" => PieceType.Rook,
                "b" => PieceType.Bishop,
                "n" => PieceType.Knight,
                _ => PieceType.None,
            };

            var aiCalculationsResult = new AiCalculationsResult(moveFromSquare, moveToSquare, promotionType);
            return aiCalculationsResult;
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

        private async Task<string> CalculateMove(CancellationToken token)
        {
            Debug.Log(string.Empty);
            Debug.Log($"<color=green>{_playerSettings.Name}(Computer) calculating move...</color>");

            // Set start position
            string positionCommand = $"position startpos {_commandInvoker.GetUciMoves()}";
            Debug.Log(positionCommand);
            await PostCommand(positionCommand, token);

            // Set skill Level
            Debug.Log($"Skill Level = {_playerSettings.ComputerSkillLevel}");
            await PostCommand($"setoption name Skill Level value {(int)_playerSettings.ComputerSkillLevel}", token);

            // Set command with time
            string goCommand = $"go movetime {_playerSettings.ComputerThinkTimeMs}";
            await PostCommand(goCommand, token);

            // Get answer
            string output = await ReadAnswer("bestmove", token);

            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            Debug.Log($"<color=green>{_playerSettings.Name}(Computer) end calculate move</color>");

            return output;
        }
    }
}