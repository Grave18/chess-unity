using ChessGame.Logic;
using TMPro;
using UnityEngine;

namespace Ui.Game
{
    public class GameStateDisplayPanel : MonoBehaviour
    {
        [SerializeField] private ChessGame.Logic.Game game;
        [SerializeField] private TMP_Text whiteText;
        [SerializeField] private TMP_Text blackText;

        private void OnEnable()
        {
            // game.OnEndTurn += OnEndTurn;
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
    }
}