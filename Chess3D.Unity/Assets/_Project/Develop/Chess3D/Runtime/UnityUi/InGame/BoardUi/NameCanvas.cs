using TMPro;
using UnityEngine;

namespace Chess3D.Runtime.UnityUi.InGame.BoardUi
{
    public class NameCanvas : MonoBehaviour
    {
        [SerializeField] private TMP_Text whitePlayerNameText;
        [SerializeField] private TMP_Text blackPlayerNameText;

        public void Init(string whitePlayerName, string blackPlayerName)
        {
            whitePlayerNameText.text = whitePlayerName;
            blackPlayerNameText.text = blackPlayerName;
        }
    }
}
