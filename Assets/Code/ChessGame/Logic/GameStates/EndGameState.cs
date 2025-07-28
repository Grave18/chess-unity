using ChessGame.Logic.MovesBuffer;

namespace ChessGame.Logic.GameStates
{
    public class EndGameState : GameState
    {
        public override string Name => "End Game";

        public EndGameState(Game game) : base(game)
        {

        }

        public override void Enter()
        {
            Game.FireEnd();
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
            if (Game.UciBuffer.CanUndo(out MoveData moveData))
            {
                Game.Machine.SetState(new UndoState(Game, moveData), isSetPreviousState:false);
            }
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