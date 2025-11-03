namespace Chess3D.Runtime.Core.Logic
{
    public enum CheckType
    {
        // Not end game
        None,
        Check,
        DoubleCheck,
        // End game
        Checkmate,
        Draw,
        TimeOut,
        Resign,
    }
}