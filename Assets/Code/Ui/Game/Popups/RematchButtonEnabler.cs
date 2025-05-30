using Network;
using Ui.Common.Buttons;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ui.Game.Popups
{
    [DefaultExecutionOrder(1)] // Needed for ButtonBase.Awake() is run before this OnEnable
    public class RematchButtonEnabler : MonoBehaviour
    {
        [SerializeField] private ChessGame.Logic.Game game;

        private void OnEnable()
        {
            SwitchButtonEnabled();
        }

        private void SwitchButtonEnabled()
        {
            var buttonBase = GetComponent<ButtonBase>();
            Assert.IsNotNull(buttonBase, "Rematch button not found");

            bool isEnabled = OnlineInstanceHandler.IsOffline || (OnlineInstanceHandler.IsOnline && game.IsGameOver());
            buttonBase.Interactable = isEnabled;
        }
    }
}