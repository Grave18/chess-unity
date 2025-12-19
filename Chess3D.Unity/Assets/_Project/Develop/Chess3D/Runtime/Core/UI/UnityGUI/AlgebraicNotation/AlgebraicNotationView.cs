using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.AlgebraicNotation
{
    public sealed class AlgebraicNotationView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private GameObject notationItemPrefab;

        [Header("Settings")]
        [SerializeField] private int numOfVisibleItems = 5;

        private readonly List<NotationItem> _notationItems = new();
        private int _fullMoveIndex;

        private Game _game;

        [Inject]
        public void Construct(Game game, UciBuffer uciBuffer, CoreEvents coreEvents)
        {
            _game = game;

            coreEvents.OnWarmup.Subscribe(_ => Initialize()).AddTo(this);
            coreEvents.OnEnd.Subscribe(_ => NotateEndGame()).AddTo(this);
            uciBuffer.OnAdd.Subscribe(OnAdd).AddTo(this);
            uciBuffer.OnUndo.Subscribe(OnUndo).AddTo(this);
            uciBuffer.OnRedo.Subscribe(OnRedo).AddTo(this);
            uciBuffer.OnDelete.Subscribe(OnDelete).AddTo(this);
        }

        private void Initialize()
        {
            foreach (NotationItem notationItem in _notationItems)
            {
                Destroy(notationItem.gameObject);
            }
            _notationItems.Clear();
            _fullMoveIndex = 0;
        }

        private void OnAdd(MoveData moveData)
        {
            string algebraicNotation = moveData.AlgebraicNotation;

            if (_game.IsEndGame)
            {
                Debug.Log("End Game");
            }

            if (moveData.TurnColor == PieceColor.White)
            {
                _fullMoveIndex += 1;
                NotationItem notationItem = GetNotationItem();
                int fullMoveCounter = moveData.FullMoveCounter;
                notationItem.AddWhite(algebraicNotation, fullMoveCounter);
            }
            else if (moveData.TurnColor == PieceColor.Black)
            {
                NotationItem notationItem = _notationItems.LastOrDefault();
                if (!notationItem)
                {
                    _fullMoveIndex += 1;
                    notationItem = GetNotationItem();
                    // Minus one because full move counter is updated after black move
                    // Factical second technically first move
                    int fullMoveCounter = moveData.FullMoveCounter - 1;
                    notationItem.AddBlack(algebraicNotation, fullMoveCounter);
                }
                else
                {
                    notationItem.AddBlack(algebraicNotation);
                }
            }

            ResetScrollbar(0);
        }

        private void OnDelete(MoveData moveData)
        {
            NotationItem notationItem = _notationItems.LastOrDefault();
            if (!notationItem)
            {
                return;
            }

            if (moveData.TurnColor == PieceColor.White)
            {
                _notationItems.Remove(notationItem);
                Destroy(notationItem.gameObject);
            }
            else if (moveData.TurnColor == PieceColor.Black)
            {
                notationItem.RemoveBlack();
            }
        }

        private void OnUndo(MoveData moveData)
        {
            if (_fullMoveIndex - 1 < 0 || _fullMoveIndex - 1 >= _notationItems.Count)
            {
                return;
            }

            NotationItem notationItem = _notationItems[_fullMoveIndex - 1];

            if (moveData.TurnColor == PieceColor.White)
            {
                _fullMoveIndex -= 1;
                notationItem.FadeWhite();
            }
            else if (moveData.TurnColor == PieceColor.Black)
            {
                notationItem.FadeBlack();
            }

            ResetScrollbar(GetPositionFromIndex());
        }

        private void OnRedo(MoveData moveData)
        {
            if (moveData.TurnColor == PieceColor.White)
            {
                _fullMoveIndex += 1;
                if (_fullMoveIndex - 1 >= 0 && _fullMoveIndex - 1 < _notationItems.Count)
                {
                    NotationItem notationItem = _notationItems[_fullMoveIndex - 1];
                    notationItem.UnFadeWhite();
                }
            }
            else if (moveData.TurnColor == PieceColor.Black)
            {
                if (_fullMoveIndex - 1 >= 0 && _fullMoveIndex - 1 < _notationItems.Count)
                {
                    NotationItem notationItem = _notationItems[_fullMoveIndex - 1];
                    notationItem.UnFadeBlack();
                }
            }

            ResetScrollbar(GetPositionFromIndex());
        }

        private void NotateEndGame()
        {
            string algebraicNotation = string.Empty;

            if (_game.IsDraw)
            {
                algebraicNotation = "½-½";
            }
            else if (_game.IsWinnerWhite)
            {
                algebraicNotation = "1-0";
            }
            else if (_game.IsWinnerBlack)
            {
                algebraicNotation = "0-1";
            }

            NotationItem notationItem = _notationItems.LastOrDefault();
            notationItem?.AddEndGame(algebraicNotation);
        }

        private NotationItem GetNotationItem()
        {
            GameObject instance = Instantiate(notationItemPrefab, scrollView.content);
            var notationItem = instance.GetComponent<NotationItem>();
            _notationItems.Add(notationItem);
            return notationItem;
        }

        private void ResetScrollbar(float position)
        {
            StartCoroutine(Coroutine());
            return;

            IEnumerator Coroutine()
            {
                yield return new WaitForEndOfFrame();
                scrollView.verticalNormalizedPosition = position;
            }
        }

        private float GetPositionFromIndex()
        {
            if (_fullMoveIndex >= numOfVisibleItems)
            {
                // Do not count indices what already on screen
                float verticalNormalizedPosition = 1f - (_fullMoveIndex + 1f - numOfVisibleItems)/(_notationItems.Count - numOfVisibleItems);
                return verticalNormalizedPosition;
            }

            return 1f;
        }
    }
}