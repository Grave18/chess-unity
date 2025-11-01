namespace Chess3D.Runtime.Logic.Players
{
    public interface IInputHandler
    {
        public void StartInput();
        void UpdateInput();
        public void StopInput();
    }
}