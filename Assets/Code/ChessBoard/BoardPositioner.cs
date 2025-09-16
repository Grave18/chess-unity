using UnityEngine;

namespace ChessBoard
{
    public class BoardPositioner : MonoBehaviour
    {
        [SerializeField] private Board board;
        [SerializeField] private BeatenPieces beatenPieces;

        public void PositionForWhitePieces()
        {
            board.gameObject.transform.localRotation = Quaternion.identity;
            beatenPieces.gameObject.transform.localScale = Vector3.one;
        }

        public void PositionForBlackPieces()
        {
            board.gameObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
            beatenPieces.gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
