using System.Text;
using AssetsAndResources;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using EditorCools;
using Logic;
using UnityEngine;

namespace Utils
{
    public class UciString : MonoBehaviour
    {
        private Game _game;
        private Board _board;
        private CommandInvoker _commandInvoker;
        private Assets _assets;

        private readonly StringBuilder _uciStringBuilder = new();

        public void Init(Game game, Board board, CommandInvoker commandInvoker, Assets assets)
        {
            _game = game;
            _board = board;
            _commandInvoker = commandInvoker;
            _assets = assets;
        }

        [Button(space: 10)]
        public void ShowUci()
        {
            string uci = Get();

            Debug.Log($"<color=gray>{uci}</color>");
            Debug.Log($"Is identical with preset: {uci == _assets.BoardPreset.Fen}");
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
                    AppendLetter(letter, piece);
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

        private void AppendLetter(char letter, Piece piece)
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
            EnPassantInfo enPassantInfo = _commandInvoker.GetEnPassantInfo();
            string enPassant = enPassantInfo != null
                ? $" {enPassantInfo.Square.Address}"
                : " -";
            _uciStringBuilder.Append(enPassant);
        }

        private void AppendHalfMove()
        {
            _uciStringBuilder.Append(" 0");
        }

        private void AppendFullMove()
        {
            _uciStringBuilder.Append(" 1");
        }
    }
}
