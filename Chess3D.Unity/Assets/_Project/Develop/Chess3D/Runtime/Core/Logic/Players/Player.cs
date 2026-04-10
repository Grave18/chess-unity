namespace Chess3D.Runtime.Core.Logic.Players
{
    public sealed class Player : IPlayer
    {
        private readonly IInputHandler _inputHandler;

        public Player(IInputHandler inputHandler)
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