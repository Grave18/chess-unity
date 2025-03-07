using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.Notation;

namespace Logic.CommandPattern
{
    public class EatAndPromoteCommand : Command
    {
        private Piece _backupPawn;
        private Piece _promotedPiece;
        private readonly Piece _beatenPiece;
        private readonly Square _beatenPieceSquare;
        private readonly Game _game;
        private readonly Board _board;
        private readonly SeriesList _seriesList;

        private PieceColor _previousTurnColor;
        private Square _previousSquare;

        public EatAndPromoteCommand(Piece piece, Piece beatenPiece, Square moveToSquare, Game game,
            Board board,
            SeriesList seriesList): base(piece, piece.GetSquare(), moveToSquare)
        {
            _beatenPiece = beatenPiece;
            _beatenPieceSquare = _beatenPiece.GetSquare();
            _game = game;
            _board = board;
            _seriesList = seriesList;
        }

        public override async Task Execute()
        {
            // Backup
            _previousSquare = Piece.GetSquare();
            _previousTurnColor = _game.CurrentTurnColor;

            // Remove beaten piece
            _beatenPiece.MoveToBeaten();

            // Move selected piece
            await Piece.MoveToAsync(MoveToSquare);

            // Get promoted piece
            if(_promotedPiece == null)
            {
                (_promotedPiece, _) = await _board.GetPieceFromSelectorAsync(Piece.GetPieceColor(), MoveToSquare);
            }
            else
            {
                _board.AddPiece(_promotedPiece);
                _promotedPiece.gameObject.SetActive(true);
            }

            // Backup and hide pawn
            _backupPawn = Piece;
            _board.RemovePiece(_backupPawn);
            _backupPawn.gameObject.SetActive(false);

            // Substitute pawn to promoted piece
            Piece = _promotedPiece;
            _board.AddPiece(Piece);

            // _seriesList.AddTurn(_backupPawn, _moveToSquare, _previousTurnColor, NotationTurnType.Capture, _game.CheckType, _promotedPiece);
        }

        public override async Task Undo()
        {
            if (_previousSquare == null || _beatenPiece == null)
            {
                return;
            }

            // Remove promoted piece
            _board.RemovePiece(_promotedPiece);
            _promotedPiece.gameObject.SetActive(false);

            // Add pawn and go back
            _board.AddPiece(_backupPawn);
            _backupPawn.gameObject.SetActive(true);
            Piece = _backupPawn;
            await Piece.MoveToAsync(_previousSquare);

            // Add beaten piece
            _beatenPiece.RemoveFromBeaten(_beatenPieceSquare);

            // _seriesList.RemoveTurn(_game.CurrentTurnColor);
        }
    }
}