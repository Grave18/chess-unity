using ChessGame.Logic.MovesBuffer;
using TMPro;
using UnityEngine;

namespace Ui.Game
{
    public class BufferDebugPanel : MonoBehaviour
    {
        [SerializeField] private ChessGame.Logic.Game game;
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
