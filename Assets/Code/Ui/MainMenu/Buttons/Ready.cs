using ChessGame.Logic;
using GameAndScene;
using PurrLobby;
using Ui.Common.Buttons;
using UnityEngine;

namespace Ui.MainMenu.Buttons
{
    public class Ready : ButtonCallbackBase
    {
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        private bool _isReady;

        protected override void OnClick()
        {
            int playerIndexInLobby = lobbyManager.GetPlayerIndexInLobby();

            PieceColor playerColor = playerIndexInLobby switch
            {
                0 => PieceColor.White,
                1 => PieceColor.Black,
                _ => PieceColor.None,
            };

            gameSettingsContainer.SetupGameOnline(playerColor);
            lobbyManager.ToggleLocalReady();
        }
    }
}