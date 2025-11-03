namespace Chess3D.Runtime.Core.Logic.Players
{
    public interface IInputHandler
    {
        public void StartInput();
        void UpdateInput();
        public void StopInput();
    }
}