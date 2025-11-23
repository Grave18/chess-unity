namespace Chess3D.Runtime.Utilities.Logging
{
    public static class Log
    {
        public static readonly TagLog Default = new(string.Empty);
        public static readonly TagLog Loading = new("LOADING");
        public static readonly TagLog Editor = new("EDITOR");
        public static readonly TagLog Boot = new("BOOT");
        public static readonly TagLog Menu = new("MENU");
        public static readonly TagLog Core = new("CORE");
        public static readonly TagLog AI = new("AI");

        public static readonly BuilderLogPool Builder = new(new TagLog(string.Empty), 5);
    }
}