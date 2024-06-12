using System.Collections.Generic;
using System.Linq;
using System.Text;
using Board;
using Board.Pieces;
using UnityEngine;

// square.AlgebraicName = $"{(char)(x + 'A')}{y + 1}";

public enum TurnType
{
    Move, Capture, Castling, EnPassant, Check, Mate
}

namespace Logic
{
    public class SeriesList : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private List<Series> serieses;
        private Series _currentSeries;

        public void AddTurn(Piece piece, Square square, PieceColor turn, TurnType turnType)
        {
            if (_currentSeries == null)
            {
                _currentSeries = new Series {Count = serieses.Count + 1};
                serieses.Add(_currentSeries);
            }

            var turnString = new StringBuilder();

            switch (piece)
            {
                case Bishop:
                    turnString.Append("B");

                    break;
                case King:
                    turnString.Append("K");

                    break;
                case Knight:
                    turnString.Append("N");

                    break;

                case Pawn:
                    if (turnType == TurnType.Capture)
                    {
                        turnString.Append(piece.GetSquare().File);
                    }

                    break;
                case Queen:
                    turnString.Append("Q");

                    break;
                case Rook:
                    turnString.Append("R");

                    break;
            }

            if (turnType == TurnType.Capture)
            {
                turnString.Append("x");
            }

            turnString.Append(square.AlgebraicName);

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

        public void RemoveTurn(PieceColor turn)
        {
            if (turn == PieceColor.White)
            {
                serieses.Remove(serieses.Last());
                _currentSeries = null;
            }
            else if (turn == PieceColor.Black)
            {
                serieses.Last().BlackMove = string.Empty;
                _currentSeries = serieses.Last();
            }
        }

        public string GetSeriesText()
        {
            var stringBuilder = new StringBuilder();

            foreach (var series in serieses)
            {
                stringBuilder.AppendLine(series.ToString());
            }

            return stringBuilder.ToString();
        }
    }

    [System.Serializable]
    public class Series
    {
        public int Count;
        public string WhiteMove;
        public string BlackMove;

        public override string ToString()
        {
            return $"{Count}. {WhiteMove} {BlackMove}";
        }
    }
}
