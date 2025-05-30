using Ui.Common.Buttons;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ui.Game.Popups
{
    [DefaultExecutionOrder(1)] // Needed for ButtonBase.Awake() is run before this OnEnable
    public class ResignButtonEnabler : MonoBehaviour
    {
        [SerializeField] private ChessGame.Logic.Game game;

        private void OnEnable()
        {
            SwitchButtonEnabled();
        }

        private void SwitchButtonEnabled()
        {
            var buttonBase = GetComponent<ButtonBase>();
            Assert.IsNotNull(buttonBase, "Resign button not found");

            bool isEnabled = !game.IsGameOver() && game.IsMyTurn();
            buttonBase.SetButtonEnabled(isEnabled);
        }
    }
}
