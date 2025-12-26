using PurrNet;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public sealed class PlayerOnline : NetworkBehaviour, IPlayer
    {
        private IInputHandler _inputHandler;

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

        public void Load(IInputHandler input)
        {
            _inputHandler = input;
        }
    }
}