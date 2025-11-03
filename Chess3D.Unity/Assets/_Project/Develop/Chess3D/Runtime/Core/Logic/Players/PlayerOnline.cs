using PurrNet;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public class PlayerOnline : NetworkBehaviour, IPlayer
    {
        private IInputHandler _inputHandler;

        public void Init(IInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
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
    }
}