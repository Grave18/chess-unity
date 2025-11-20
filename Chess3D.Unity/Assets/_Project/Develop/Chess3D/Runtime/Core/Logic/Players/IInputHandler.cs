using Chess3D.Runtime.Utilities;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public interface IInputHandler : ILoadUnit
    {
        public void StartInput();
        void UpdateInput();
        public void StopInput();
    }
}