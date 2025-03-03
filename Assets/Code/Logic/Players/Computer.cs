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
        [Header("References")]
        [SerializeField] private Board board;

        [Header("Settings")]
        [SerializeField] private int thinkTimeMs = 3000;

        private CancellationTokenSource _cancellationTokenSource;

        private Process _process;

        public override async void AllowMakeMove()
        {
            base.AllowMakeMove();

            _cancellationTokenSource = new CancellationTokenSource();

            string move;
            try
            {
                move = await GetBestMove(_cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Move was canceled");
                return;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return;
            }

            // Extract move form string
            string moveFrom = move.Substring(9, 2);
            string moveTo = move.Substring(11, 2);
            Debug.Log($"Best Move: {moveFrom}{moveTo}");

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

        public override void DisallowMakeMove()
        {
            base.DisallowMakeMove();
            _cancellationTokenSource?.Cancel();
        }

        private void Start()
        {
            StartStockfish();
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
            await PostCommand("ucinewgame");
        }

        private async Task<string> GetBestMove(CancellationToken token)
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

            string output = null;
            while (output == null || !output.Contains(find))
            {
                if (token.IsCancellationRequested)
                {
                    await PostCommand("stop");
                }
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
            if (_process is  { HasExited: true })
            {
                return;
            }

            _ = PostCommand("quit");
        }
    }
}