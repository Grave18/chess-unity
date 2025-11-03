using System.Collections.Generic;
using System.Linq;

namespace Chess3D.Runtime.Core.Notation
{
    public static class FenValidator
    {
        private static bool _isValid;
        private static string _errorMessage = string.Empty;
        private static FenSplit _fenParsed;

        public static bool IsValid(string fen, out string errorMessage)
        {
            if (string.IsNullOrEmpty(fen))
            {
                errorMessage = "Fen cannot be empty";
                return false;
            }

            _isValid = true;
            _errorMessage = string.Empty;

            SplitFenWithValidation(fen);
            ValidatePiecesPreset();
            ValidateTurnColor();
            ValidateCastling();
            ValidateEnPassant();
            ValidateHalfMove();
            ValidateFullMove();

            errorMessage = _errorMessage;
            return _isValid;
        }

        private static void SplitFenWithValidation(string fen)
        {
            fen = fen.Trim();
            string[] splitPreset = fen.Split(' ');

            for (int i = 0; i < splitPreset.Length; i++)
            {
                if (splitPreset[i] == string.Empty)
                {
                    if (i - 1 < 0)
                    {
                        _errorMessage += "Duplicated spaces at the beginning\n";
                    }
                    else
                    {
                        _errorMessage += $"After part '{splitPreset[i - 1]}' may be duplicated spaces\n";
                    }

                    _isValid = false;
                }
            }

            _fenParsed = new FenSplit
            {
                PiecesPreset = splitPreset.Length > 0 ? splitPreset[0] : string.Empty,
                TurnColor =    splitPreset.Length > 1 ? splitPreset[1] : string.Empty,
                Castling =     splitPreset.Length > 2 ? splitPreset[2] : string.Empty,
                EnPassant =    splitPreset.Length > 3 ? splitPreset[3] : string.Empty,
                HalfMoveClock =     splitPreset.Length > 4 ? splitPreset[4] : string.Empty,
                FullMoveCounter =     splitPreset.Length > 5 ? splitPreset[5] : string.Empty
            };

            if (splitPreset.Length > 6)
            {
                _errorMessage += $"Part '{splitPreset[6]}' is more than expected parts count";
                _isValid = false;
            }
        }

        private static void ValidatePiecesPreset()
        {
            if (!_isValid)
            {
                return;
            }

            int rankCount = 1;
            int squaresInRankCount = 0;
            int whitePieceCount = 0;
            int blackPieceCount = 0;
            int whitePawnCount = 0;
            int blackPawnCount = 0;
            int whiteKingCount = 0;
            int blackKingCount = 0;
            int whiteQueenCount = 0;
            int blackQueenCount = 0;
            int whiteRookCount = 0;
            int blackRookCount = 0;
            int whiteBishopCount = 0;
            int blackBishopCount = 0;
            int whiteKnightCount = 0;
            int blackKnightCount = 0;

            foreach (char ch in _fenParsed.PiecesPreset)
            {
                switch (ch)
                {
                    case '/':
                        if (!IsSquareInRankCountRight(squaresInRankCount, rankCount))
                        {
                            _isValid = false;
                            return;
                        }

                        rankCount += 1;

                        if (rankCount > 8)
                        {
                            _errorMessage = $"Rank count {rankCount} is greater than 8";
                            _isValid = false;
                            return;
                        }

                        squaresInRankCount = 0;
                        break;
                    case > '0' and <= '8':
                        squaresInRankCount += int.Parse(ch.ToString());
                        break;
                    case 'P':
                        whitePieceCount += 1;
                        whitePawnCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'N':
                        whitePieceCount += 1;
                        whiteKnightCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'B':
                        whitePieceCount += 1;
                        whiteBishopCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'R':
                        whitePieceCount += 1;
                        whiteRookCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'Q':
                        whitePieceCount += 1;
                        whiteQueenCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'K':
                        whitePieceCount += 1;
                        whiteKingCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'p':
                        blackPieceCount += 1;
                        blackPawnCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'n':
                        blackPieceCount += 1;
                        blackKnightCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'b':
                        blackPieceCount += 1;
                        blackBishopCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'r':
                        blackPieceCount += 1;
                        blackRookCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'q':
                        blackPieceCount += 1;
                        blackQueenCount += 1;
                        squaresInRankCount += 1;
                        break;
                    case 'k':
                        blackPieceCount += 1;
                        blackKingCount += 1;
                        squaresInRankCount += 1;
                        break;
                    default:
                        _errorMessage = $"Invalid character '{ch}' in rank {rankCount}";
                        _isValid = false;
                        return;
                }
            }

            if (rankCount < 8)
            {
                _errorMessage = $"Rank count {rankCount} is less than 8";
                _isValid = false;
                return;
            }

            if (whitePieceCount > 16)
            {
                _errorMessage = $"White pieces count {whitePieceCount} is greater than 16";
                _isValid = false;
                return;
            }

            if (blackPieceCount > 16)
            {
                _errorMessage = $"Black pieces count {blackPieceCount} is greater than 16";
                _isValid = false;
                return;
            }

            if (whiteKingCount > 1)
            {
                _errorMessage = $"White kings count {whiteKingCount} is greater than 1";
                _isValid = false;
                return;
            }

            if (blackKingCount > 1)
            {
                _errorMessage = $"Black kings count {blackKingCount} is greater than 1";
                _isValid = false;
                return;
            }

            if (whiteQueenCount > 9)
            {
                _errorMessage = $"White queens count {whiteQueenCount} is greater than 9";
                _isValid = false;
                return;
            }

            if (whiteKingCount == 0)
            {
                _errorMessage = "No white king on board";
                _isValid = false;
                return;
            }

            if (blackKingCount == 0)
            {
                _errorMessage = "No black king on board";
                _isValid = false;
                return;
            }

            if (blackQueenCount > 9)
            {
                _errorMessage = $"Black queens count {blackQueenCount} is greater than 9";
                _isValid = false;
                return;
            }

            if (whiteRookCount > 10)
            {
                _errorMessage = $"White rooks count {whiteRookCount} is greater than 10";
                _isValid = false;
                return;
            }

            if (blackRookCount > 10)
            {
                _errorMessage = $"Black rooks count {blackRookCount} is greater than 10";
                _isValid = false;
                return;
            }

            if (whiteBishopCount > 10)
            {
                _errorMessage = $"White bishops count {whiteBishopCount} is greater than 10";
                _isValid = false;
                return;
            }

            if (blackBishopCount > 10)
            {
                _errorMessage = $"Black bishops count {blackBishopCount} is greater than 10";
                _isValid = false;
                return;
            }

            if (whiteKnightCount > 10)
            {
                _errorMessage = $"White knights count {whiteKnightCount} is greater than 10";
                _isValid = false;
                return;
            }

            if (blackKnightCount > 10)
            {
                _errorMessage = $"Black knights count {blackKnightCount} is greater than 10";
                _isValid = false;
                return;
            }

            if (whitePawnCount > 8)
            {
                _errorMessage = $"White pawns count {whitePawnCount} is greater than 8";
                _isValid = false;
                return;
            }

            if (blackPawnCount > 8)
            {
                _errorMessage = $"Black pawns count {blackPawnCount} is greater than 8";
                _isValid = false;
                return;
            }
        }

        private static bool IsSquareInRankCountRight(int squaresInRankCount, int rankCount)
        {
            if (squaresInRankCount > 8)
            {
                _errorMessage = $"Square in rank {rankCount} count is {squaresInRankCount} what is greater than 8";
                return false;
            }

            if (squaresInRankCount < 8)
            {
                _errorMessage = $"Square in rank {rankCount} count is {squaresInRankCount} what is less than 8";
                return false;
            }

            return true;
        }

        private static void ValidateTurnColor()
        {
            if (!_isValid)
            {
                return;
            }

            if (_fenParsed.TurnColor is not "w" and not "b")
            {
                _errorMessage = $"Turn color '{_fenParsed.TurnColor}' must be 'w' or 'b'";
                _isValid = false;
            }
        }

        private static void ValidateCastling()
        {
            if (!_isValid)
            {
                return;
            }

            string castling = _fenParsed.Castling;
            if (castling.Length is > 4 or < 1)
            {
                _errorMessage = $"Castling '{_fenParsed.Castling}' must be between 1 and 4 characters long";
                _isValid = false;
                return;
            }

            if (castling == "-")
            {
                return;
            }

            var indexedCastling = new Dictionary<char, int>
            {
                { 'K', 0 },
                { 'Q', 1 },
                { 'k', 2 },
                { 'q', 3 }
            };

            int prevIndex = -1;
            for (int i = 0; i < castling.Length; i++)
            {
                char castlingChar = castling[i];
                if (IsNotKQkq(castlingChar))
                {
                    _errorMessage = $"Castling symbol '{castlingChar}' is invalid. Must be: 'K', 'Q', 'k' or 'q'";
                    _isValid = false;
                    return;
                }

                if (indexedCastling[castlingChar] < prevIndex)
                {
                    _errorMessage = $"Castling '{castlingChar}' of index {i} must be in order 'KQkq'";
                    _isValid = false;
                    return;
                }
                if (indexedCastling[castlingChar] == prevIndex)
                {
                    _errorMessage = $"Castling {castlingChar} contains duplicate";
                    _isValid = false;
                    return;
                }

                prevIndex = indexedCastling[castlingChar];
            }
        }

        private static bool IsNotKQkq(char c)
        {
            return c is not 'K' and not 'Q' and not 'k' and not 'q';
        }

        private static void ValidateEnPassant()
        {
            if (!_isValid)
            {
                return;
            }

            string ep = _fenParsed.EnPassant;

            if (ep == "-")
            {
                return;
            }

            if (ep.Length != 2)
            {
                _errorMessage = $"En passant '{ep}' must be 2 characters long. First file second rank";
                _isValid = false;
                return;
            }

            int fileNum = FileToNumber(ep[0]);
            int rank = CharToOrdinalInt(ep[1]);
            int rankIndex = rank - 1;
            string[] ranks = _fenParsed.PiecesPreset.Split('/').Reverse().ToArray();

            if (rank == 3)
            {
                string rankWithPawnFen = ranks[rankIndex + 1];
                CheckEpPawnInPlace(rankWithPawnFen, fileNum, ep, 'P');
            }
            else if (rank == 6)
            {
                string rankWithPawnFen = ranks[rankIndex - 1];
                CheckEpPawnInPlace(rankWithPawnFen, fileNum, ep, 'p');
            }
            else
            {
                _errorMessage = $"En passant square '{ep}' must be in the 3rd or 6th rank square";
                _isValid = false;
            }
        }

        private static int FileToNumber(char ch)
        {
            return ch - 'a' + 1;
        }

        private static void CheckEpPawnInPlace(string rankFen, int file, string ep, char pawn)
        {
            int currentFile = 0;
            bool isFoundPawn = false;
            foreach (char fenSymbol in rankFen)
            {
                if (IsEmptySpaceNumber(fenSymbol))
                {
                    currentFile += CharToOrdinalInt(fenSymbol);
                }
                else
                {
                    currentFile += 1;
                }

                // If found corresponding file(a-g) and pawn
                if (fenSymbol == pawn && currentFile == file)
                {
                    isFoundPawn = true;
                    break;
                }
            }

            if (!isFoundPawn)
            {
                _errorMessage = $"En passant square '{ep}' is not in the same file as the pawn";
                _isValid = false;
            }
        }

        private static int CharToOrdinalInt(char ch)
        {
            return int.Parse(ch.ToString());
        }

        /// Is 0-8
        private static bool IsEmptySpaceNumber(char ch)
        {
            return ch is > '0' and <= '8';
        }

        private static void ValidateHalfMove()
        {
            if (!_isValid)
            {
                return;
            }

            if (!int.TryParse(_fenParsed.HalfMoveClock, out _))
            {
                _errorMessage = $"Half move '{_fenParsed.HalfMoveClock}' must be a number";
                _isValid = false;
            }
        }

        private static void ValidateFullMove()
        {
            if (!_isValid)
            {
                return;
            }

            if (!int.TryParse(_fenParsed.FullMoveCounter, out _))
            {
                _errorMessage = $"Full move '{_fenParsed.FullMoveCounter}' must be a number";
                _isValid = false;
            }
        }
    }
}