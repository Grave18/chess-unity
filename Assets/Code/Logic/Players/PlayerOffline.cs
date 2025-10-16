namespace Logic.Players
{
    public class PlayerOffline : IPlayer
    {
        private readonly Game _game;
        private readonly IInputHandler _inputHandler;

        public PlayerOffline(Game game, IInputHandler inputHandler)
        {
            _game = game;
            _inputHandler = inputHandler;

            _inputHandler.SetPlayer(this);
        }

        public void StartPlayer()
        {
            _inputHandler.StartInput();
        }

        public void UpdatePlayer()
        {
            _inputHandler.UpdateInput();
        }

        public void StopPlayer()
        {
            _inputHandler.StopInput();
        }

        public void Move(string uci)
        {
            _game.GameStateMachine.Move(uci);
        }
    }
}