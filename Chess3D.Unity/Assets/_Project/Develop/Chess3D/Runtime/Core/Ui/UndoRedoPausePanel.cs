using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Online;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Chess3D.Runtime.Core.Ui
{
    public class UndoRedoPausePanel : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button undo;
        [SerializeField] private Button redo;
        [SerializeField] private Button pause;
        [SerializeField] private Image pauseImage;

        [Header("Presets")]
        [SerializeField] private Sprite playSprite;
        [SerializeField] private Sprite pauseSprite;

        private CoreEvents _coreEvents;
        private SettingsService _settingsService;
        private IGameStateMachine _gameStateMachine;

        private bool _isPause;

        [Inject]
        public void Construct(CoreEvents coreEvents, SettingsService settingsService, IGameStateMachine gameStateMachine)
        {
            _coreEvents = coreEvents;
            _settingsService = settingsService;
            _gameStateMachine = gameStateMachine;
        }

        private void Start()
        {
            HandleUndoRedoButtons();
        }

        private void HandleUndoRedoButtons()
        {
            if (OnlineInstanceHandler.IsOnline)
            {
                undo.interactable = false;
                redo.interactable = false;
            }
            else
            {
                undo.interactable = true;
                redo.interactable = true;
            }
        }

        private void OnEnable()
        {
            _coreEvents.OnPlay += OnPlay;
            _coreEvents.OnPause += OnPause;
            // TODO: Refactor this mess
            // _coreEvents.OnEndMoveWithColor += DisablePauseIfOnlineAndNotMoving;
            // _coreEvents.OnStartWithColor += DisablePauseIfOnlineAndNotMoving;
            undo.onClick.AddListener(OnUndoPressed);
            redo.onClick.AddListener(OnRedoPressed);
            pause.onClick.AddListener(OnPlayPausePressed);
        }

        private void OnDisable()
        {
            _coreEvents.OnPlay -= OnPlay;
            _coreEvents.OnPause -= OnPause;
            // _coreEvents.OnEndMoveWithColor -= DisablePauseIfOnlineAndNotMoving;
            // _coreEvents.OnStartWithColor -= DisablePauseIfOnlineAndNotMoving;
            undo.onClick.RemoveListener(OnUndoPressed);
            redo.onClick.RemoveListener(OnRedoPressed);
            pause.onClick.RemoveListener(OnPlayPausePressed);
        }

        private void DisablePauseIfOnlineAndNotMoving(PieceColor currentTurnColor)
        {
            if (OnlineInstanceHandler.IsOnline)
            {
                // TODO: Refactor this mess
                // PieceColor playerColor = _settingsService.GameSettings.PlayerColor;
                // pause.interactable = playerColor == currentTurnColor;
            }
            else
            {
                pause.interactable = true;
            }
        }


        private void OnPlay()
        {
            pauseImage.sprite = pauseSprite;
            _isPause = false;
        }

        private void OnPause()
        {
            pauseImage.sprite = playSprite;
            _isPause = true;
        }

        private void OnUndoPressed()
        {
            _gameStateMachine.Undo();
        }

        private void OnRedoPressed()
        {
            _gameStateMachine.Redo();
        }

        private void OnPlayPausePressed()
        {
            if (_isPause)
            {
                _gameStateMachine.Play();
            }
            else
            {
                _gameStateMachine.Pause();
            }
        }
    }
}
