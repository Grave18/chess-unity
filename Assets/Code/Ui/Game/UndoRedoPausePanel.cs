using ChessGame.Logic;
using Network;
using Ui.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game
{
    public class UndoRedoPausePanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessGame.Logic.Game game;
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
            game.OnEndMoveColor += DisablePauseIfOnlineAndNotMoving;
            game.OnStartColor += DisablePauseIfOnlineAndNotMoving;
            undo.onClick.AddListener(OnUndoPressed);
            redo.onClick.AddListener(OnRedoPressed);
            pause.onClick.AddListener(OnPlayPausePressed);
        }

        private void OnDisable()
        {
            game.OnPlay -= OnPlay;
            game.OnPause -= OnPause;
            game.OnEndMoveColor -= DisablePauseIfOnlineAndNotMoving;
            game.OnStartColor -= DisablePauseIfOnlineAndNotMoving;
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
            game.Undo();
        }

        private void OnRedoPressed()
        {
            game.Redo();
        }

        private void OnPlayPausePressed()
        {
            if (_isPause)
            {
                game.Play();
            }
            else
            {
                game.Pause();
            }
        }
    }
}
