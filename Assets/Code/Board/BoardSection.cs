using Logic;
using UnityEngine;

namespace Board
{
    public class BoardSection : MonoBehaviour, IHighlightable, ISelectable
    {
        public void Highlight()
        {
            Debug.Log($"Section: {name} is highlighted.");
        }

        public void DisableHighlight()
        {
            Debug.Log($"Section: {name} highlight is disabled.");
        }

        public void Select()
        {
            Debug.Log($"Section: {name} is selected.");
        }

        public void DisableSelect()
        {
            Debug.Log($"Section: {name} selection is disabled.");
        }
    }
}
