using Chess3D.Runtime.ChessBoard.Info;
using Chess3D.Runtime.UtilsCommon.Mathematics;
using UnityEngine;

namespace Chess3D.Runtime.ChessBoard.Pieces
{
    public class Knight : Piece
    {
        [Header("Knight")]
        [SerializeField] private Vector2Int[] moves;

        [Header("Knight Movement settings")]
        [SerializeField] private float jumpHeight = 0.15f;
        [SerializeField] private EasingType jumpEasing = EasingType.InOutCubic;

        protected override void CalculateMovesAndCapturesInternal()
        {
            foreach (Vector2Int offset in moves)
            {
                Square square = Game.GetSquareRel(pieceColor, currentSquare, offset);

                if (square == Game.NullSquare)
                {
                    continue;
                }

                if (!square.HasPiece())
                {
                    MoveSquares.Add(square, new MoveInfo());
                }
                else if (square.GetPieceColor() != pieceColor)
                {
                    CaptureSquares.Add(square, new CaptureInfo(square.GetPiece()));
                }
                else if (square.GetPieceColor() == pieceColor)
                {
                    DefendSquares.Add(square);
                }
            }
        }

        public override void MoveTo(Vector3 from, Vector3 to, float t)
        {
            Vector3 moveComponent = Vector3.Lerp(from, to, Easing.Generic(t, moveEasing));
            Vector3 jumpComponent = GetArk(t);

            transform.position = moveComponent + jumpComponent;
        }

        private Vector3 GetArk(float t)
        {
            float jumpT = Easing.Generic(t, jumpEasing);
            float arc = 4f * jumpHeight * jumpT * (1f - jumpT);
            return Vector3.up * arc;
        }
    }
}