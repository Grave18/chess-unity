namespace Logic.Players
{
    public interface IInputHandler
    {
        public void StartInput();
        void UpdateInput();
        public void StopInput();
    }
}