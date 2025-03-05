using System.Threading.Tasks;
using ChessBoard.Pieces;
using UnityEngine;

namespace Logic.Players
{
    public class Competitors : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Game game;
        [SerializeField] private Player playerWhite;
        [SerializeField] private Player playerBlack;

        private Player _currentPlayer;

        public async Task<PieceType> RequestPromotedPiece()
        {
            return await _currentPlayer.RequestPromotedPiece();
        }

        public void SelectPromotedPiece(PieceType pieceType)
        {
            _currentPlayer.SelectPromotedPiece(pieceType);
        }

        private void Awake()
        {
            _currentPlayer = playerWhite;
        }

        private void OnEnable()
        {
            game.OnPlay += OnPlay;
            game.OnPause += OnPause;
            game.OnEndTurn += OnEndTurn;
        }

        private void OnDisable()
        {
            game.OnPlay -= OnPlay;
            game.OnPause -= OnPause;
            game.OnEndTurn -= OnEndTurn;
        }

        private void OnPlay()
        {
            if (game.CurrentTurnColor == PieceColor.White)
            {
                playerWhite.AllowMakeMove();
            }
            else if (game.CurrentTurnColor == PieceColor.Black)
            {
                playerBlack.AllowMakeMove();
            }
        }

        private void OnPause()
        {
            if (game.CurrentTurnColor == PieceColor.White)
            {
                playerBlack.DisallowMakeMove();
            }
            else if (game.CurrentTurnColor == PieceColor.Black)
            {
                playerWhite.DisallowMakeMove();
            }
        }

        private void OnEndTurn()
        {
            // Prevent calculations after redo to computer turn
            // Real player not allow to start move if paused
            if (game.State != GameState.Idle)
            {
                return;
            }

            if (game.CurrentTurnColor == PieceColor.White)
            {
                _currentPlayer = playerWhite;
                playerWhite.AllowMakeMove();
                playerBlack.DisallowMakeMove();
            }
            else if (game.CurrentTurnColor == PieceColor.Black)
            {
                _currentPlayer = playerBlack;
                playerBlack.AllowMakeMove();
                playerWhite.DisallowMakeMove();
            }
        }
    }
}