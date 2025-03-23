using System.Threading;
using System.Threading.Tasks;
using Ai;
using ChessBoard;
using ChessBoard.Pieces;
using GameAndScene.Initialization;
using Logic.MovesBuffer;
using Debug = UnityEngine.Debug;

namespace Logic.Players
{
    public class Computer : Player
    {
        private readonly Stockfish _stockfish;
        private readonly Buffer _commandBuffer;
        private readonly PlayerSettings _playerSettings;

        public Computer(Game game, Buffer commandBuffer, PlayerSettings playerSettings, Stockfish stockfish)
        :base(game)
        {
            _commandBuffer = commandBuffer;
            _playerSettings = playerSettings;
            _stockfish = stockfish;
        }

        /// Get calculations from stockfish process and make move
        public override async void AllowMakeMove()
        {
            // Todo: reimplement delay of Ai
            // Prevent momentary calculations after undo or redo
            // int delay = Game.MoveType is MoveTypeLegacy.Undo or MoveTypeLegacy.Redo ? 3000 : 100;
            int delay = 100;

            try
            {
                await Task.Delay(delay, _moveCts.Token);
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Awaiting was cancelled");
                return;
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }

            AiCalculationsResult aiCalculationsResult = await _stockfish.GetAiResult(_playerSettings, _moveCts.Token);
            if (aiCalculationsResult == null) return;

            MakeMove(aiCalculationsResult.MoveFrom, aiCalculationsResult.MoveTo);
        }

        private CancellationTokenSource _moveCts = new ();
        public override void DisallowMakeMove()
        {
            _moveCts?.Cancel();
            _moveCts = new CancellationTokenSource();
        }

        private void MakeMove(Square moveFromSquare, Square moveToSquare)
        {
            Piece movePiece = moveFromSquare.GetPiece();
            if (movePiece == null)
            {
                Debug.LogError("Piece not found");
                Debug.Log($"MoveFromSquare: {moveFromSquare?.name}");
                Debug.DebugBreak();
                return;
            }

            // Todo: make moves from computer
            // Move
            // if (movePiece.CanMoveTo(moveToSquare, out MoveInfo moveInfo))
            // {
            //     _ = CommandInvoker.MoveTo(moveFromSquare.GetPiece(), moveToSquare, moveInfo);
            // }
            // // Castling
            // else if (movePiece is King king && king.CanCastlingAt(moveToSquare, out CastlingInfo castlingInfo))
            // {
            //     _ = CommandInvoker.Castling(king, moveToSquare, castlingInfo.Rook, castlingInfo.RookToSquare, castlingInfo.MoveType);
            // }
            // // Eat
            // else if (movePiece.CanCaptureAt(moveToSquare, out CaptureInfo captureInfo))
            // {
            //     _ = CommandInvoker.EatAt(movePiece, moveToSquare, captureInfo);
            // }
            // else
            // {
            //     Debug.LogError("Move not found");
            //     Debug.Log($"MovePiece: {movePiece.name}, MoveFrom: {moveFromSquare.name}, MoveTo: {moveToSquare.name}");
            //     Debug.DebugBreak();
            // }
        }
    }
}