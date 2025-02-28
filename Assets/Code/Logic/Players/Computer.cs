using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using EditorCools;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Logic.Players
{
    public class Computer : Player
    {
        [Header("References")]
        [SerializeField] private Board board;

        [Header("Settings")]
        [SerializeField] private int thinkTimeMs = 3000;

        private Process _process;

        private void Start()
        {
            StartStockfish();
        }

        public override async void AllowMakeMove()
        {
            base.AllowMakeMove();

            string move = await GetBestMove();

            // Extract move form string
            string moveFrom = move.Substring(9, 2);
            string moveTo = move.Substring(11, 2);
            Debug.Log($"Best Move: {moveFrom}{moveTo}");

            Square moveFromSquare = board.GetSquare(moveFrom);
            Square moveToSquare = board.GetSquare(moveTo);

            Piece movePiece = moveFromSquare.GetPiece();
            if (movePiece == null)
            {
                Debug.Log("Piece not found");
                return;
            }

            // Move
            if (movePiece.CanMoveTo(moveToSquare))
            {
                CommandInvoker.MoveTo(moveFromSquare.GetPiece(), moveToSquare);
            }
            // Castling
            else if (movePiece is King king && king.CanCastlingAt(moveToSquare, out CastlingInfo castlingInfo))
            {
                CommandInvoker.Castling(king, moveToSquare, castlingInfo.Rook, castlingInfo.RookSquare, castlingInfo.NotationTurnType);
            }
            // Eat
            else if (movePiece.CanEatAt(moveToSquare, out CaptureInfo captureInfo))
            {
                CommandInvoker.EatAt(movePiece, moveToSquare, captureInfo);
            }
        }

        private async void StartStockfish()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "stockfish-windows-x86-64-avx2.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            _process = Process.Start(startInfo);
            await ReadAnswer("Stockfish");
            await PostCommand("ucinewgame");
        }

        [Button(space: 10f)]
        private async Task<string> GetBestMove()
        {
            Debug.Log("Computer calculate move...");

            string positionCommand = $"position startpos {CommandInvoker.GetUciMoves()}";
            Debug.Log(positionCommand);
            await PostCommand(positionCommand);

            string goCommand = $"go movetime {thinkTimeMs}";
            await PostCommand(goCommand);

            string output = await ReadAnswer("bestmove");

            Debug.Log("Computer end calculate move");

            return output;
        }

        private async Task<string> ReadAnswer(string find)
        {
            if (_process is { HasExited: true })
            {
                Debug.Log("Can not read. Process is null or exited");
                return null;
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