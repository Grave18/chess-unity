﻿using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic.MovesBuffer;
using Logic.Players.Moves;
using UnityEngine;

namespace Logic.Players.GameStates
{
    public class UndoState : GameState
    {
        private readonly MoveData _moveData;
        private bool _isRunning;
        private Turn turn;

        private float _t = 1;
        private const float TimeSec = 0.1f;

        public UndoState(Game game, MoveData moveData) : base(game)
        {
            _moveData = moveData;
        }

        public override string Name => "Undo";

        public override void Enter()
        {
            ParsedUci parsedUci = GetParsedUci(_moveData.Uci);
            bool isValid = ValidateUndo(parsedUci);

            if (isValid)
            {
                Run();
            }
            else
            {
                Abort();
            }
        }

        private ParsedUci GetParsedUci(string uci)
        {
            // Extract move form string
            string from = uci.Substring(0, 2);
            string to = uci.Substring(2, 2);

            Square fromSquare = Game.Board.GetSquare(from);
            Square toSquare = Game.Board.GetSquare(to);
            var promotedPieceType = PieceType.None;

            if (uci.Length == 5)
            {
                string promotion = uci.Substring(4, 1);
                promotedPieceType = Board.GetPieceType(promotion);
            }

            var parsedUci = new ParsedUci
            {
                FromSquare = fromSquare,
                ToSquare = toSquare,
                PromotedPieceType = promotedPieceType,
            };

            return parsedUci;
        }

        private bool ValidateUndo(ParsedUci parsedUci)
        {
            Piece piece = parsedUci.ToSquare.GetPiece();

            if (_moveData.MoveType == MoveType.Move)
            {
                turn = new SimpleMove(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType == MoveType.MovePromotion)
            {
                // Piece and Promoted piece swapped
                Piece promotedPiece = parsedUci.ToSquare.GetPiece();
                turn = new MovePromotion(promotedPiece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.HiddenPawn);
                return true;
            }

            if (_moveData.MoveType is MoveType.Capture or MoveType.EnPassant)
            {
                turn = new Capture(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                    _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType is MoveType.CapturePromotion)
            {
                Piece promotedPiece = parsedUci.ToSquare.GetPiece();
                turn = new CapturePromotion(promotedPiece, parsedUci.FromSquare, parsedUci.ToSquare,
                    _moveData.HiddenPawn, _moveData.BeatenPiece);
                return true;
            }

            if (_moveData.MoveType is MoveType.CastlingShort or MoveType.CastlingLong)
            {
                turn = new Castling(_moveData.CastlingInfo, _moveData.IsFirstMove);
                return true;
            }

            return false;
        }

        private void Run()
        {
            _isRunning = true;
        }

        private void Abort()
        {
            // Todo: maybe use stack of states
            Debug.Log("Invalid move");
            Game.SetState(new IdleState(Game));
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

        public override void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            if (_t > 0)
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
            _t -= Time.deltaTime / TimeSec;

            turn.Progress(_t);
        }

        private void EndMove()
        {
            turn.EndUndo();

            Game.CommandBuffer.Undo();
            Game.ChangeTurn();
            Game.SetState(new IdleState(Game));
        }
    }
}