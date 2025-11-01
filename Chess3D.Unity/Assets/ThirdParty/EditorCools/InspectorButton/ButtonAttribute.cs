namespace EditorCools
{
    using System;

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ButtonAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Row;
        public readonly float Space;
        public readonly bool HasRow;
        public readonly object[] Args;
        
        public ButtonAttribute(string name = default, string row = default, float space = default, object[] args = default)
        {
            Row = row;
            HasRow = !string.IsNullOrEmpty(Row);
            Name = name;
            Space = space;
            Args = args;
        }
    }
}

