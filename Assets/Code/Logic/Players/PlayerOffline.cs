using System.Collections;
using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
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
        private bool _isAllowMove;

        // Selected in Ui
        private TaskCompletionSource<PieceType> _pieceTypeCompletionSource = new();

        public PlayerOffline(Game game, CommandInvoker commandInvoker, Camera mainCamera, Highlighter highlighter,
            LayerMask layerMask)
            :base(game, commandInvoker)
        {
            _highlighter = highlighter;
            _mainCamera = mainCamera;
            _layerMask = layerMask;
        }

        public override void Start()
        {
            Game.StartCoroutine(Update());
        }

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
            _isAllowMove = true;
        }

        public override void DisallowMakeMove()
        {
            _isAllowMove = false;
        }

        private IEnumerator Update()
        {
            while(Application.isPlaying)
            {
                if(!_isAllowMove)
                {
                    yield return null;
                }

                // Cast ray from cursor
                Vector3 mousePos = Input.mousePosition;
                Ray ray = _mainCamera.ScreenPointToRay(mousePos);
                bool isHit = Physics.Raycast(ray, out RaycastHit hit, MaxDistance, _layerMask);
                Transform hitTransform = hit.transform;

                if (Input.GetButtonDown("Fire1") && Game.State == GameState.Idle)
                {
                    ISelectable selectable = null;

                    if (hitTransform != null)
                    {
                        hitTransform.TryGetComponent(out selectable);
                    }

                    Click(selectable);
                    _highlighter.UpdateHighlighting();
                }
                else
                {
                    Hover(isHit, hitTransform);
                }

                yield return null;
            }
        }

        private void Click(ISelectable selectable)
        {
            // Deselect if not hit anything
            if (selectable == null)
            {
                DeselectCurrent();
                return;
            }

            // Select if clicked on current turn piece
            if (selectable.HasPiece() && Game.IsRightTurn(selectable.GetPieceColor()))
            {
                Select(selectable);
                return;
            }

            // Exit early if no selected piece
            if (Game.Selected == null)
            {
                return;
            }

            Piece piece = Game.Selected.GetPiece();
            Square moveToSquare = selectable.GetSquare();

            // Move
            if (piece.CanMoveTo(moveToSquare, out MoveInfo moveInfo))
            {
                _ = CommandInvoker.MoveTo(piece, moveToSquare, moveInfo);
            }
            // Castling
            else if (piece is King king && king.CanCastlingAt(moveToSquare, out CastlingInfo castlingInfo))
            {
                _ = CommandInvoker.Castling(king, moveToSquare, castlingInfo.Rook, castlingInfo.RookSquare, castlingInfo.NotationTurnType);
            }
            // Eat
            else if (piece.CanEatAt(moveToSquare, out CaptureInfo captureInfo))
            {
                _ = CommandInvoker.EatAt(piece, moveToSquare, captureInfo);
            }

            DeselectCurrent();
        }

        private void Select(ISelectable selectable)
        {
            Game.Selected = selectable;
        }

        private void DeselectCurrent()
        {
            Game.Selected = null;
        }

        private void Hover(bool isHit, Transform hitTransform)
        {
            // Dehighlight if not hit anything
            if (!isHit)
            {
                Game.Highlighted = null;

                return;
            }

            bool tryGetSelectable = hitTransform.TryGetComponent(out ISelectable selectable);

            // Highlight
            if (tryGetSelectable && !selectable.IsEqual(Game.Highlighted))
            {
                Game.Highlighted = selectable;
            }
        }
    }
}
