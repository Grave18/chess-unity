using ChessBoard;
using ChessBoard.Pieces;
using Highlighting;
using Ui.Promotion;
using UnityEngine;

namespace Logic.Players
{
    public class PlayerOffline : Player
    {
        private readonly Camera _mainCamera;
        private readonly PromotionPanel _promotionPanel;
        private readonly Highlighter _highlighter;
        private readonly LayerMask _layerMask;
        private readonly bool _isAutoPromoteToQueen;

        private const float MaxDistance = 100;

        public PlayerOffline(Game game, Camera mainCamera, Highlighter highlighter, LayerMask layerMask,
            bool isAutoPromoteToQueen, PromotionPanel promotionPanel)
            : base(game)
        {
            _highlighter = highlighter;
            _mainCamera = mainCamera;
            _layerMask = layerMask;
            _isAutoPromoteToQueen = isAutoPromoteToQueen;
            _promotionPanel = promotionPanel;
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

            ConstructUci(selectable);
        }

        /// Construct Uci string of move. example: "b1a1q", "e2e4"
        private void ConstructUci(ISelectable selectable)
        {
            Piece piece = Game.Selected.GetPiece();
            Square fromSquare = Game.Selected.GetSquare();
            Square toSquare = selectable.GetSquare();

            string uci = $"{fromSquare?.Address}{toSquare?.Address}";

            if (CanPromote(piece, toSquare))
            {
                if (_isAutoPromoteToQueen)
                {
                    uci += "q";
                    Move(uci);
                    return;
                }

                Game.Pause();
                _promotionPanel.RequestPromotedPiece(Game.CurrentTurnColor, pieceLetter =>
                {
                    uci += pieceLetter;
                    Move(uci);
                    Game.Play();
                });
            }
            else
            {
                Move(uci);
            }
        }

        private void Move(string uci)
        {
            Game.Move(uci);
            Game.Deselect();
        }

        private static bool CanPromote(Piece piece, Square toSquare)
        {
            return piece is Pawn
                   && toSquare?.Rank is "1" or "8"
                   && (piece.CanMoveTo(toSquare, out _) || piece.CanCaptureAt(toSquare, out _));
        }
    }
}