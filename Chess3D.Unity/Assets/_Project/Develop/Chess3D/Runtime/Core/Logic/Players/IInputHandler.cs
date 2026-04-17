using Cysharp.Threading.Tasks;

namespace Chess3D.Runtime.Core.Logic.Players
{
    public interface IInputHandler
    {
        public UniTask Load();
        public void StartInput();
        void UpdateInput();
        public void StopInput();
    }
}