using Logic.CommandPattern;
using UnityEngine;
using UnityEngine.UI;

namespace UndoRedo
{
    public class UndoRedoUi : MonoBehaviour
    {
        [SerializeField] private CommandHandler commandHandler;
        [SerializeField] private Button undo;
        [SerializeField] private Button redo;

        private void Start()
        {
            undo.onClick.AddListener(commandHandler.Undo);
            redo.onClick.AddListener(commandHandler.Redo);
        }
    }
}
