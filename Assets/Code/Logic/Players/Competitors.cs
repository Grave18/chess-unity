using UnityEngine;

namespace Logic.Players
{
    public class Competitors : MonoBehaviour
    {
        private Game _game;
        private Player _playerWhite;
        private Player _playerBlack;
        private Player _currentPlayer;

        public void Init(Game game, Player playerWhite, Player playerBlack)
        {
            _game = game;
            _playerWhite = playerWhite;
            _playerBlack = playerBlack;
            _currentPlayer = _playerWhite;
        }

        public void StartPlayer()
        {
            _currentPlayer.Start();
        }

        public void UpdatePlayer()
        {
            _currentPlayer.Update();
        }

        public void StopPlayer()
        {
            _currentPlayer.Stop();
        }

        public void SubstitutePlayers(Player playerWhite, Player playerBlack)
        {
            _playerWhite = playerWhite;
            _playerBlack = playerBlack;

            ChangeCurrentPlayer();
        }

        public void ChangeCurrentPlayer()
        {
            if (_game.CurrentTurnColor == PieceColor.White)
            {
                _currentPlayer = _playerWhite;
            }
            else if (_game.CurrentTurnColor == PieceColor.Black)
            {
                _currentPlayer = _playerBlack;
            }
        }
    }
}