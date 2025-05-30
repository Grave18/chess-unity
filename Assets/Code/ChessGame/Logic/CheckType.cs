namespace ChessGame.Logic
{
    public enum CheckType
    {
        // Not end game
        None,
        Check,
        DoubleCheck,
        // End game
        CheckMate,
        Draw,
        Resign,
        TimeOutWhite,
        TimeOutBlack,
    }
}