using System.Text;
using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Info;
using ChessGame.ChessBoard.Pieces;
using ChessGame.Logic;
using ChessGame.Logic.MovesBuffer;
using UnityEngine;

#if UNITY_EDITOR
using EditorCools;
#endif

namespace Notation
{
    public class FenFromBoard : MonoBehaviour
    {
        private Game _game;
        private Board _board;
        private UciBuffer _uciBuffer;
        private string _fen;

        private readonly StringBuilder _uciStringBuilder = new();

        public void Init(Game game, Board board, UciBuffer uciBuffer, string fen)
        {
            _game = game;
            _board = board;
            _uciBuffer = uciBuffer;
            _fen = fen;
        }

        public string Get()
        {
            _uciStringBuilder.Clear();

            AppendLetters();
            AppendColor();
            AppendCastling();
            AppendEnPassantSquare();
            AppendHalfMove();
            AppendFullMove();

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
            var squares = _board.Squares;

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
            string color = _game.CurrentTurnColor switch
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
            Piece piece = _board.GetSquare(squareAddress).GetPiece();
            bool pieceIsFirstMove = piece != null && piece.IsFirstMove;
            return pieceIsFirstMove;
        }

        private void AppendEnPassantSquare()
        {
            EnPassantInfo enPassantInfo = _board.GetEnPassantInfo();
            string enPassant = enPassantInfo != null
                ? $" {enPassantInfo.Square.Address}"
                : " -";
            _uciStringBuilder.Append(enPassant);
        }

        private void AppendHalfMove()
        {
            int halfMoveClock = _uciBuffer.HalfMoveClock;

            _uciStringBuilder.Append($" {halfMoveClock}");
        }

        private void AppendFullMove()
        {
            int fullMoveCounter = _uciBuffer.FullMoveCounter;

            _uciStringBuilder.Append($" {fullMoveCounter}");
        }

#if UNITY_EDITOR

        [Button(space: 10)]
        public void ShowAndCopyToClipboard()
        {
            string uci = Get();

            GUIUtility.systemCopyBuffer = uci;

            Debug.Log($"<color=gray>{uci}</color>");
            Debug.Log($"Is identical with preset: {uci == _fen}");
        }

#endif

    }
}