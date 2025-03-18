using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class UndoRedoPausePanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CommandInvoker commandInvoker;
        [SerializeField] private Game game;

        [Header("UI")]
        [SerializeField] private Button undo;
        [SerializeField] private Button redo;
        [SerializeField] private Button pause;
        [SerializeField] private Image pauseImage;

        [Header("Presets")]
        [SerializeField] private Sprite playSprite;
        [SerializeField] private Sprite pauseSprite;

        private void Awake()
        {
            undo.onClick.AddListener(OnUndoPressed);
            redo.onClick.AddListener(OnRedoPressed);
            pause.onClick.AddListener(OnPlayPausePressed);
        }

        private void OnEnable()
        {
            game.OnPause += OnPause;
            game.OnPlay += OnPlay;
        }

        private void OnDisable()
        {
            game.OnPause -= OnPause;
            game.OnPlay -= OnPlay;
        }

        private void OnUndoPressed()
        {
            _ = commandInvoker.Undo();
        }

        private void OnRedoPressed()
        {
            _ = commandInvoker.Redo();
        }

        private void OnPlayPausePressed()
        {
            if (pauseImage.sprite == pauseSprite)
            {
                game.Pause();
            }
            else if(pauseImage.sprite == playSprite)
            {
                game.Play();
            }
        }

        private void OnPlay()
        {
            pauseImage.sprite = pauseSprite;
        }

        private void OnPause()
        {
            pauseImage.sprite = playSprite;
        }
    }
}
