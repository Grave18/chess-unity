using Logic.MovesBuffer;
using Logic.Players.GameStates;
using UnityEngine;
using Utils.Mathematics;

namespace Logic.Players.Moves
{
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
}