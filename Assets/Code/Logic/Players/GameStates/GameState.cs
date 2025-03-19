using System.Collections.Generic;
using System.Linq;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using UnityEngine;
using Utils.Mathematics;

namespace Logic.Players.GameStates
{
    public abstract class GameState
    {
        public abstract string Name { get; }
        protected Game Game { get; private set; }

        protected GameState(Game game)
        {
            Game = game;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Move(string uci);
        public abstract void Undo();
        public abstract void Redo();
        public abstract void Play();
        public abstract void Pause();
        public abstract void Update();
    }

    public class Idle : GameState
    {
        public Idle(Game game) : base(game)
        {
        }

        public override string Name => "Idle";

        public override void Enter()
        {
            CalculateEndMove();
        }

        public override void Exit()
        {

        }

        public override void Move(string uci)
        {
            Game.SetState(new MoveS(Game, uci));
        }

        public override void Undo()
        {

        }

        public override void Redo()
        {

        }

        public override void Play()
        {

        }

        public override void Pause()
        {

        }

        public override void Update()
        {
            Game.Competitors.UpdatePlayer();
        }

        /// Calculations for all turns
        private void CalculateEndMove()
        {
            Game.UnderAttackSquares = GetUnderAttackSquares(Game.PrevTurnPieces);
            Game.CheckType = CalculateCheck(Game.PrevTurnPieces);

            foreach (Piece piece in Game.CurrentTurnPieces)
            {
                piece.CalculateMovesAndCaptures();
                piece.CalculateConstrains();
            }

            CalculateCheckMateOrStalemate(Game.CurrentTurnPieces);
        }

        private static HashSet<Square> GetUnderAttackSquares(HashSet<Piece> pieces)
        {
            var underAttackSquares = new HashSet<Square>();
            foreach (Piece piece in pieces)
            {
                piece.CalculateMovesAndCaptures();
                FillUnderAttackSquaresForPiece(piece, underAttackSquares);
            }

            return underAttackSquares;
        }

        private static void FillUnderAttackSquaresForPiece(Piece piece, HashSet<Square> underAttackSquares)
        {
            // Pawn's under attack
            if (piece is Pawn pawn)
            {
                var attackSquares = new List<Square>(pawn.UnderAttackSquares);
                foreach (Square underAttackSquare in attackSquares)
                {
                    underAttackSquares.Add(underAttackSquare);
                }
            }
            // Other piece's under attack
            else
            {
                var moveSquares = new List<Square>(piece.MoveSquares.Keys);
                foreach (Square moveSquare in moveSquares)
                {
                    underAttackSquares.Add(moveSquare);
                }
            }

            // All pieces defends
            foreach (Square defendSquare in piece.DefendSquares)
            {
                underAttackSquares.Add(defendSquare);
            }
        }

        private CheckType CalculateCheck(HashSet<Piece> pieces)
        {
            Game.AttackLines.Clear();

            foreach (Piece piece in pieces)
            {
                // Fill under attack line
                bool isCheck = IsPieceMakeCheck(piece);
                if (piece is LongRange longRange)
                {
                    if (!longRange.HasAttackLine) continue;

                    var attackLine = new AttackLine(piece, isCheck, longRange.AttackLineSquares, longRange.SquareBehindKing);
                    Game.AttackLines.Add(attackLine);
                }
                else
                {
                    if (!isCheck) continue;

                    var attackLine = new AttackLine(piece, true);
                    Game.AttackLines.Add(attackLine);
                }
            }

            return Game.AttackLines.GetCheckCount() switch
            {
                0 => CheckType.None,
                1 => CheckType.Check,
                _ => CheckType.DoubleCheck
            };

            bool IsPieceMakeCheck(Piece piece)
            {
                foreach ((Square square, _) in piece.CaptureSquares)
                {
                    if (square.HasPiece() && square.GetPiece() is King king &&
                        king.GetPieceColor() != piece.GetPieceColor())
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void CalculateCheckMateOrStalemate(HashSet<Piece> currentTurnPieces)
        {
            // If all pieces have no moves
            if (currentTurnPieces.Any(piece => piece.MoveSquares.Count  > 0
                || piece.CaptureSquares.Count > 0))
            {
                return;
            }

            Game.CheckType = Game.CheckType switch
                {
                    CheckType.None => CheckType.Stalemate,
                    CheckType.Check or CheckType.DoubleCheck => CheckType.CheckMate,
                    _ => Game.CheckType,
                };
        }
    }

    public class MoveS : GameState
    {
        private readonly string _uci;
        private bool _isRunning;
        private ParsedUci _parsedUci;
        private MoveType _moveType;

        public MoveS(Game game, string uci) : base(game)
        {
            _uci = uci;
        }

        public override string Name => "Move";

        public override void Enter()
        {
            ParsedUci parsedUci = GetParsedUci(_uci);
            bool isValid = Validate(parsedUci, out MoveType moveType);

            if (isValid)
            {
                Run(parsedUci, moveType);
            }
            else
            {
                Return();
            }
        }

        private ParsedUci GetParsedUci(string uci)
        {
            // Extract move form string
            string from = uci.Substring(0, 2);
            string to = uci.Substring(2, 2);

            Square fromSquare = Game.Board.GetSquare(from);
            Square toSquare = Game.Board.GetSquare(to);
            Piece piece = fromSquare.GetPiece();
            Piece promotedPiece = null;

            if(uci.Length == 5)
            {
                string promotion = uci.Substring(4, 1);
                promotedPiece = Game.Board.GetPiece(promotion, Game.CurrentTurnColor, to);
            }

            var parsedUci = new ParsedUci
            {
                Piece = piece,
                FromSquare = fromSquare,
                ToSquare = toSquare,
                PromotedPiece = promotedPiece,
            };

            return parsedUci;
        }

        private static bool Validate(ParsedUci parsedUci, out MoveType moveType)
        {
            moveType = MoveType.None;
            Piece piece = parsedUci.FromSquare.GetPiece();

            // Move
            if (piece.CanMoveTo(parsedUci.ToSquare, out MoveInfo moveInfo))
            {
                moveType = MoveType.Move;
                // _ = CommandInvoker.MoveTo(piece, moveToSquare, moveInfo);
                return true;
            }

            // Castling
            if (piece is King king && king.CanCastlingAt(parsedUci.ToSquare, out CastlingInfo castlingInfo))
            {
                moveType = MoveType.Castling;
                // _ = CommandInvoker.Castling(king, moveToSquare, castlingInfo.Rook, castlingInfo.RookSquare, castlingInfo.NotationTurnType);
                return true;
            }

            // Eat
            if (piece.CanEatAt(parsedUci.ToSquare, out CaptureInfo captureInfo))
            {
                moveType = MoveType.Capture;
                // _ = CommandInvoker.EatAt(piece, moveToSquare, captureInfo);
                return true;
            }

            return false;
        }

        private void Run(ParsedUci parsedUci, MoveType moveType)
        {
            _parsedUci = parsedUci;
            _moveType = moveType;
            _isRunning = true;
        }

        private void Return()
        {
            // Todo: maybe use stack of states
            Debug.Log("Invalid move");
            Game.SetState(new Idle(Game));
        }

        public override void Exit()
        {

        }

        public override void Move(string uci)
        {

        }

        public override void Undo()
        {

        }

        public override void Redo()
        {

        }

        public override void Play()
        {

        }

        public override void Pause()
        {

        }

        private float _t;
        private const float MoveDuration = 0.3f;
        public override void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            if (_t < 1)
            {
                ProgressMove();
            }
            else
            {
                EndMove();
            }
        }

        private void ProgressMove()
        {
            _t += Time.deltaTime/MoveDuration;

            Vector3 from = _parsedUci.FromSquare.transform.position;
            Vector3 to = _parsedUci.ToSquare.transform.position;
            Vector3 pos = Vector3.Lerp(from, to, Easing.InOutCubic(_t));
            _parsedUci.Piece.MoveTo(pos);
        }

        private void EndMove()
        {
            _parsedUci.Piece.ResetSquare(_parsedUci.ToSquare);
            _isRunning = false;
            AddMoveToBuffer();
            Game.SetState(new Idle(Game));
        }

        private void AddMoveToBuffer()
        {
            // Todo: Log move to buffer
        }
    }

    public enum MoveType
    {
        Move,
        Capture,
        Castling,
        MovePromotion,
        CapturePromotion,
        EnPassant,
        None
    }

    public struct ParsedUci
    {
        public Piece Piece;
        public Square FromSquare;
        public Square ToSquare;
        public Piece PromotedPiece;
    }
}