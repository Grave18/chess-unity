using System.Collections;
using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using GameAndScene.Initialization;
using Highlighting;
using UnityEngine;

namespace Logic.Players
{
    public class PlayerOffline : Player
    {
        private readonly Camera _mainCamera;
        private readonly Highlighter _highlighter;
        private readonly LayerMask _layerMask;

        private const float MaxDistance = 100;

        // Selected in Ui
        private TaskCompletionSource<PieceType> _pieceTypeCompletionSource = new();

        public PlayerOffline(Game game, CommandInvoker commandInvoker, Camera mainCamera, Highlighter highlighter,
            LayerMask layerMask, PlayerSettings playerSettings)
            : base(game, commandInvoker)
        {
            _highlighter = highlighter;
            _mainCamera = mainCamera;
            _layerMask = layerMask;
        }

        private IEnumerator _coroutine;

        public override async Task<PieceType> RequestPromotedPiece()
        {
            return await _pieceTypeCompletionSource.Task;
        }

        public override void SelectPromotedPiece(PieceType pieceType)
        {
            _pieceTypeCompletionSource.SetResult(pieceType);
            _pieceTypeCompletionSource = new TaskCompletionSource<PieceType>();
        }

        public override void AllowMakeMove()
        {
        }

        public override void DisallowMakeMove()
        {
        }

        public override void Update()
        {
            // Cast ray from cursor
            Vector3 mousePos = Input.mousePosition;
            Ray ray = _mainCamera.ScreenPointToRay(mousePos);
            bool isHit = Physics.Raycast(ray, out RaycastHit hit, MaxDistance, _layerMask);
            Transform hitTransform = hit.transform;

            if (Input.GetButtonDown("Fire1"))
            {
                ISelectable selectable = null;

                if (hitTransform != null)
                {
                    hitTransform.TryGetComponent(out selectable);
                }

                Click(selectable);
                _highlighter.UpdateHighlighting();
            }
        }

        private void Click(ISelectable selectable)
        {
            // Deselect if not hit anything
            if (selectable == null)
            {
                Game.Deselect();
                return;
            }

            // Select if clicked on current turn piece
            if (Game.CanSelect(selectable))
            {
                Game.Select(selectable);
                return;
            }

            // Exit early if no selected piece
            if (Game.Selected == null)
            {
                return;
            }

            string uci = GetUci(selectable);
            Game.Move(uci);
            Game.Deselect();
        }

        /// Get Uci string of move. example: "b1a1q", "e2e4"
        private string GetUci(ISelectable selectable)
        {
            Piece piece = Game.Selected.GetPiece();
            Square fromSquare = Game.Selected.GetSquare();
            Square toSquare = selectable.GetSquare();

            string uci = $"{fromSquare?.Address}{toSquare?.Address}";

            if (CanPromote(piece, toSquare))
            {
                uci += "q";
            }

            return uci;
        }

        private static bool CanPromote(Piece piece, Square toSquare)
        {
            return piece is Pawn
                   && toSquare?.Rank is "1" or "8"
                   && (piece.CanMoveTo(toSquare, out _) || piece.CanCaptureAt(toSquare, out _));
        }
    }
}