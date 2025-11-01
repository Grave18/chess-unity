using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Initialization;
using Logic.MovesBuffer;
using Settings;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Ai
{
    public sealed class Stockfish : IDisposable
    {
        private readonly UciBuffer _uciBuffer;
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
        private string _out = string.Empty;
        private readonly StringBuilder _outStringBuilder = new();
        private readonly StringBuilder _errorStringBuilder = new();

        private readonly TaskCompletionSource<bool> _isAiLoaded = new();

        public Stockfish(UciBuffer uciBuffer, string fen)
        {
            _uciBuffer = uciBuffer;
            _fen = fen;
        }

        public void Dispose()
        {
            _process?.Dispose();
        }

        public void ShowProcessOutput()
        {
            Debug.Log("StdOut: " + _outStringBuilder.ToString());
            Debug.Log("StdErr: " + _errorStringBuilder.ToString());
        }

        public async Task ShowInternalBoardState()
        {
            try
            {
                string state = await GetInternalBoardState();
                LogState(state);
            }
            catch (Exception)
            {
                Debug.Log("ShowState aborted");
            }
        }

        private async Task<string> GetInternalBoardState()
        {
            CancellationToken exitCancellationToken = Application.exitCancellationToken;
            PostCommand("d");
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

            if (IsProcessDead())
            {
                return;
            }

            _process!.OutputDataReceived += ReadOutput;
            _process!.ErrorDataReceived += ReadError;
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        private void ReadError(object sender, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                _errorStringBuilder.AppendLine(outLine.Data);
            }
        }

        private void ReadOutput(object sender, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                _out = outLine.Data;
                _outStringBuilder.AppendLine(outLine.Data);
            }
        }

        private async Task SetupStockfish()
        {
            // Enable uci mode
            PostCommand("uci");
            await FindAnswer("uciok", Application.exitCancellationToken);

            // Set Threads
            int logicalProcessorsCount = SystemInfo.processorCount;
            Debug.Log($"Logical processors count = {logicalProcessorsCount}");
            PostCommand($"setoption name Threads value {logicalProcessorsCount/2}");
        }

        private async Task StartNewGame()
        {
            PostCommand("ucinewgame");
            PostCommand("isready");
            await FindAnswer("readyok", Application.exitCancellationToken);
        }

        private void DeclareReady()
        {
            Debug.Log("Stockfish loaded!");
            _isAiLoaded.SetResult(true);
        }

        public async Task<string> GetUci(PlayerSettings playerSettings, CancellationToken token)
        {
            _playerSettings = playerSettings;

            if (await IsAiFailedToLoad())
            {
                Debug.LogError("Ai failed to load");
                return null;
            }

            string bestMove = await GetBestMove(token);

            if (bestMove == null)
            {
                return null;
            }

            if (IsNoMoreMoves(bestMove))
            {
                LogNoMoreMoves(bestMove);
                return null;
            }

            string uci = ParseBestMoveToUci(bestMove);

            return uci;
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

        private async Task<string> GetBestMove(CancellationToken token)
        {
            try
            {
                string move = await GetBestMoveFromStockfish(token);
                return move;
            }
            catch (TaskCanceledException)
            {
                await HandleTaskCancel();
                return null;
            }
            catch (InvalidOperationException ex)
            {
                Debug.LogError("Invalid operation Exception: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return null;
            }
        }

        private async Task HandleTaskCancel()
        {
            Debug.Log("Move was canceled");
            try
            {
                PostCommand("stop");
                await FindAnswer("bestmove", Application.exitCancellationToken);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private async Task<string> GetBestMoveFromStockfish(CancellationToken token)
        {
            string name = $"{_playerSettings.Name}(Computer {_playerSettings.ComputerSkillLevel})";

            Debug.Log("\u21bb"); // ↻
            Debug.Log($"<color=green>{name} calculating move...</color>");

            // Set start position
            // Todo: get uci from command buffer _buffer.GetUciMoves()
            string positionCommand = $"position fen {_fen} {_uciBuffer.GetMovesUci()}";
            Debug.Log(positionCommand);
            PostCommand(positionCommand);

            // Set skill Level
            PostCommand($"setoption name Skill Level value {(int)_playerSettings.ComputerSkillLevel}");

            // Set command with time
            string goCommand = $"go movetime {_playerSettings.ComputerThinkTimeMs}";
            PostCommand(goCommand);

            // Get answer
            string output = await FindAnswer("bestmove", token);

            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            Debug.Log($"<color=green>{name} end calculate move</color>");
            Debug.Log($"<color=cyan>{output}</color>");

            return output;
        }

        private static string ParseBestMoveToUci(string move)
        {
            string result;

            if(move.Length >= 14)
            {
                result = move.Substring(9, 5);
                result = result.Trim();
            }
            else
            {
                result = move.Substring(9, 4);
            }

            return result;
        }

        /// Find line what contains find
        private async Task<string> FindAnswer(string find, CancellationToken token)
        {
            if (IsProcessDead())
            {
                Debug.Log("Can not read. Process is null or exited");
                return null;
            }

            while (!_out.Contains(find) && !token.IsCancellationRequested)
            {
                await Task.Delay(10, token);
            }

            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            string output = _out;
            _out = string.Empty;

            return output;
        }

        /// Read all output
        private async Task<string> ReadAllOutput(string find, CancellationToken token)
        {
            if (IsProcessDead())
            {
                Debug.Log("Can not read. Process is null or exited");
                return null;
            }

            StreamReader reader = _process.StandardOutput;

            string output = string.Empty;
            StringBuilder sb = new();
            while (!output.Contains(find) && !token.IsCancellationRequested)
            {
                await Task.Delay(25, token);
                output = await reader.ReadLineAsync();
                sb.AppendLine(output);
            }

            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            return sb.ToString();
        }

        private void PostCommand(string command)
        {
            if (IsProcessDead())
            {
                Debug.Log($"Cannot execute {command}. Process is null or exited");
                return;
            }

            StreamWriter writer = _process.StandardInput;
            writer.WriteLine(command);
        }

        private bool IsProcessDead()
        {
            return _process is { HasExited: true };
        }
    }
}