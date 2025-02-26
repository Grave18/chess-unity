using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EditorCools;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Ai
{
    public class StartProcess : MonoBehaviour
    {
        private Process _process;

        private void Start()
        {
            StartStockfish();
        }

        private void StartStockfish()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "stockfish-windows-x86-64-avx2.exe",
                // Arguments = "uci",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            _process = Process.Start(startInfo);

            if (_process != null)
            {
                StreamReader reader = _process.StandardOutput;

                Debug.Log(reader.ReadLine());
            }
            else
            {
                Debug.Log("Process is null");
            }
        }

        [Button(space: 10f)]
        private async void RunUciCommand()
        {
            await PostCommand("uci");
            await ReadAnswer();
        }

        [Button(space: 10f)]
        private async void RunCalculateMoveCommand()
        {
            const int timeMs = 3000;

            Debug.Log("Start calculate move...");
            await PostCommand("ucinewgame");
            await PostCommand("position startpos");
            await PostCommand($"go movetime {timeMs}");
            string output = await ReadAnswer("bestmove");

            Debug.Log(output);
        }

        [Button(space: 10f)]
        private async Task<string> ReadAnswer()
        {
            if (_process is not { HasExited: false })
            {
                Debug.Log("Can not read. Process is null or exited");
                return string.Empty;
            }

            StreamReader reader = _process.StandardOutput;

            var buffer = new char[16000];
            int numCharsRead = await reader.ReadAsync(buffer, 0, buffer.Length);
            var sb = new StringBuilder();
            for (int i = 0; i < numCharsRead; i++)
            {
                sb.Append(buffer[i]);
            }

            return sb.ToString();
        }

        private async Task<string> ReadAnswer(string find)
        {
            if (_process is { HasExited: true })
            {
                Debug.Log("Can not read. Process is null or exited");
                return string.Empty;
            }

            StreamReader reader = _process.StandardOutput;

            string output = null;
            while (output == null || !output.Contains(find))
            {
                await Task.Delay(100);
                output = await reader.ReadLineAsync();
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

        private void OnDestroy()
        {
            if (_process is  { HasExited: true })
            {
                return;
            }

            _ = PostCommand("quit");
        }
    }
}