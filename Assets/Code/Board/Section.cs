using UnityEngine;

namespace Board
{
    public class Section : MonoBehaviour
    {
        private Piece currentPiece;
        private MeshRenderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        public void SetPiece(Piece piece)
        {
            currentPiece = piece;
        }

        public bool IsEmpty()
        {
            return currentPiece == null;
        }
    }
}
