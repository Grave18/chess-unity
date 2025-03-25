using System.Collections.Generic;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.MovesBuffer;
using Logic.Players;
using Logic.Players.GameStates;
using UnityEngine;
using UnityEngine.Events;

namespace Logic
{
    public class Game : MonoBehaviour
    {
        [field: Header("References")]
        [field:SerializeField] public Competitors Competitors { get; set; }
        public Board Board { get; private set; }
        public UciBuffer UciBuffer { get; private set; }

        public PieceColor CurrentTurnColor { get; private set; } = PieceColor.White;
        public CheckType CheckType { get; set; } = CheckType.None;

        public ISelectable Selected { get; private set; }

        public AttackLinesList AttackLines { get; } = new();
        public HashSet<Square> UnderAttackSquares { get; set; } = new();

        public event UnityAction OnStart;
        public event UnityAction OnEnd;
        public event UnityAction OnChangeTurn;
        public event UnityAction OnPlay;
        public event UnityAction OnPause;

        public void FirePlay() => OnPlay?.Invoke();
        public void FirePause() => OnPause?.Invoke();
        public void FireStart() => OnStart?.Invoke();
        public void FireEnd() => OnEnd?.Invoke();
        private void FireChangeTurn() => OnChangeTurn?.Invoke();

        private GameState _state;
        private GameState _previousState;
        private PieceColor _timeOutColor = PieceColor.None;

        // Getters
        public HashSet<Piece> WhitePieces => Board.WhitePieces;
        public HashSet<Piece> BlackPieces => Board.BlackPieces;
        public HashSet<Piece> CurrentTurnPieces => CurrentTurnColor == PieceColor.White ? Board.WhitePieces : Board.BlackPieces;
        public HashSet<Piece> PrevTurnPieces => CurrentTurnColor == PieceColor.Black ? Board.WhitePieces : Board.BlackPieces;
        public IEnumerable<Square> Squares => Board.Squares;
        public Square NullSquare => Board.NullSquare;

        private PieceColor _startingColor;
        public void Init(Board board, UciBuffer commandUciBuffer,PieceColor turnColor)
        {
            Board = board;
            UciBuffer = commandUciBuffer;
            _startingColor = turnColor;
        }

        public void StartGame()
        {
            CheckType = CheckType.None;
            CurrentTurnColor = _startingColor;
            Board.Build();
            SetState(new IdleState(this));
            FireStart();
        }

        public void SetState(GameState state)
        {
            _previousState = _state;
            _state?.Exit();
            _state = state;
            _state?.Enter();
        }

        public void SetPreviousState()
        {
            if (_previousState != null)
            {
                SetState(_previousState);
                _previousState = null;
            }
            else
            {
                Debug.Log("No previous Game State found");
            }
        }

        public void ChangeTurn()
        {
            CurrentTurnColor = CurrentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            Competitors.ChangeCurrentPlayer();
            FireChangeTurn();
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

        public PieceColor GetWinner()
        {
            if (CheckType == CheckType.CheckMate)
            {
                return CurrentTurnColor ==  PieceColor.White ? PieceColor.Black : PieceColor.White;
            }

            if (CheckType == CheckType.TimeOutWhite)
            {
                return PieceColor.Black;
            }

            if (CheckType == CheckType.TimeOutBlack)
            {
                return PieceColor.White;
            }

            if (CheckType == CheckType.Stalemate)
            {
                return PieceColor.None;
            }

            return PieceColor.None;
        }

        public void SetTimeOut(PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.White)
            {
                CheckType = CheckType.TimeOutWhite;
            }
            else if (pieceColor == PieceColor.Black)
            {
                CheckType = CheckType.TimeOutBlack;
            }
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
}