using ChessGame.Logic;
using Network;
using Ui.Common.Buttons;
using UnityEngine;
using UnityEngine.Assertions;

// Need for button base Awake is run before this OnEnable
[DefaultExecutionOrder(1)]
public class DrawButtonOnlineEnabler : MonoBehaviour
{
    [SerializeField] private Game game;

    private void OnEnable()
    {
        EnableButtonIfOnline();
    }

    private void EnableButtonIfOnline()
    {
        var buttonBase = GetComponent<ButtonBase>();
        Assert.IsNotNull(buttonBase, "Draw button not found");

        bool isEnabled = OnlineInstanceHandler.IsOnline && !game.IsGameOver();
        buttonBase.SetButtonEnabled(isEnabled);
    }
}
