using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic.MovesBuffer;
using Logic.Players.GameStates;
using UnityEngine;
using Utils.Mathematics;

namespace Logic.Players.Moves
{
    public class Capture : Turn
    {
        private CaptureInfo _captureInfo;
        public Capture(ParsedUci parsedUci, MoveData moveData) : base(parsedUci, moveData)
        {
        }

        public override void Progress(float t)
        {
            Vector3 from = ParsedUci.FromSquare.transform.position;
            Vector3 to = ParsedUci.ToSquare.transform.position;
            Vector3 pos = Vector3.Lerp(from, to, Easing.InOutCubic(t));

            Piece movedPiece = ParsedUci.Piece.GetPiece();
            movedPiece.MoveTo(pos);
        }

        public override void End()
        {
            Piece movedPiece = ParsedUci.FromSquare.GetPiece();
            Piece beatenPiece = MoveData.BeatenPiece;
            Square beatenPieceSquare = beatenPiece.GetSquare();

            beatenPiece.RemoveFromBoard();
            BeatenPieces.Instance.Add(beatenPiece, beatenPieceSquare);
            movedPiece.SetNewSquare(ParsedUci.ToSquare);

            if (MoveData.IsFirstMove)
            {
                movedPiece.IsFirstMove = false;
            }
        }

        public override void EndUndo()
        {
            Piece movedPiece = ParsedUci.ToSquare.GetPiece();
            (Piece beatenPiece, Square beatenPieceSquare) = BeatenPieces.Instance.Remove();

            movedPiece.SetNewSquare(ParsedUci.FromSquare);
            beatenPiece.AddToBoard(beatenPieceSquare);

            if (MoveData.IsFirstMove)
            {
                movedPiece.IsFirstMove = true;
            }
        }
    }
}