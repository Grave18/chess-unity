using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.MovesBuffer;

namespace Logic.CommandPattern
{
    public class EatAndPromoteCommand : Command
    {
        private Piece _backupPawn;
        private readonly Piece _beatenPiece;
        private readonly Square _beatenPieceSquare;
        private readonly Board _board;
        private Square _previousSquare;

        public EatAndPromoteCommand(Piece piece, Piece beatenPiece, Square moveToSquare,
            Board board): base(piece, piece.GetSquare(), moveToSquare, MoveType.Capture)
        {
            _beatenPiece = beatenPiece;
            _beatenPieceSquare = _beatenPiece.GetSquare();
            _board = board;
        }

        public override async Task Execute()
        {
            // Backup
            _previousSquare = Piece.GetSquare();

            // Remove beaten piece
            _beatenPiece.RemoveFromBoard();

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
        }

        public override async Task Undo()
        {
            if (_previousSquare == null || _beatenPiece == null)
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

            // Add beaten piece
            _beatenPiece.AddToBoard(_beatenPieceSquare);
        }
    }
}