using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.Sound;

namespace Chess3D.Runtime.Core.Logic.Moves
{
    public sealed class Castling : Turn
    {
        private readonly SimpleMove _kingMove;
        private readonly SimpleMove _rookMove;
        private readonly EffectsPlayer _effectsPlayer;

        public Castling(Game game, CastlingInfo castlingInfo, bool isFirstMove, EffectsPlayer effectsPlayer)
        {
            _kingMove = new SimpleMove(game, castlingInfo.King, castlingInfo.KingFromSquare, castlingInfo.KingToSquare, isFirstMove, effectsPlayer);
            _rookMove = new SimpleMove(game, castlingInfo.Rook, castlingInfo.RookFromSquare, castlingInfo.RookToSquare, isFirstMove, effectsPlayer);
            _effectsPlayer = effectsPlayer;
        }

        public override void Progress(float t)
        {
            if (t < 0.5f)
            {
                _kingMove.Progress(2*t);
            }
            else
            {
                _rookMove.Progress(2*t - 1);
            }
        }

        public override void End()
        {
            _kingMove.End();
            _rookMove.End();
        }

        public override void EndUndo()
        {
            _kingMove.EndUndo();
            _rookMove.EndUndo();
        }

        public override void PlaySound()
        {
            _effectsPlayer.PlayCastleSound();
        }
    }
}