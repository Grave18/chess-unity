// using ChessBoard;
// using ChessBoard.Info;
// using ChessBoard.Pieces;
// using Logic.Moves;
// using Logic.MovesBuffer;
// using UnityEngine;
//
// namespace Logic.GameStates
// {
//     public class RedoState : MovableState
//     {
//         private const float MoveTimeSec = 0.1f;
//
//         private readonly MoveData _moveData;
//         private bool _isRunning;
//
//         public override string Name => "Redo";
//         protected override float T { get; set; } = 0f;
//         protected override float SoundT => 0.6f;
//
//         public RedoState(Game game, MoveData moveData) : base(game)
//         {
//             _moveData = moveData;
//         }
//
//
//         public override void Enter()
//         {
//             ParsedUci parsedUci = GetParsedUci(_moveData.Uci);
//             bool isValid = ValidateRedo(parsedUci);
//
//             if (isValid)
//             {
//                 Run();
//             }
//             else
//             {
//                 Abort();
//             }
//         }
//
//         private ParsedUci GetParsedUci(string uci)
//         {
//             // Extract move form string
//             string from = uci.Substring(0, 2);
//             string to = uci.Substring(2, 2);
//
//             Square fromSquare = Game.Board.GetSquare(from);
//             Square toSquare = Game.Board.GetSquare(to);
//             var promotedPieceType = PieceType.None;
//
//             if (uci.Length == 5)
//             {
//                 string promotion = uci.Substring(4, 1);
//                 promotedPieceType = Board.GetPieceType(promotion);
//             }
//
//             var parsedUci = new ParsedUci
//             {
//                 FromSquare = fromSquare,
//                 ToSquare = toSquare,
//                 PromotedPieceType = promotedPieceType,
//             };
//
//             return parsedUci;
//         }
//
//         private bool ValidateRedo(ParsedUci parsedUci)
//         {
//             Piece piece = parsedUci.FromSquare.GetPiece();
//
//             if (_moveData.MoveType == MoveType.Move)
//             {
//                 Turn = new SimpleMove(Game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
//                 return true;
//             }
//
//             if (_moveData.MoveType == MoveType.MovePromotion)
//             {
//                 Piece promotedPiece = Game.Board.GetSpawnedPiece(parsedUci.PromotedPieceType, Game.CurrentTurnColor,
//                     parsedUci.ToSquare);
//                 Turn = new MovePromotion(Game, piece, parsedUci.FromSquare, parsedUci.ToSquare, promotedPiece);
//                 return true;
//             }
//
//             if (_moveData.MoveType is MoveType.Capture)
//             {
//                 _moveData.BeatenPiece = parsedUci.ToSquare.GetPiece();
//                 Turn = new Capture(Game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
//                     _moveData.IsFirstMove);
//                 return true;
//             }
//
//             if (_moveData.MoveType is MoveType.EnPassant)
//             {
//                 // No need to update _moveData.BeatenPiece because it cannot be promoted piece
//                 Turn = new Capture(Game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
//                     _moveData.IsFirstMove);
//                 return true;
//             }
//
//             if (_moveData.MoveType is MoveType.CapturePromotion)
//             {
//                 // Order matters. Must grab captured piece first
//                 _moveData.BeatenPiece = parsedUci.ToSquare.GetPiece();
//                 Piece promotedPiece = Game.Board.GetSpawnedPiece(parsedUci.PromotedPieceType, Game.CurrentTurnColor,
//                     parsedUci.ToSquare);
//                 Turn = new CapturePromotion(Game, piece, parsedUci.FromSquare, parsedUci.ToSquare,
//                     promotedPiece, _moveData.BeatenPiece);
//                 return true;
//             }
//
//             if (_moveData.MoveType is MoveType.CastlingShort or MoveType.CastlingLong)
//             {
//                 Turn = new Castling(Game, _moveData.CastlingInfo, _moveData.IsFirstMove);
//                 return true;
//             }
//
//             return false;
//         }
//
//         private void Run()
//         {
//             _isRunning = true;
//         }
//
//         private void Abort()
//         {
//             Debug.LogError("Invalid Redo");
//             Game.GameStateMachine.SetPreviousState();
//         }
//
//         public override void Exit()
//         {
//             // Empty
//         }
//
//         public override void Move(string uci)
//         {
//             // Can't move
//         }
//
//         public override void Undo()
//         {
//             // No need
//         }
//
//         public override void Redo()
//         {
//             // Already Redo
//         }
//
//         public override void Play()
//         {
//             // Not Paused
//         }
//
//         public override void Pause()
//         {
//             // May be implemented, but with Undo and Redo
//         }
//
//         public override void Update()
//         {
//             if (!_isRunning)
//             {
//                 return;
//             }
//
//             if (IsProgressMove())
//             {
//                 ProgressMove();
//             }
//             else
//             {
//                 EndMove();
//             }
//         }
//
//         private bool IsProgressMove()
//         {
//             return T < 1;
//         }
//
//         private void ProgressMove()
//         {
//             float delta = Time.deltaTime / MoveTimeSec;
//             T += delta;
//
//             Turn.Progress(T);
//             PlaySound(delta);
//         }
//
//         private void EndMove()
//         {
//             Turn.End();
//
//             Game.ChangeTurn();
//             Game.UciBuffer.Redo();
//             Game.PreformCalculations();
//             Game.FireEndMove();
//             Game.GameStateMachine.SetPreviousState();
//
//             PlayCheckSound();
//         }
//     }
// }