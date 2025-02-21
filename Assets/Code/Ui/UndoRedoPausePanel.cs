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
            }
            else if(game.State == GameState.Pause)
            {
                game.Play();
            }
        }
    }
}
