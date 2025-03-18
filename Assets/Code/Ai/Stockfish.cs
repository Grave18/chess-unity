using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        private readonly string _fen;

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
        private readonly TaskCompletionSource<bool> _isAiLoaded = new();

        public Stockfish(Board board, Game game, CommandInvoker commandInvoker, string fen)
        {
            _board = board;
            _game = game;
            _commandInvoker = commandInvoker;
            _fen = fen;
        }

        public void Dispose()
        {
            _process?.Dispose();
        }

        public async Task ShowState()
        {
            try
            {
                string state = await GetState();
                LogState(state);
            }
            catch (Exception)
            {
                Debug.Log("ShowState aborted");
            }
        }

        private async Task<string> GetState()
        {
            CancellationToken exitCancellationToken = Application.exitCancellationToken;
            await PostCommand("d", exitCancellationToken);
            string state = await ReadAllOutput("Checkers:", exitCancellationToken);
            return state;
        }

        private static void LogState(string state)
        {
            state = "<color=magenta>Stockfish state:</color>\n" + state;
            Debug.Log(state);
        }

        public async Task Start()
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
            await FindAnswer("uciok", Application.exitCancellationToken);

            // Set Threads
            int logicalProcessorsCount = SystemInfo.processorCount;
            Debug.Log($"Logical processors count = {logicalProcessorsCount}");
            await PostCommand($"setoption name Threads value {logicalProcessorsCount/2}");
        }

        private async Task StartNewGame()
        {
            await PostCommand("ucinewgame");
            await PostCommand("isready");
            await FindAnswer("readyok", Application.exitCancellationToken);
        }

        private void DeclareReady()
        {
            Debug.Log("Computer is ready");
            _isAiLoaded.SetResult(true);
        }

        public async Task<AiCalculationsResult> GetAiResult(PlayerSettings playerSettings, CancellationToken token)
        {
            _playerSettings = playerSettings;

            if (await IsAiFailedToLoad())
            {
                Debug.LogError("Ai failed to load");
                return null;
            }

            string moveString = await GetMoveString(token);
            if (moveString == null) return null;

            if (IsNoMoreMoves(moveString))
            {
                LogNoMoreMoves(moveString);
                return null;
            }

            ExtractSquareAddressesAndPromotionFrom(moveString, out string moveFrom, out string moveTo, out string promotion);

            return GetAICalculationsResult(moveFrom, moveTo, promotion);
        }

        private async Task<bool> IsAiFailedToLoad()
        {
            return !await _isAiLoaded.Task;
        }

        private static bool IsNoMoreMoves(string moveString)
        {
            return moveString.Contains("(none)");
        }

        private static void LogNoMoreMoves(string moveString)
        {
            string message = "<color=cyan>No more moves</color>";
            message += $"\n<color=gray>{moveString}</color>";
            Debug.Log(message);
        }

        private async Task<string> GetMoveString(CancellationToken token)
        {
            _game.StartThink();

            string move = null;
            try
            {
                move = await CalculateMove(token);
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Move was canceled");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
            finally
            {
                _game.EndThink();
            }

            return move;
        }

        private async Task<string> CalculateMove(CancellationToken token)
        {
            string name = $"{_playerSettings.Name}(Computer {_playerSettings.ComputerSkillLevel})";

            Debug.Log("\u21bb"); // ↻
            Debug.Log($"<color=green>{name} calculating move...</color>");

            // Set start position
            string positionCommand = $"position fen {_fen} {_commandInvoker.GetUciMoves()}";
            Debug.Log(positionCommand);
            await PostCommand(positionCommand, token);

            // Set skill Level
            await PostCommand($"setoption name Skill Level value {(int)_playerSettings.ComputerSkillLevel}", token);

            // Set command with time
            string goCommand = $"go movetime {_playerSettings.ComputerThinkTimeMs}";
            await PostCommand(goCommand, token);

            // Get answer
            string output = await FindAnswer("bestmove", token);

            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            Debug.Log($"<color=green>{name} end calculate move</color>");

            return output;
        }

        private static void ExtractSquareAddressesAndPromotionFrom(string move,
            out string moveFrom, out string moveTo, out string promotion)
        {
            // Extract move form string
            moveFrom = move.Substring(9, 2);
            moveTo = move.Substring(11, 2);

            promotion = " ";
            if(move.Length >= 14)
            {
                promotion = move.Substring(13, 1);
            }

            LogMove(move, moveFrom, moveTo, promotion);
        }

        private static void LogMove(string move, string moveFrom, string moveTo, string promotion)
        {
            // Log
            string message = $"Best Move: <color=cyan>{moveFrom}{moveTo}</color>";
            message += promotion == " " ? string.Empty : $". Promotion to: {promotion}";
            message += $"\n<color=gray>{move}</color>";

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

        /// Find line what contains find
        private async Task<string> FindAnswer(string find, CancellationToken token)
        {
            if (_process is { HasExited: true })
            {
                Debug.Log("Can not read. Process is null or exited");
                return null;
            }

            StreamReader reader = _process.StandardOutput;

            string output = string.Empty;
            while (!output.Contains(find))
            {
                await Task.Delay(25, token);
                if (token.IsCancellationRequested) break;
                output = await reader.ReadLineAsync();
            }

            if (token.IsCancellationRequested)
            {
                await PostCommand("stop");
                while (!output.Contains(find))
                {
                    await Task.Delay(10, token);
                    output = await reader.ReadLineAsync();
                }

                throw new TaskCanceledException();
            }

            return output;
        }

        /// Read all output
        private async Task<string> ReadAllOutput(string find, CancellationToken token)
        {
            if (_process is { HasExited: true })
            {
                Debug.Log("Can not read. Process is null or exited");
                return null;
            }

            StreamReader reader = _process.StandardOutput;

            string output = string.Empty;
            StringBuilder sb = new();
            while (!token.IsCancellationRequested && !output.Contains(find))
            {
                await Task.Delay(25, token);
                output = await reader.ReadLineAsync();
                sb.AppendLine(output);
            }

            if (token.IsCancellationRequested)
            {
                await PostCommand("stop");
                while (!output.Contains(find))
                {
                    await Task.Delay(25, token);
                    output = await reader.ReadLineAsync();
                    sb.AppendLine(output);
                }

                throw new TaskCanceledException();
            }

            return sb.ToString();
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
    }
}