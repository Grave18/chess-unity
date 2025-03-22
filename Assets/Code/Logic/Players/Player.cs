using System.Threading.Tasks;
using ChessBoard.Info;

namespace Logic.Players
{
    public abstract class Player
    {
        protected Game Game { get; private set; }

        protected Player(Game game)
        {
            Game = game;
        }

        public virtual void Update()
        {

        }

        public virtual void AllowMakeMove()
        {

        }

        public virtual void DisallowMakeMove()
        {

        }

        public virtual Task<PieceType> RequestPromotedPiece()
        {
            return Task.FromResult(PieceType.None);
        }

        public virtual void SelectPromotedPiece(PieceType pieceType)
        {

        }
    }
}