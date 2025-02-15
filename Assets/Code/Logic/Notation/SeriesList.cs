using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessBoard;
using ChessBoard.Pieces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Logic.Notation
{
    public class SeriesList : MonoBehaviour
    {
        [FormerlySerializedAs("gameManager")]
        [SerializeField] private Game game;
        [SerializeField] private List<Series> serieses;

        private Series _currentSeries;

        public void AddTurn(Piece piece, Square square, PieceColor turn, NotationTurnType notationTurnType,
            CheckType checkType, Piece promotedPiece = null)
        {
            // Add new series
            if (_currentSeries == null)
            {
                _currentSeries = new Series {Count = serieses.Count + 1};
                serieses.Add(_currentSeries);
            }

            var turnString = new StringBuilder();

            // Castling
            if(notationTurnType == NotationTurnType.CastlingShort)
            {
                turnString.Append("0-0");
                return;
            }

            if (notationTurnType == NotationTurnType.CastlingLong)
            {
                turnString.Append("0-0-0");
                return;
            }

            // Piece name
            bool isCaptureOrEnPassant = notationTurnType is NotationTurnType.Capture or NotationTurnType.EnPassant;
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
                        turnString.Append(piece.GetSquare().File);
                    }
                    break;
            }

            // Is capturing
            if (isCaptureOrEnPassant)
            {
                turnString.Append("x");
            }

            // Square name
            turnString.Append(square.AlgebraicName);

            // Is En Passant
            if (notationTurnType == NotationTurnType.EnPassant)
            {
                turnString.Append(" <i>e.p.</i>"); // italic
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
            switch (checkType)
            {
                case CheckType.Check:       turnString.Append("+");  break;
                case CheckType.DoubleCheck: turnString.Append("++"); break;
                case CheckType.CheckMate:   turnString.Append("#");  break;
            }

            AddTurn(turn, turnString);
        }

        public void RemoveTurn(PieceColor turn)
        {
            if (turn == PieceColor.White)
            {
                if(serieses.Count == 0) return;
                serieses.Remove(serieses.Last());
                _currentSeries = null;
            }
            else if (turn == PieceColor.Black)
            {
                if(serieses.Count == 0) return;
                serieses.Last().BlackMove = string.Empty;
                _currentSeries = serieses.Last();
            }
        }

        public string GetSeriesText()
        {
            var stringBuilder = new StringBuilder();

            foreach (Series series in serieses)
            {
                stringBuilder.AppendLine(series.ToString());
            }

            if (game.IsEndgame())
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

        private void AddTurn(PieceColor turn, StringBuilder turnString)
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
    }
}
