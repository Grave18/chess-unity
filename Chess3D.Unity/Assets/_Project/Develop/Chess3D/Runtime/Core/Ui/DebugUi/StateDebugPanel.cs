using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Online;
using PurrNet;
using TMPro;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Ui.DebugUi
{
    public class StateDebugPanel : MonoBehaviour
    {
        [Header("Ui texts")]
        [SerializeField] private TMP_Text authorityText;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text checkText;
        [SerializeField] private TMP_Text playerTypeText;

        private Game _game;
        private IGameStateMachine _gameStateMachine;
        private ISettingsService _settingsService;

        [Inject]
        public void Construct(Game game, IGameStateMachine gameStateMachine, ISettingsService settingsService)
        {
            _game = game;
            _gameStateMachine = gameStateMachine;
            _settingsService = settingsService;
        }

        private void Update()
        {
            if (_game is null)
            {
                return;
            }

            authorityText.text = "Authority: " + GetAuthority();
            stateText.text = "State: " + _gameStateMachine?.StateName;
            turnText.text = "Turn: " + _game.CurrentTurnColor;
            checkText.text = "Check: " + _game.CheckType;
            playerTypeText.text = "Player: " + _settingsService.S.GameSettings.PlayerWhite.PlayerType;
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
