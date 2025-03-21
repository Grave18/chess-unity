using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic.MovesBuffer;
using Logic.Players.GameStates;
using UnityEngine;
using Utils.Mathematics;

namespace Logic.Players.Moves
{
    public abstract class Turn
    {
        protected ParsedUci ParsedUci { get; }
        protected MoveData MoveData { get; }

        protected Turn(ParsedUci parsedUci, MoveData moveData)
        {
            ParsedUci = parsedUci;
            MoveData = moveData;
        }

        public virtual void Progress(float t)
        {

        }

        public virtual void End()
        {

        }

        public virtual void EndUndo()
        {

        }
    }

    public class SimpleMove : Turn
    {
        public SimpleMove(ParsedUci parsedUci, MoveData moveData) : base(parsedUci, moveData)
        {
        }

        public override void Progress(float t)
        {
            Vector3 from = ParsedUci.FromSquare.transform.position;
            Vector3 to = ParsedUci.ToSquare.transform.position;
            Vector3 pos = Vector3.Lerp(from, to, Easing.InOutCubic(t));

            ParsedUci.Piece.MoveTo(pos);
        }

        public override void End()
        {
            ParsedUci.Piece.SetNewSquare(ParsedUci.ToSquare);

            if (MoveData.IsFirstMove)
            {
                ParsedUci.Piece.IsFirstMove = false;
            }
        }

        public override void EndUndo()
        {
            ParsedUci.Piece.SetNewSquare(ParsedUci.FromSquare);

            if (MoveData.IsFirstMove)
            {
                ParsedUci.Piece.IsFirstMove = true;
            }
        }
    }

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
            Piece beatenPiece = ParsedUci.ToSquare.GetPiece();

            beatenPiece.RemoveFromBoard();
            movedPiece.SetNewSquare(ParsedUci.ToSquare);
            BeatenPieces.Instance.Add(beatenPiece);

            if (MoveData.IsFirstMove)
            {
                movedPiece.IsFirstMove = false;
            }
        }

        public override void EndUndo()
        {
            Piece movedPiece = ParsedUci.ToSquare.GetPiece();
            Piece beatenPiece = BeatenPieces.Instance.Remove();

            movedPiece.SetNewSquare(ParsedUci.FromSquare);
            beatenPiece.AddToBoard(ParsedUci.ToSquare);
            beatenPiece.SetNewSquare(ParsedUci.ToSquare);

            if (MoveData.IsFirstMove)
            {
                movedPiece.IsFirstMove = true;
            }
        }
    }

    public class Castling : Turn
    {
        public Castling(ParsedUci parsedUci, MoveData moveData) : base(parsedUci, moveData)
        {
        }
    }
}