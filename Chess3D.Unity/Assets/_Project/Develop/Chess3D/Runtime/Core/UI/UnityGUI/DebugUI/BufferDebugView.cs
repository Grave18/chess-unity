using Chess3D.Runtime.Core.Logic.MovesBuffer;
using TMPro;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.DebugUI
{
    public class BufferDebugView : MonoBehaviour
    {
        [SerializeField] private TMP_Text uciText;

        private UciBuffer _uciBuffer;

        [Inject]
        public void Construct(UciBuffer uciBuffer)
        {
            _uciBuffer = uciBuffer;
        }

        private void Update()
        {
            if(_uciBuffer == null)
            {
                return;
            }

            uciText.text = _uciBuffer.GetAllUciDebug();
        }
    }
}
