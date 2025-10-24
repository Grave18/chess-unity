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

        public void Move(string uci)
        {
            if (isServer)
            {
                _game.GameStateMachine.Move(uci);
            }
            else
            {
                Move_ServerRpc(uci);
            }
        }

        [ServerRpc]
        private void Move_ServerRpc(string uci)
        {
            _game.GameStateMachine.Move(uci);
        }
    }
}