namespace Ui.Menu.Auxiliary
{
    public class FullscreenModeUi
    {
        public string Mode { get; }
        public int Index {get; }

        public FullscreenModeUi(string mode, int index)
        {
            Mode = mode;
            Index = index;
        }

        public override string ToString()
        {
            return Mode;
        }
    }
}