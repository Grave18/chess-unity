using System.Collections;
using Logic;
using TMPro;
using UnityEngine;

namespace Ui
{
    public class GameStateDisplayPanel : MonoBehaviour
    {
        [SerializeField] private Game game;
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;

        private void OnEnable()
        {
            game.OnEndTurn += OnEndTurn;
        }

        private void Update()
        {
            string text = game.State switch
            {
                GameState.Idle => "Idle",
                GameState.Move => "Move",
                GameState.Pause => "Pause",
                GameState.Think => "Think",
                _ => string.Empty
            };

            if (game.CurrentTurnColor == PieceColor.White)
            {
                if (whiteText.text != text)
                {
                    whiteText.text = text;
                }
            }
            else if (game.CurrentTurnColor == PieceColor.Black)
            {
                if (blackText.text != text)
                {
                    blackText.text = text;
                }
            }
        }

        private void OnEndTurn()
        {
            if (game.CurrentTurnColor == PieceColor.White)
            {
                blackText.text = "Idle";
                whiteText.CrossFadeAlpha(1f, 0.2f, false);
                blackText.CrossFadeAlpha(0f, 0.2f, false);
            }
            else if (game.CurrentTurnColor == PieceColor.Black)
            {
                whiteText.text = "Idle";
                whiteText.CrossFadeAlpha(0f, 0.2f, false);
                blackText.CrossFadeAlpha(1f, 0.2f, false);
            }
        }

        private IEnumerator FadeText(string text, TMP_Text tmpText)
        {
            tmpText.alpha = 1f;
            while (tmpText.alpha > 0.5f)
            {
                tmpText.alpha -= 5f * Time.deltaTime;
                yield return null;
            }

            tmpText.text = text;

            while (tmpText.alpha < 1f)
            {
                tmpText.alpha += 5f * Time.deltaTime;
                yield return null;
            }
        }
    }
}