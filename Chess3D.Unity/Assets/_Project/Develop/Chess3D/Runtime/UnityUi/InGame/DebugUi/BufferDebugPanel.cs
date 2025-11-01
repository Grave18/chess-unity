using Chess3D.Runtime.Logic.MovesBuffer;
using TMPro;
using UnityEngine;

namespace Chess3D.Runtime.UnityUi.InGame.DebugUi
{
    public class BufferDebugPanel : MonoBehaviour
    {
        [SerializeField] private Logic.Game game;
        [SerializeField] private TMP_Text uciText;

        private void Update()
        {
            UciBuffer uciBuffer = game.UciBuffer;
            if(uciBuffer == null)
            {
                return;
            }

            uciText.text = uciBuffer.GetAllUciDebug();
        }
    }
}
