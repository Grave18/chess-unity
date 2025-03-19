using System.Collections.Generic;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic.Players;
using Logic.Players.GameStates;
using UnityEngine;

namespace Logic
{
    public class Game : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CommandInvoker commandInvoker;
        [field:SerializeField] public Competitors Competitors { get; set; }
        public Board Board { get; private set; }

        [field: Header("Debug")]
        [field: SerializeField] public PieceColor CurrentTurnColor { get; set; } = PieceColor.White;
        [field: SerializeField] public CheckType CheckType { get; set; } = CheckType.None;
        [field: SerializeField] public MoveTypeLegacy MoveType { get; set; } = MoveTypeLegacy.None;

        public ISelectable Selected { get; set; }

        public AttackLinesList AttackLines { get; } = new();
        public HashSet<Square> UnderAttackSquares { get; set; } = new();

        // public event Action OnEndTurn;
        // public event Action OnStart;
        // public event Action OnEnd;
        // public event Action OnPlay;
        // public event Action OnPause;

        private GameState _state;
        private PieceColor _timeOutColor = PieceColor.None;

        // Getters
        public HashSet<Piece> WhitePieces => Board.WhitePieces;
        public HashSet<Piece> BlackPieces => Board.BlackPieces;
        public IEnumerable<Square> Squares => Board.Squares;
        public Square NullSquare => Board.NullSquare;

        public HashSet<Piece> CurrentTurnPieces => CurrentTurnColor == PieceColor.White ? Board.WhitePieces : Board.BlackPieces;
        public HashSet<Piece> PrevTurnPieces => CurrentTurnColor == PieceColor.Black ? Board.WhitePieces : Board.BlackPieces;

        public void Init(Board board, PieceColor turnColor)
        {
            Board = board;
            CurrentTurnColor = turnColor;
        }

        public void StartGame()
        {
            MoveType = MoveTypeLegacy.None;
            CheckType = CheckType.None;
            Board.Build();
            SetState(new Idle(this));
        }

        public void SetState(GameState state)
        {
            _state?.Exit();
            _state = state;
            _state?.Enter();
        }

        private void Update()
        {
            _state?.Update();
        }

        public void Move(string uci)
        {
            _state?.Move(uci);
        }

        public void Undo()
        {
            _state?.Undo();
        }

        public void Redo()
        {
            _state?.Redo();
        }

        public void Play()
        {
            _state?.Play();
        }

        public void Pause()
        {
            _state?.Pause();
        }

        public void StartTurn(MoveTypeLegacy moveType = MoveTypeLegacy.Move)
        {
            // MoveType = moveType;
            // _state = GameState.Move;
            // OnStartTurn?.Invoke();
        }
        public void EndTurn()
        {
            // currentTurnColor = currentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            // _state = GameState.Idle;
            //
            // CalculateEndMove();
            // OnEndTurn?.Invoke();
            //
            // if(IsGameOver())
            // {
            //     OnEnd?.Invoke();
            // }
        }

        public void StartThink()
        {
            // _state = GameState.Think;
            // OnStartThink?.Invoke();
        }

        public void EndThink()
        {
            // _state = GameState.Idle;
            // OnEndThink?.Invoke();
        }

        public PieceColor GetWinner()
        {
            if (CheckType == CheckType.CheckMate)
            {
                return CurrentTurnColor ==  PieceColor.White ? PieceColor.Black : PieceColor.White;
            }

            if (CheckType == CheckType.TimeOut)
            {
                return _timeOutColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            }

            if (CheckType == CheckType.Stalemate)
            {
                return PieceColor.None;
            }

            return PieceColor.None;
        }

        public void SetTimeOut(PieceColor pieceColor)
        {
            CheckType = CheckType.TimeOut;
            _timeOutColor = pieceColor;
        }

        public bool IsGameOver()
        {
            return CheckType is CheckType.CheckMate or CheckType.Stalemate or CheckType.TimeOut;
        }

        /// Retrieves the En Passant information for the last command if applicable
        public EnPassantInfo GetEnPassantInfo()
        {
            // Todo: move to state?
            return commandInvoker.GetEnPassantInfo();
        }

        /// Get section relative to current piece color
        public Square GetSquareRel(PieceColor pieceColor, Square currentSquare, Vector2Int offset)
        {
            return Board.GetSquareRel(pieceColor, currentSquare, offset);
        }

        /// Get section relative to absolute position (white side)
        public Square GetSquareAbs(Square currentSquare, Vector2Int offset)
        {
            return Board.GetSquareAbs(currentSquare, offset);
        }

        public string GetStateName()
        {
            return _state?.Name;
        }

        public bool CanSelect(ISelectable selectable)
        {
            return selectable.HasPiece() && selectable.GetPieceColor() == CurrentTurnColor;
        }

        public void Select(ISelectable selectable)
        {
            Selected = selectable;
        }

        /// Deselect currently selected piece
        public void Deselect()
        {
            Selected = null;
        }
    }

    public enum MoveTypeLegacy
    {
        None,
        Move,
        Undo,
        Redo,
    }
}