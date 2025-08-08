using UnityEngine;
using UnityEngine.Assertions;
using UnityUi.Common.Buttons;

namespace UnityUi.InGame.Popups
{
    [DefaultExecutionOrder(1)] // Needed for ButtonBase.Awake() is run before this OnEnable
    public class ResignButtonEnabler : MonoBehaviour
    {
        [SerializeField] private Logic.Game game;

        private void OnEnable()
        {
            SwitchButtonEnabled();
        }

        private void SwitchButtonEnabled()
        {
            var buttonBase = GetComponent<ButtonBase>();
            Assert.IsNotNull(buttonBase, "Resign button not found");

            bool isEnabled = !game.IsGameOver && game.IsMyTurn;
            buttonBase.Interactable = isEnabled;
        }
    }
}
