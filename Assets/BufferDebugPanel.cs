using Logic;
using Logic.MovesBuffer;
using TMPro;
using UnityEngine;

public class BufferDebugPanel : MonoBehaviour
{
    [SerializeField] private Game game;

    [SerializeField] private TMP_Text uciText;

    private void Update()
    {
        Buffer buffer = game.CommandBuffer;
        if(buffer == null)
        {
            return;
        }

        uciText.text = buffer.GetAllUciDebug();
    }
}
