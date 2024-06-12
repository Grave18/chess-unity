using Logic.CommandPattern;
using UnityEngine;
using UnityEngine.UI;

namespace UndoRedo
{
    public class UndoRedoUi : MonoBehaviour
    {
        [SerializeField] private CommandManager _commandManager;
        [SerializeField] private Button undo;
        [SerializeField] private Button redo;

        private void Start()
        {
            undo.onClick.AddListener(_commandManager.Undo);
            redo.onClick.AddListener(_commandManager.Redo);
        }
    }
}
