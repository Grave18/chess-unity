using System.Text;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using VContainer;

namespace Chess3D.Runtime.Core.Notation
{
    [UnityEngine.Scripting.Preserve]
    public class FenFromBoard
    {
        private readonly IObjectResolver _resolver;

        private readonly StringBuilder _uciStringBuilder = new();

        public FenFromBoard(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public string Get()
        {
            _uciStringBuilder.Clear();

            AppendLetters();
            AppendColor();
            AppendCastling();
            AppendEnPassantSquare();
            AppendHalfMoveClock();
            AppendFullMoveCounter();

            return _uciStringBuilder.ToString();
        }

        /// Don't contain a half move and full move
        public string GetShort()
        {
            _uciStringBuilder.Clear();

            AppendLetters();
            AppendColor();
            AppendCastling();
            AppendEnPassantSquare();

            return _uciStringBuilder.ToString();
        }

        private void AppendLetters()
        {
            // TODO: Hidden dependency
            var board = _resolver.Resolve<Board>();
            var squares = board.Squares;

            int counter = 0;
            foreach (Square square in squares)
            {
                Piece piece = square.GetPiece();
                char letter = ' ';

                switch (piece)
                {
                    case Pawn:   letter = 'p'; break;
                    case Knight: letter = 'n'; break;
                    case Bishop: letter = 'b'; break;
                    case Rook:   letter = 'r'; break;
                    case Queen:  letter = 'q'; break;
                    case King:   letter = 'k'; break;
                    case null:   counter += 1; break;
                }

                if(piece != null)
                {
                    AppendAndResetCounterIfPossible(ref counter);
                    AppendPieceLetter(letter, piece);
                }

                if (IsLastColumnFile(square))
                {
                    AppendAndResetCounterIfPossible(ref counter);
                    AppendSeparatorIfPossible(square);
                }
            }
        }

        private static bool IsLastColumnFile(Square square)
        {
            return square.File == "h";
        }

        private void AppendAndResetCounterIfPossible(ref int counter)
        {
            if (counter != 0)
            {
                _uciStringBuilder.Append(counter);
                counter = 0;
            }
        }

        private void AppendSeparatorIfPossible(Square square)
        {
            if(square.Rank != "1")
            {
                _uciStringBuilder.Append('/');
            }
        }

        private void AppendPieceLetter(char letter, Piece piece)
        {
            if (piece.GetPieceColor() == PieceColor.White)
            {
                letter = char.ToUpper(letter);
            }

            _uciStringBuilder.Append(letter);
        }

        private void AppendColor()
        {
            // TODO: hidden dependency
            var game = _resolver.Resolve<Game>();
            string color = game.CurrentTurnColor switch
            {
                PieceColor.White => " w",
                PieceColor.Black => " b",
                _ => " <color error>",
            };

            _uciStringBuilder.Append(color);
        }

        private void AppendCastling()
        {
            string castling = string.Empty;

            bool isWqRookFirst = IsPieceFirstMove("a1");
            bool isWkRookFirst = IsPieceFirstMove("h1");
            bool isWKingFirst =  IsPieceFirstMove("e1");

            bool isBqRookFirst = IsPieceFirstMove("a8");
            bool isBkRookFirst = IsPieceFirstMove("h8");
            bool isBKingFirst =  IsPieceFirstMove("e8");

            if (isWKingFirst && isWkRookFirst) { castling += 'K'; }
            if (isWKingFirst && isWqRookFirst) { castling += 'Q'; }
            if (isBKingFirst && isBkRookFirst) { castling += 'k'; }
            if (isBKingFirst && isBqRookFirst) { castling += 'q'; }

            _uciStringBuilder.Append(castling == string.Empty ? " -" : $" {castling}");
        }

        private bool IsPieceFirstMove(string squareAddress)
        {
            // TODO: Hidden dependency
            var board = _resolver.Resolve<Board>();
            Piece piece = board.GetSquare(squareAddress).GetPiece();
            bool pieceIsFirstMove = piece != null && piece.IsFirstMove;
            return pieceIsFirstMove;
        }

        private void AppendEnPassantSquare()
        {
            // TODO: Hidden dependency
            var board = _resolver.Resolve<Board>();
            EnPassantInfo enPassantInfo = board.GetEnPassantInfo();
            string enPassant = enPassantInfo != null
                ? $" {enPassantInfo.Square.Address}"
                : " -";
            _uciStringBuilder.Append(enPassant);
        }

        private void AppendHalfMoveClock()
        {
            // TODO: hidden dependency
            var uciBuffer = _resolver.Resolve<UciBuffer>();
            int halfMoveClock = uciBuffer.HalfMoveClock;

            _uciStringBuilder.Append($" {halfMoveClock}");
        }

        private void AppendFullMoveCounter()
        {
            // TODO: hidden dependency
            var uciBuffer = _resolver.Resolve<UciBuffer>();
            int fullMoveCounter = uciBuffer.FullMoveCounter;

            _uciStringBuilder.Append($" {fullMoveCounter}");
        }
    }
}