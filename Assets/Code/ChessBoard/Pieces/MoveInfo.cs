namespace ChessBoard.Pieces
{
#pragma warning disable CS0252, CS0253
    public class MoveInfo
    {
        public bool Is2SquaresPawnMove { get; }

        public MoveInfo(bool is2SquaresPawnMove = false)
        {
            Is2SquaresPawnMove = is2SquaresPawnMove;
        }
    }
}