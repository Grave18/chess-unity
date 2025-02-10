using System.Threading.Tasks;

namespace Logic.CommandPattern
{
    public abstract class Command
    {
        public abstract Task ExecuteAsync();
        public abstract Task UndoAsync();
    }
}
