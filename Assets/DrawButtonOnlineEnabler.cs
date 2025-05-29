using ChessGame.Logic;
using Network;
using Ui.Common.Buttons;
using UnityEngine;
using UnityEngine.Assertions;

[DefaultExecutionOrder(1)] // Needed for ButtonBase.Awake() is run before this OnEnable
public class DrawButtonOnlineEnabler : MonoBehaviour
{
    [SerializeField] private Game game;

    private void OnEnable()
    {
        SwitchButtonEnabled();
    }

    private void SwitchButtonEnabled()
    {
        var buttonBase = GetComponent<ButtonBase>();
        Assert.IsNotNull(buttonBase, "Draw button not found");

        bool isEnabled = OnlineInstanceHandler.IsOnline && !game.IsGameOver() && game.IsMyTurn();
        buttonBase.SetButtonEnabled(isEnabled);
    }
}
