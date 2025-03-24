using System.Threading;
using Ai;
using GameAndScene.Initialization;

namespace Logic.Players
{
    public class Computer : Player
    {
        private readonly Stockfish _stockfish;
        private readonly PlayerSettings _playerSettings;

        private CancellationTokenSource _moveCts = new ();

        public Computer(Game game, PlayerSettings playerSettings, Stockfish stockfish)
        :base(game)
        {
            _playerSettings = playerSettings;
            _stockfish = stockfish;
        }

        /// Get calculations from Ai and make move
        public override async void Start()
        {
            string uci = await _stockfish.GetUci(_playerSettings, _moveCts.Token);

            if (uci == null)
            {
                return;
            }

            Game.Move(uci);
        }

        // Stop All Ai calculations
        public override void Stop()
        {
            _moveCts?.Cancel();
            _moveCts = new CancellationTokenSource();
        }
    }
}