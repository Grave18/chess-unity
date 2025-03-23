using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class UndoRedoPausePanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Game game;

        [Header("UI")]
        [SerializeField] private Button undo;
        [SerializeField] private Button redo;
        [SerializeField] private Button pause;
        [SerializeField] private Image pauseImage;

        [Header("Presets")]
        [SerializeField] private Sprite playSprite;
        [SerializeField] private Sprite pauseSprite;

        private bool _isPause;

        private void OnEnable()
        {
            game.OnPlay += OnPlay;
            game.OnPause += OnPause;
            undo.onClick.AddListener(OnUndoPressed);
            redo.onClick.AddListener(OnRedoPressed);
            pause.onClick.AddListener(OnPlayPausePressed);
        }

        private void OnDisable()
        {
            game.OnPlay -= OnPlay;
            game.OnPause -= OnPause;
            undo.onClick.RemoveListener(OnUndoPressed);
            redo.onClick.RemoveListener(OnRedoPressed);
            pause.onClick.RemoveListener(OnPlayPausePressed);
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
