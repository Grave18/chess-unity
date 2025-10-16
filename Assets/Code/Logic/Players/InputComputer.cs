using System.Threading;
using Ai;
using Settings;

namespace Logic.Players
{
    public class InputComputer : IInputHandler
    {
        private readonly Stockfish _stockfish;
        private readonly PlayerSettings _playerSettings;

        private IPlayer _player;
        private CancellationTokenSource _moveCts = new();

        public InputComputer(PlayerSettings playerSettings, Stockfish stockfish)
        {
            _playerSettings = playerSettings;
            _stockfish = stockfish;
        }

        public void SetPlayer(IPlayer player)
        {
            _player = player;
        }

        public async void StartInput()
        {
            string uci = await _stockfish.GetUci(_playerSettings, _moveCts.Token);

            if (uci == null)
            {
                return;
            }

            _player.Move(uci);
        }

        public void UpdateInput()
        {
            // Noop
        }

        public void StopInput()
        {
            _moveCts?.Cancel();
            _moveCts = new CancellationTokenSource();
        }
    }
}