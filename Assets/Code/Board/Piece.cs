using Logic;
using UnityEngine;

namespace Board
{
    public class Piece : MonoBehaviour, IHighlightable, ISelectable
    {
        public void Highlight()
        {
            Debug.Log($"Piece: {name} is highlighted.");
        }

        public void DisableHighlight()
        {
            Debug.Log($"Piece: {name} highlight is disabled.");
        }

        public void Select()
        {
            Debug.Log($"Piece: {name} is selected.");
        }

        public void DisableSelect()
        {
            Debug.Log($"Piece: {name} selection is disabled.");
        }
    }
}
