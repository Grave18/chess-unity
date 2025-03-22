using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessBoard;
using ChessBoard.Pieces;
using Logic;
using Logic.MovesBuffer;
using UnityEngine;

namespace AlgebraicNotation
{
    public class SeriesList : MonoBehaviour
    {
        [SerializeField] private Game game;
        [SerializeField] private List<Series> serieses;

        private Series _currentSeries;
        private int _commandCount;

        private void OnEndTurn()
        {
            // Todo: add code for run notation
            // Command lastCommand = commandInvoker.GetLastCommand();
            //
            // if (_commandCount < commandInvoker.GetCommandsCount())
            // {
            //     if(IsNotValidTurn(lastCommand)) return;
            //     AddRecord(lastCommand.Piece, lastCommand.MoveFromSquare, lastCommand.MoveToSquare, lastCommand.MoveType, lastCommand.PromotedPiece);
            // }
            // else
            // {
            //     RemoveRecord(IsNotValidTurn(lastCommand) ? PieceColor.Black : lastCommand.Piece.GetPieceColor());
            // }
            //
            // _commandCount = commandInvoker.GetCommandsCount();
        }

        /// Add new record to algebraic notation
        private void AddRecord(Piece piece, Square fromSquare, Square toSquare, MoveType moveType, Piece promotedPiece)
        {
            // Add new series
            if (_currentSeries == null)
            {
                _currentSeries = new Series {Count = serieses.Count + 1};
                serieses.Add(_currentSeries);
            }

            var turnString = new StringBuilder();

            // Castling
            if(moveType == MoveType.CastlingShort)
            {
                turnString.Append("0-0");
                AddRecordToSeries(piece.GetPieceColor(), turnString);
                return;
            }

            if (moveType == MoveType.CastlingLong)
            {
                turnString.Append("0-0-0");
                AddRecordToSeries(piece.GetPieceColor(), turnString);
                return;
            }

            // Piece name
            bool isCaptureOrEnPassant = moveType is MoveType.Capture or MoveType.EnPassant;
            switch (piece)
            {
                case King:   turnString.Append("K"); break;
                case Queen:  turnString.Append("Q"); break;
                case Rook:   turnString.Append("R"); break;
                case Bishop: turnString.Append("B"); break;
                case Knight: turnString.Append("N"); break;
                case Pawn:
                    if (isCaptureOrEnPassant)
                    {
                        turnString.Append(fromSquare.File);
                    }
                    break;
            }

            // Is capturing
            if (isCaptureOrEnPassant)
            {
                turnString.Append("x");
            }

            // Square name
            turnString.Append(toSquare.Address);

            // Is En Passant
            if (moveType == MoveType.EnPassant)
            {
                turnString.Append(" e.p.");
            }
            // Is promotion
            else if (promotedPiece != null)
            {
                switch (promotedPiece)
                {
                    case Queen:  turnString.Append("=Q"); break;
                    case Rook:   turnString.Append("=R"); break;
                    case Bishop: turnString.Append("=B"); break;
                    case Knight: turnString.Append("=N"); break;
                }
            }

            // Is check
            switch (game.CheckType)
            {
                case CheckType.Check:       turnString.Append("+");  break;
                case CheckType.DoubleCheck: turnString.Append("++"); break;
                case CheckType.CheckMate:   turnString.Append("#");  break;
            }

            AddRecordToSeries(piece.GetPieceColor(), turnString);
        }

        private void AddRecordToSeries(PieceColor turn, StringBuilder turnString)
        {
            if (turn == PieceColor.White)
            {
                _currentSeries.WhiteMove = turnString.ToString();
                Debug.Log(_currentSeries);
            }
            else if (turn == PieceColor.Black)
            {
                _currentSeries.BlackMove = turnString.ToString();
                Debug.Log(_currentSeries);
                _currentSeries = null;
            }
        }

        public void RemoveRecord(PieceColor turnColor)
        {
            if (turnColor == PieceColor.Black)
            {
                if(serieses.Count == 0) return;
                serieses.Remove(serieses.Last());
                _currentSeries = null;
            }
            else if (turnColor == PieceColor.White)
            {
                if(serieses.Count == 0) return;
                serieses.Last().BlackMove = string.Empty;
                _currentSeries = serieses.Last();
            }
        }

        public string GetSeriesText()
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < serieses.Count; i++)
            {
                string value = serieses[i].ToString();
                if (i == serieses.Count - 1) value =$"<u>{value}</u>";

                stringBuilder.AppendLine(value);
            }

            if (game.IsGameOver())
            {
                string endGameText = game.GetWinner() switch
                {
                    PieceColor.White => "1-0",
                    PieceColor.Black => "0-1",
                    PieceColor.None => "\u00bd-\u00bd", // ½-½
                    _ => string.Empty,
                };
                stringBuilder.AppendLine(endGameText);
            }

            return stringBuilder.ToString();
        }

        public void Clear()
        {
            serieses.Clear();
        }

        private void OnEnable()
        {
            // game.OnEndTurn += OnEndTurn;
        }

        private void OnDisable()
        {
            // game.OnEndTurn -= OnEndTurn;
        }
    }
}
