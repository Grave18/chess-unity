using TMPro;
using UnityEngine;

namespace Ui.Game
{
    public class StateDebugPanel : MonoBehaviour
    {
        [SerializeField] private Logic.Game game;

        [SerializeField] private TMP_Text stateText;
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text checkText;

        private void Update()
        {
            stateText.text = "State: " + game.GetStateName();
            turnText.text = "Turn: " + game.CurrentTurnColor;
            checkText.text = "Check: " + game.CheckType;
        }
    }
}
