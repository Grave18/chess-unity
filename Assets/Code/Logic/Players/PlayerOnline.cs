using PurrNet;

namespace Logic.Players
{
    public class PlayerOnline : NetworkBehaviour, IPlayer
    {
        private Game _game;
        private IInputHandler _inputHandler;

        public void Init(Game game, IInputHandler inputHandler)
        {
            _game = game;
            _inputHandler = inputHandler;

            inputHandler.SetPlayer(this);
        }

        public void StartPlayer()
        {
            if (isController)
            {
                _inputHandler.StartInput();
            }
        }

        public void UpdatePlayer()
        {
            if (isController)
            {
                _inputHandler.UpdateInput();
            }
        }

        public void StopPlayer()
        {
            if (isController)
            {
                _inputHandler.StopInput();
            }
        }

        [ObserversRpc(runLocally: true)]
        public void Move(string uci)
        {
            _game.GameStateMachine.Move(uci);
        }
    }
}