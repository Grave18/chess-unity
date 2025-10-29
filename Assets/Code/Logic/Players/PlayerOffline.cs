namespace Logic.Players
{
    public class PlayerOffline : IPlayer
    {
        private readonly IInputHandler _inputHandler;

        public PlayerOffline(IInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
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
    }
}