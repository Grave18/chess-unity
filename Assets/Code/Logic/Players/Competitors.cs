using System.Threading.Tasks;
using ChessBoard.Info;
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

        public void ChangePlayers(Player playerWhite, Player playerBlack)
        {
            _playerWhite.Stop();
            _playerWhite.Stop();
            _playerWhite = playerWhite;
            _playerBlack = playerBlack;
            _playerWhite.Start();
            _playerWhite.Start();

            OnEndTurn();
        }

        public async Task<PieceType> RequestPromotedPiece()
        {
            return await _currentPlayer.RequestPromotedPiece();
        }

        public void SelectPromotedPiece(PieceType pieceType)
        {
            _currentPlayer.SelectPromotedPiece(pieceType);
        }

        private void OnPlay()
        {
            if (_game.CurrentTurnColor == PieceColor.White)
            {
                _playerWhite.AllowMakeMove();
            }
            else if (_game.CurrentTurnColor == PieceColor.Black)
            {
                _playerBlack.AllowMakeMove();
            }
        }

        private void OnPause()
        {
            if (_game.CurrentTurnColor == PieceColor.White)
            {
                _playerBlack.DisallowMakeMove();
            }
            else if (_game.CurrentTurnColor == PieceColor.Black)
            {
                _playerWhite.DisallowMakeMove();
            }
        }

        private void OnEndTurn()
        {
            // Prevent calculations after redo to computer turn
            // Real player not allow to start move if paused
            if (_game.State != GameState.Idle)
            {
                return;
            }

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
            _game.OnPlay += OnPlay;
            _game.OnPause += OnPause;
            _game.OnEndTurn += OnEndTurn;
        }

        private void OnDestroy()
        {
            _game.OnPlay -= OnPlay;
            _game.OnPause -= OnPause;
            _game.OnEndTurn -= OnEndTurn;
        }
    }
}