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

            SubscribeToEvents();
        }
        public void UpdatePlayer()
        {
            _currentPlayer.Update();
        }

        public void ChangePlayers(Player playerWhite, Player playerBlack)
        {
            _playerWhite = playerWhite;
            _playerBlack = playerBlack;

            OnEndTurn();
        }

        private void OnPlay()
        {
            _currentPlayer.AllowMakeMove();
        }

        private void OnPause()
        {
            _currentPlayer.DisallowMakeMove();
        }

        private void OnEndTurn()
        {
            if (_game.CurrentTurnColor == PieceColor.White)
            {
                _currentPlayer = _playerWhite;
                _playerWhite.AllowMakeMove();
                _playerBlack.DisallowMakeMove();
            }
            else if (_game.CurrentTurnColor == PieceColor.Black)
            {
                _currentPlayer = _playerBlack;
                _playerBlack.AllowMakeMove();
                _playerWhite.DisallowMakeMove();
            }
        }

        private void SubscribeToEvents()
        {
            // _game.OnPlay += OnPlay;
            // _game.OnPause += OnPause;
            // _game.OnEndTurn += OnEndTurn;
        }

        private void OnDestroy()
        {
            // _game.OnPlay -= OnPlay;
            // _game.OnPause -= OnPause;
            // _game.OnEndTurn -= OnEndTurn;
        }
    }
}