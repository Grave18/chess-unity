using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.Players;
using TMPro;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.DebugUI
{
    public sealed class StateDebugView : MonoBehaviour
    {
        [Header("Ui texts")]
        [SerializeField] private TMP_Text authorityText;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text checkText;
        [SerializeField] private TMP_Text playerTypeText;

        private Game _game;
        private IGameStateMachine _gameStateMachine;
        private ISettingsContainer _settingsContainer;
        private ConfigContainer _configContainer;

        [Inject]
        public void Construct(Game game, IGameStateMachine gameStateMachine, ISettingsContainer settingsContainer,
            ConfigContainer configContainer)
        {
            _game = game;
            _gameStateMachine = gameStateMachine;
            _settingsContainer = settingsContainer;
            _configContainer = configContainer;
        }

        private void Awake()
        {
#if GRAVEBOX_PROD
            gameObject.SetActive(false);
#endif
        }

        private void Update()
        {
            if (_game is null)
            {
                return;
            }

            stateText.text = "State: " + _gameStateMachine?.StateName;
            turnText.text = "Turn: " + _game.CurrentTurnColor;
            checkText.text = "Check: " + _game.CheckType;
            playerTypeText.text = "Player: " + _settingsContainer.GameSettings.PlayerWhite.PlayerType;
        }
    }
}
