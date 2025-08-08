using Logic;
using Network;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUi.InGame
{
    public class UndoRedoPausePanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Logic.Game game;
        [SerializeField] private Initialization.Initialization initialization;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        [Header("UI")]
        [SerializeField] private Button undo;
        [SerializeField] private Button redo;
        [SerializeField] private Button pause;
        [SerializeField] private Image pauseImage;

        [Header("Presets")]
        [SerializeField] private Sprite playSprite;
        [SerializeField] private Sprite pauseSprite;

        private bool _isPause;

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
            game.OnPlay += OnPlay;
            game.OnPause += OnPause;
            game.OnEndMoveWithColor += DisablePauseIfOnlineAndNotMoving;
            game.OnStartWithColor += DisablePauseIfOnlineAndNotMoving;
            undo.onClick.AddListener(OnUndoPressed);
            redo.onClick.AddListener(OnRedoPressed);
            pause.onClick.AddListener(OnPlayPausePressed);
        }

        private void OnDisable()
        {
            game.OnPlay -= OnPlay;
            game.OnPause -= OnPause;
            game.OnEndMoveWithColor -= DisablePauseIfOnlineAndNotMoving;
            game.OnStartWithColor -= DisablePauseIfOnlineAndNotMoving;
            undo.onClick.RemoveListener(OnUndoPressed);
            redo.onClick.RemoveListener(OnRedoPressed);
            pause.onClick.RemoveListener(OnPlayPausePressed);
        }

        private void DisablePauseIfOnlineAndNotMoving(PieceColor currentTurnColor)
        {
            if (OnlineInstanceHandler.IsOnline)
            {
                PieceColor playerColor = gameSettingsContainer.GameSettings.PlayerColor;
                pause.interactable = playerColor == currentTurnColor;
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
            game.GameStateMachine.Undo();
        }

        private void OnRedoPressed()
        {
            game.GameStateMachine.Redo();
        }

        private void OnPlayPausePressed()
        {
            if (_isPause)
            {
                game.GameStateMachine.Play();
            }
            else
            {
                game.GameStateMachine.Pause();
            }
        }
    }
}
