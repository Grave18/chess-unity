using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Online;
using PurrNet;
using TMPro;
using UnityEngine;

namespace Chess3D.Runtime.Core.Ui.DebugUi
{
    public class StateDebugPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Logic.Game game;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        [Header("Ui texts")]
        [SerializeField] private TMP_Text authorityText;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text checkText;
        [SerializeField] private TMP_Text playerTypeText;

        private void Update()
        {
            if (!game)
            {
                return;
            }

            authorityText.text = "Authority: " + GetAuthority();
            stateText.text = "State: " + game.GameStateMachine?.StateName;
            turnText.text = "Turn: " + game.CurrentTurnColor;
            checkText.text = "Check: " + game.CheckType;
            playerTypeText.text = "Player: " + gameSettingsContainer.GameSettings.Player1Settings.PlayerType;
        }

        private static string GetAuthority()
        {
            if (InstanceHandler.NetworkManager == null || OnlineInstanceHandler.IsOffline)
            {
                return "Offline";
            }

            if (InstanceHandler.NetworkManager.isHost)
            {
                return "Host";
            }

            if (InstanceHandler.NetworkManager.isServer)
            {
                return "Server";
            }

            return "Client";
        }
    }
}
