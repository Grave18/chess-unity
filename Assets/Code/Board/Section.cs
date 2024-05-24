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

        public void Select()
        {
            _renderer.material.color = Color.magenta;
        }

        public void DisableSelect()
        {
            _renderer.material.color = Color.white;
        }
    }
}
