using System.Collections.Generic;
using ChessBoard.Info;
using Logic;
using UnityEngine;

namespace Notation
{
    public class FenFromString
    {
        public PieceColor TurnColor { get; }
        public int HalfMoveClock { get; }
        public int FullMoveCounter { get; }
        public IList<PieceInfo> PieceInfos { get; }
        public string EnPassantAddress => _fenSplit.EnPassant;

        private readonly FenSplit _fenSplit;

        /// Fen string must be valid
        public FenFromString(string fenString)
        {
            string[] splitPreset = fenString.Split(' ');

            _fenSplit = new FenSplit
            {
                PiecesPreset = splitPreset[0],
                TurnColor = splitPreset[1],
                Castling = splitPreset[2],
                EnPassant = splitPreset[3],
                HalfMoveClock = splitPreset[4],
                FullMoveCounter = splitPreset[5],
            };

            TurnColor = GetTurnColorFromFenSplit(_fenSplit.TurnColor);
            HalfMoveClock = GetHalfMoveClock(_fenSplit.HalfMoveClock);
            FullMoveCounter = GetFullMoveCounter(_fenSplit.FullMoveCounter);
            PieceInfos = GetPiecesFromPreset();
        }

        public string GetShort()
        {
            return $"{_fenSplit.PiecesPreset} {_fenSplit.TurnColor} {_fenSplit.Castling} {_fenSplit.EnPassant}";
        }

        private static PieceColor GetTurnColorFromFenSplit(string turnColorStr)
        {
            PieceColor turnColor = turnColorStr switch
            {
                "w" => PieceColor.White,
                "b" => PieceColor.Black,
                _ => PieceColor.None
            };

            return turnColor;
        }

        private static int GetHalfMoveClock(string halfMoveClockStr)
        {
            return int.TryParse(halfMoveClockStr, out int halfMoveClock) ? halfMoveClock : 0;
        }

        private static int GetFullMoveCounter(string fullMoveCounterStr)
        {
            return int.TryParse(fullMoveCounterStr, out int fullMoveCounter) ? fullMoveCounter : 1;
        }

        private IList<PieceInfo> GetPiecesFromPreset()
        {
            var pieceInfos = new List<PieceInfo>();
            int squareNum = 0;
            foreach (char ch in _fenSplit.PiecesPreset)
            {
                PieceInfo pieceInfo;

                switch (ch)
                {
                    case '/':
                        continue;
                    case > '0' and <= '8':
                    {
                        int squaresToSkip = ch - '0';
                        squareNum += squaresToSkip;
                        continue;
                    }
                    // White
                    case 'B':
                        pieceInfo = new PieceInfo(PieceType.Bishop, PieceColor.White, squareNum);
                        break;
                    case 'K':
                    {
                        bool isFirstMove = CheckWhiteKingFirstMove();
                        pieceInfo = new PieceInfo(PieceType.King, PieceColor.White, squareNum, isFirstMove);
                    }
                        break;
                    case 'N':
                        pieceInfo = new PieceInfo(PieceType.Knight, PieceColor.White, squareNum);
                        break;
                    case 'P':
                    {
                        bool isFirstMove = CheckWhitePawnFirstMove(squareNum);
                        pieceInfo = new PieceInfo(PieceType.Pawn, PieceColor.White, squareNum, isFirstMove);
                    }
                        break;
                    case 'Q':
                        pieceInfo = new PieceInfo(PieceType.Queen, PieceColor.White, squareNum);
                        break;
                    case 'R':
                    {
                        bool isFirstMove = CheckWhiteRookFirstMove(squareNum);
                        pieceInfo = new PieceInfo(PieceType.Rook, PieceColor.White, squareNum, isFirstMove);
                    }
                        break;
                    // Black
                    case 'b':
                        pieceInfo = new PieceInfo(PieceType.Bishop, PieceColor.Black, squareNum);
                        break;
                    case 'k':
                    {
                        bool isFirstMove = CheckBlackKingFirstMove();
                        pieceInfo = new PieceInfo(PieceType.King, PieceColor.Black, squareNum, isFirstMove);
                    }
                        break;
                    case 'n':
                        pieceInfo = new PieceInfo(PieceType.Knight, PieceColor.Black, squareNum);
                        break;
                    case 'p':
                    {
                        bool isFirstMove = CheckBlackPawnFirstMove(squareNum);
                        pieceInfo = new PieceInfo(PieceType.Pawn, PieceColor.Black, squareNum, isFirstMove);
                    }
                        break;
                    case 'q':
                        pieceInfo = new PieceInfo(PieceType.Queen, PieceColor.Black, squareNum);
                        break;
                    case 'r':
                    {
                        bool isFirstMove = CheckBlackRookFirstMove(squareNum);
                        pieceInfo = new PieceInfo(PieceType.Rook, PieceColor.Black, squareNum, isFirstMove);
                    }
                        break;
                    default:
                        Debug.LogError($"{ch} is not a valid character");
                        return null;
                }

                pieceInfos.Add(pieceInfo);
                squareNum += 1;
            }

            return pieceInfos;
        }

        private static bool CheckWhitePawnFirstMove(int squareNum)
        {
            return squareNum / 8 == 6; // Rank is 2
        }

        private static bool CheckBlackPawnFirstMove(int squareNum)
        {
            return squareNum / 8 == 1; // Rank is 7
        }

        private bool CheckWhiteRookFirstMove(int squareNum)
        {
            return _fenSplit.Castling.Contains("K") && squareNum == 63 // H1
                   || _fenSplit.Castling.Contains("Q") && squareNum == 56; // A1
        }

        private bool CheckBlackRookFirstMove(int squareNum)
        {
            return _fenSplit.Castling.Contains("k") && squareNum == 7 // H8
                   || _fenSplit.Castling.Contains("q") && squareNum == 0; // A8
        }

        private bool CheckBlackKingFirstMove()
        {
            return _fenSplit.Castling.Contains("k")
                   || _fenSplit.Castling.Contains("q");
        }

        private bool CheckWhiteKingFirstMove()
        {
            return _fenSplit.Castling.Contains("K")
                   || _fenSplit.Castling.Contains("Q");
        }
    }
}