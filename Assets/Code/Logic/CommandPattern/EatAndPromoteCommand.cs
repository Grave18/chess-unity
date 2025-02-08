using Board;
using Board.Builder;
using Board.Pieces;
using Logic.Notation;
using Ui;
using UnityEngine;

namespace Logic.CommandPattern
{
    public class EatAndPromoteCommand : Command
    {
        private Piece _piece;
        private readonly Square _square;
        private readonly GameManager _gameManager;
        private readonly BoardBuilder _boardBuilder;
        private readonly SeriesList _seriesList;

        private PieceColor _previousTurnColor;
        private Square _previousSquare;
        private Piece _beatenPiece;

        public EatAndPromoteCommand(Piece piece, Square square, GameManager gameManager, BoardBuilder boardBuilder,
            SeriesList seriesList)
        {
            _piece = piece;
            _square = square;
            _gameManager = gameManager;
            _boardBuilder = boardBuilder;
            _seriesList = seriesList;
        }

        public override void Execute()
        {
            // _seriesList.AddTurn(_piece, _square, _gameManager.CurrentTurnColor, NotationTurnType.Capture);

            _previousSquare = _piece.GetSquare();
            _previousTurnColor = _gameManager.CurrentTurnColor;

            _piece.IsFirstMove = false;

            _beatenPiece = _piece.EatAt(_square);
            _piece.MoveTo(_square, OnEndMoveTo);
            return;

            void OnEndMoveTo()
            {
                _gameManager.RemovePiece(_piece);
                Object.Destroy(_piece.gameObject);

                // _piece = _boardBuilder.GetPieceFromSelector(_piece.GetPieceColor(), _square);
                _gameManager.AddPiece(_piece);

                _gameManager.EndTurn();

                // Is it Check?
                // NotationTurnType notationTurnType = _gameManager.CheckType switch
                // {
                //     CheckType.Check => NotationTurnType.Check,
                //     CheckType.DoubleCheck => NotationTurnType.DoubleCheck,
                //     CheckType.CheckMate => NotationTurnType.CheckMate,
                //     _ => NotationTurnType.Move
                // };

                _seriesList.AddTurn(_piece, _square, _previousTurnColor, NotationTurnType.PromoteCapture);
            }
        }

        public override void Undo()
        {
            if (_previousSquare == null || _beatenPiece == null)
            {
                return;
            }

            // Remove promoted piece
            _gameManager.RemovePiece(_piece);
            Object.Destroy(_piece.gameObject);

            // Add pawn and go back
            _piece = _boardBuilder.GetPiece(PieceType.Pawn, _piece.GetPieceColor(), _square);
            _gameManager.AddPiece(_piece);
            _piece.MoveTo(_previousSquare);

            // Add beaten piece
            _beatenPiece.RemoveFromBeaten(_square);

            // Remove from notation and end turn
            _seriesList.RemoveTurn(_previousTurnColor);
            _gameManager.EndTurn();
        }
    }
}
