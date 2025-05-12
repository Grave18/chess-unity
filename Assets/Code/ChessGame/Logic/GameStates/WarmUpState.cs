using MainCamera;

namespace ChessGame.Logic.GameStates
{
    public class WarmUpState : GameState
    {
        public override string Name => "WarmUp";

        public WarmUpState(Game game, CameraController cameraController, PieceColor color) : base(game)
        {

        }

        public override void Enter()
        {
            // Empty
        }

        public override void Exit(string nextState)
        {
            // Empty
        }

        public override void Move(string uci)
        {
            // Empty
        }

        public override void Undo()
        {
            // Empty
        }

        public override void Redo()
        {
            // Empty
        }

        public override void Play()
        {
            // Empty
        }

        public override void Pause()
        {
            // Empty
        }

        public override void Update()
        {
            // Empty
        }
    }
}