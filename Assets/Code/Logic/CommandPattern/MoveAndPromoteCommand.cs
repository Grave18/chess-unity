using System.Threading.Tasks;
using AlgebraicNotation;
using ChessBoard;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public class MoveAndPromoteCommand : Command
    {
        private Piece _backupPawn;
        private readonly Game _game;
        private readonly Board _board;
        private readonly SeriesList _seriesList;

        private PieceColor _previousTurnColor;
        private Square _previousSquare;

        public MoveAndPromoteCommand(Piece piece, Square moveToSquare, Game game, Board board,
            SeriesList seriesList) : base(piece, piece.GetSquare(), moveToSquare)
        {
            _game = game;
            _board = board;
            _seriesList = seriesList;
        }

        public override async Task Execute()
        {
            // Backup
            _previousSquare = Piece.GetSquare();
            _previousTurnColor = _game.CurrentTurnColor;

            // Move selected piece
            await Piece.MoveToAsync(MoveToSquare);

            // Get promoted piece
            if(PromotedPiece == null)
            {
                (PromotedPiece, _) = await _board.GetPieceFromSelectorAsync(Piece.GetPieceColor(), MoveToSquare);
            }
            else
            {
               _board.AddPiece(PromotedPiece);
               PromotedPiece.gameObject.SetActive(true);
            }

            // Backup and hide pawn
            _backupPawn = Piece;
            _board.RemovePiece(_backupPawn);
            _backupPawn.gameObject.SetActive(false);

            // Substitute pawn to promoted piece
            Piece = PromotedPiece;
            _board.AddPiece(Piece);

            // _seriesList.AddTurn(_backupPawn, _moveToSquare, _previousTurnColor, NotationTurnType.Move, _game.CheckType, _promotedPiece);
        }

        public override async Task Undo()
        {
            if (_previousSquare == null)
            {
                return;
            }

            // Remove promoted piece
            _board.RemovePiece(PromotedPiece);
            PromotedPiece.gameObject.SetActive(false);

            // Add pawn and go back
            _board.AddPiece(_backupPawn);
            _backupPawn.gameObject.SetActive(true);
            Piece = _backupPawn;
            await Piece.MoveToAsync(_previousSquare);

            // _seriesList.RemoveTurn(_previousTurnColor);
        }
    }
}