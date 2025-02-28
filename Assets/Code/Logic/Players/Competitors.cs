using UnityEngine;

namespace Logic.Players
{
    public class Competitors : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Game game;
        [SerializeField] private Player playerWhite;
        [SerializeField] private Player playerBlack;


        private void OnEnable()
        {
            game.OnEndTurn += OnEndTurn;
        }

        private void OnDisable()
        {
            game.OnEndTurn -= OnEndTurn;
        }

        private void OnEndTurn()
        {
            if (game.CurrentTurnColor == PieceColor.White)
            {
                playerWhite.AllowMakeMove();
                playerBlack.DisallowMakeMove();
            }
            else if (game.CurrentTurnColor == PieceColor.Black)
            {
                playerBlack.AllowMakeMove();
                playerWhite.DisallowMakeMove();
            }
        }
    }
}