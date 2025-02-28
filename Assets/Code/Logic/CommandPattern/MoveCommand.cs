﻿using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.Notation;

namespace Logic.CommandPattern
{
    public class MoveCommand : Command
    {
        private readonly Piece _piece;
        private readonly Square _moveToSquare;
        private readonly Game _game;
        private readonly SeriesList _seriesList;

        private Square _previousSquare;
        private bool _previousIsFirstMove;

        public MoveCommand(Piece piece, Square moveToSquare, Game game, SeriesList seriesList)
            : base(piece.GetSquare(), moveToSquare)
        {
            _piece = piece;
            _moveToSquare = moveToSquare;
            _game = game;
            _seriesList = seriesList;
        }

        public override async Task ExecuteAsync()
        {
            _game.StartTurn();

            // Backup
            _previousSquare = _piece.GetSquare();
            _previousIsFirstMove = _piece.IsFirstMove;
            _piece.IsFirstMove = false;

            // Move
            await _piece.MoveToAsync(_moveToSquare);

            // End turn and add to notation
            _game.EndTurn();
            _seriesList.AddTurn(_piece, _moveToSquare, _game.PreviousTurnColor, NotationTurnType.Move, _game.CheckType);
        }

        public override async Task UndoAsync()
        {
            if (_previousSquare == null)
            {
                return;
            }

            _game.StartTurn();

            // Move back
            await _piece.MoveToAsync(_previousSquare);
            _piece.IsFirstMove = _previousIsFirstMove;

            // Remove from notation and end turn
            _game.EndTurn();
            _seriesList.RemoveTurn(_game.CurrentTurnColor);
        }

        public override Piece GetPiece()
        {
            return _piece;
        }
    }
}
