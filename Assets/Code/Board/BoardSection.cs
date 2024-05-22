using Logic;
using UnityEngine;

namespace Board
{
    public class BoardSection : MonoBehaviour, IHighlightable, ISelectable
    {
        private Piece currentPiece;

        public void Highlight()
        {
            Debug.Log($"Section: {name} is highlighted.");
            currentPiece?.Highlight();
        }

        public void DisableHighlight()
        {
            Debug.Log($"Section: {name} highlight is disabled.");
            currentPiece?.DisableHighlight();
        }

        public void Select()
        {
            Debug.Log($"Section: {name} is selected.");
            currentPiece?.Select();
        }

        public void DisableSelect()
        {
            Debug.Log($"Section: {name} selection is disabled.");
            currentPiece?.DisableSelect();
        }

        public void Move(Vector3 target)
        {
            if(currentPiece != null)
            {
                Debug.Log($"Section: {name} delegate move to {currentPiece.name}.");
                currentPiece.Move(target);
            }
        }
    }
}
