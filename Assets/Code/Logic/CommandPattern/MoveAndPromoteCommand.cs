using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Builder;
using ChessBoard.Pieces;
using Logic.Notation;

namespace Logic.CommandPattern
{
    public class MoveAndPromoteCommand : Command
    {
        private Piece _piece;
        private Piece _backupPawn;
        private Piece _promotedPiece;
        private readonly Square _moveToSquare;
        private readonly Game _game;
        private readonly Board _board;
        private readonly SeriesList _seriesList;

        private PieceColor _previousTurnColor;
        private Square _previousSquare;

        public MoveAndPromoteCommand(Piece piece, Square moveToSquare, Game game, Board board,
            SeriesList seriesList)
        {
            _piece = piece;
            _moveToSquare = moveToSquare;
            _game = game;
            _board = board;
            _seriesList = seriesList;
        }

        public override async Task ExecuteAsync()
        {
            _game.StartTurn();

            // Backup
            _previousSquare = _piece.GetSquare();
            _previousTurnColor = _game.CurrentTurnColor;

            // Move selected piece
            await _piece.MoveToAsync(_moveToSquare);

            // Get promoted piece
            if(_promotedPiece == null)
            {
                (_promotedPiece, _) = await _board.GetPieceFromSelectorAsync(_piece.GetPieceColor(), _moveToSquare);
            }
            else
            {
               _promotedPiece.AddToBoard();
            }

            // Backup and hide pawn
            _backupPawn = _piece;
            _backupPawn.RemoveFromBoard();

            // Substitute pawn to promoted piece
            _piece = _promotedPiece;
            _game.AddPiece(_piece);

            // End turn and add to notation
            _game.EndTurn();

            // Is it Check?
            // NotationTurnType notationTurnType = _gameManager.CheckType switch
            // {
            //     CheckType.Check => NotationTurnType.Check,
            //     CheckType.DoubleCheck => NotationTurnType.DoubleCheck,
            //     CheckType.CheckMate => NotationTurnType.CheckMate,
            //     _ => NotationTurnType.Move
            // };

            _seriesList.AddTurn(_piece, _moveToSquare, _previousTurnColor, NotationTurnType.PromoteMove);
        }

        public override async Task UndoAsync()
        {
            if (_previousSquare == null)
            {
                return;
            }

            _game.StartTurn();

            // Remove promoted piece
            _promotedPiece.RemoveFromBoard();

            // Add pawn and go back
            _backupPawn.AddToBoard();
            _piece = _backupPawn;
            await _piece.MoveToAsync(_previousSquare);

            // Remove from notation and end turn
            _seriesList.RemoveTurn(_previousTurnColor);
            _game.EndTurn();
        }

        public override Piece GetPiece()
        {
            return _piece;
        }
    }
}