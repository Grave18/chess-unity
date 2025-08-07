using UnityEngine;

namespace Logic.Players
{
    public class Competitors : MonoBehaviour
    {
        private Game _game;
        private IPlayer _playerWhite;
        private IPlayer _playerBlack;

        private IPlayer _initialPlayer;
        private IPlayer _currentPlayer;

        public void Init(Game game, IPlayer playerWhite, IPlayer playerBlack, PieceColor firstMoveColor)
        {
            _game = game;
            _playerWhite = playerWhite;
            _playerBlack = playerBlack;

            _initialPlayer = firstMoveColor == PieceColor.White ? _playerWhite : _playerBlack;
            _currentPlayer = _initialPlayer;
        }

        public void Restart()
        {
            _currentPlayer = _initialPlayer;
        }

        public void StartPlayer()
        {
            _currentPlayer.StartPlayer();
        }

        public void UpdatePlayer()
        {
            _currentPlayer.UpdatePlayer();
        }

        public void StopPlayer()
        {
            _currentPlayer.StopPlayer();
        }

        public void SubstitutePlayers(IPlayer playerWhite, IPlayer playerBlack)
        {
            _playerWhite = playerWhite;
            _playerBlack = playerBlack;

            SwapCurrentPlayer();
        }

        public void SwapCurrentPlayer()
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