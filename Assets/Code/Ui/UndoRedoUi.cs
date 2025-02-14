using Logic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UndoRedo
{
    public class UndoRedoUi : MonoBehaviour
    {
        [FormerlySerializedAs("_commandManager")]
        [SerializeField] private CommandInvoker commandInvoker;
        [SerializeField] private Button undo;
        [SerializeField] private Button redo;

        private void Start()
        {
            undo.onClick.AddListener(commandInvoker.Undo);
            redo.onClick.AddListener(commandInvoker.Redo);
        }
    }
}
