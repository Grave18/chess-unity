using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Chess3D.Runtime.Core.Ui.AlgebraicNotation
{
    public class AlgebraicNotationPanel : MonoBehaviour, IDisposable
    {
        private const int NumOfVisibleItems = 5;

        private Game _game;
        private UciBuffer _uciBuffer;
        private CoreEvents _coreEvents;

        [Header("Ui")]
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private GameObject notationItemPrefab;

        /// Zero based full move index
        private int _fullMoveIndex = -1;
        private readonly List<NotationItem> _notationItems = new();
        private int FullMoveCounter => _uciBuffer.FullMoveCounter;

        [Inject]
        public void Construct(Game game, UciBuffer uciBuffer, CoreEvents coreEvents)
        {
            _game = game;
            _uciBuffer = uciBuffer;
            _coreEvents = coreEvents;

            _coreEvents.OnWarmup += Clear;
            _coreEvents.OnEnd += NotateEndGame;
            _uciBuffer.OnAdd += OnAdd;
            _uciBuffer.OnUndo += OnUndo;
            _uciBuffer.OnRedo += OnRedo;
            _uciBuffer.OnDelete += OnDelete;
        }

        public void Dispose()
        {
            if (_coreEvents is null)
            {
                return;
            }

            _coreEvents.OnWarmup -= Clear;
            _coreEvents.OnEnd -= NotateEndGame;
            _uciBuffer.OnAdd -= OnAdd;
            _uciBuffer.OnUndo -= OnUndo;
            _uciBuffer.OnRedo -= OnRedo;
            _uciBuffer.OnDelete -= OnDelete;
        }

        private void Clear()
        {
            foreach (NotationItem notationItem in _notationItems)
            {
                Destroy(notationItem.gameObject);
            }

            _fullMoveIndex = 0;
            _notationItems.Clear();
        }

        private void NotateEndGame()
        {
            NotationItem notationItem = GetNotationItem();
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

            notationItem.AddEndGame(algebraicNotation);
        }

        private void OnAdd(MoveData moveData)
        {
            string algebraicNotation = moveData.AlgebraicNotation;
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
            else
            {
                Debug.LogError("AlgebraicNotationPanel: Invalid turn color");
            }

            ResetScrollbar(0);
        }

        private NotationItem GetNotationItem()
        {
            GameObject instance = Instantiate(notationItemPrefab, scrollView.content);
            var notationItem = instance.GetComponent<NotationItem>();
            _notationItems.Add(notationItem);
            return notationItem;
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
            else
            {
                Debug.LogError("AlgebraicNotationPanel: Invalid turn color");
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
            else
            {
                Debug.LogError("AlgebraicNotationPanel: Invalid turn color");
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
            else
            {
                Debug.LogError("AlgebraicNotationPanel: Invalid turn color");
            }

            ResetScrollbar(GetPositionFromIndex());
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
            if (_fullMoveIndex >= NumOfVisibleItems)
            {
                // Do not count indices what already on screen
                float verticalNormalizedPosition = 1f - (_fullMoveIndex + 1f - NumOfVisibleItems)/(_notationItems.Count - NumOfVisibleItems);
                return verticalNormalizedPosition;
            }

            return 1f;
        }
    }
}