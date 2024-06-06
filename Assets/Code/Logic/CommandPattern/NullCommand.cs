namespace Logic.CommandPattern
{
    public class NullCommand : Command
    {
        public override void Execute() { }
        public override void Undo() { }
    }
}
