using System;
using System.Threading;
using System.Threading.Tasks;
using Ai;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using GameAndScene.Initialization;
using Debug = UnityEngine.Debug;

namespace Logic.Players
{
    public class Computer : Player
    {
        private readonly Stockfish _stockfish;
        private readonly PlayerSettings _playerSettings;

        private PieceType _promotion = PieceType.None;

        public Computer(Game game, CommandInvoker commandInvoker, PlayerSettings playerSettings, Stockfish stockfish)
        :base(game, commandInvoker)
        {
            _playerSettings = playerSettings;
            _stockfish = stockfish;
        }

        public override Task<PieceType> RequestPromotedPiece()
        {
            return Task.FromResult(_promotion);
        }

        /// Get calculations from stockfish process and make move
        public override async void AllowMakeMove()
        {
            // Prevent momentary calculations after undo or redo
            int delay = Game.MoveType is MoveType.Undo or MoveType.Redo ? 3000 : 100;

            try
            {
                await Task.Delay(delay, _moveCts.Token);
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Awaiting was cancelled");
                return;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            AiCalculationsResult aiCalculationsResult = await _stockfish.GetAiResult(_playerSettings, _moveCts.Token);
            if (aiCalculationsResult == null) return;

            SetPromotionPiece(aiCalculationsResult.Promotion);
            MakeMove(aiCalculationsResult.MoveFrom, aiCalculationsResult.MoveTo);
        }

        private CancellationTokenSource _moveCts = new ();
        public override void DisallowMakeMove()
        {
            _moveCts?.Cancel();
            _moveCts = new CancellationTokenSource();
        }

        private void SetPromotionPiece(PieceType promotion)
        {
            _promotion = promotion;
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
    }
}