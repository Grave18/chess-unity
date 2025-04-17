using System.Threading;
using Ai;
using GameAndScene.Initialization;

namespace Logic.Players
{
    public class PlayerComputer : IPlayer
    {
        private readonly Game _game;
        private readonly Stockfish _stockfish;
        private readonly PlayerSettings _playerSettings;

        private CancellationTokenSource _moveCts = new ();

        public PlayerComputer(Game game, PlayerSettings playerSettings, Stockfish stockfish)
        {
            _game = game;
            _playerSettings = playerSettings;
            _stockfish = stockfish;
        }


        /// Get calculations from Ai and make move
        public async void StartPlayer()
        {
            string uci = await _stockfish.GetUci(_playerSettings, _moveCts.Token);

            if (uci == null)
            {
                return;
            }

            _game.Move(uci);
        }

        public void UpdatePlayer()
        {
            // Empty
        }

        // Stop All Ai calculations
        public void StopPlayer()
        {
            _moveCts?.Cancel();
            _moveCts = new CancellationTokenSource();
        }
    }
}