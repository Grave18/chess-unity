using UnityEngine;

namespace Logic.Players
{
    public class Competitors : MonoBehaviour
    {
        private Game _game;
        private IPlayer _playerWhite;
        private IPlayer _playerBlack;
        private IPlayer _currentPlayer;

        public void Init(Game game, IPlayer playerWhite, IPlayer playerBlack)
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

        public void SubstitutePlayers(IPlayer playerWhite, IPlayer playerBlack)
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