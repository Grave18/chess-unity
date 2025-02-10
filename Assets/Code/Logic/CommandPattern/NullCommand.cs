using System.Threading.Tasks;

namespace Logic.CommandPattern
{
    public class NullCommand : Command
    {
        public override async Task ExecuteAsync() => await Task.Delay(0);
        public override async Task UndoAsync() => await Task.Delay(0);
    }
}
