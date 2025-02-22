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

        private void OnEnable()
        {
            undo.onClick.AddListener(commandInvoker.Undo);
            redo.onClick.AddListener(commandInvoker.Redo);
            pause.onClick.AddListener(PlayPause);
        }

        private void PlayPause()
        {
            if (game.State == GameState.Idle)
            {
                game.Pause();
                pauseImage.sprite = playSprite;
            }
            else if(game.State == GameState.Pause)
            {
                game.Play();
                pauseImage.sprite = pauseSprite;
            }
        }
    }
}
