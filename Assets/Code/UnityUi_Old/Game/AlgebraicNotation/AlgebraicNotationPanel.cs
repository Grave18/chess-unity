using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChessGame.Logic;
using ChessGame.Logic.MovesBuffer;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game.AlgebraicNotation
{
    public class AlgebraicNotationPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessGame.Logic.Game game;
        [SerializeField] private UciBuffer uciBuffer;

        [Header("Ui")]
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private GameObject notationItemPrefab;

        [Header("Settings")]
        [SerializeField] private int numOfVisibleItems = 5;

        private int _index = -1;
        private readonly List<NotationItem> _notationItems = new();

        private void OnEnable()
        {
            game.OnWarmup += Clear;
            game.OnEnd += NotateEndGame;
            uciBuffer.OnAdd += OnAdd;
            uciBuffer.OnUndo += OnUndo;
            uciBuffer.OnRedo += OnRedo;
            uciBuffer.OnDelete += OnDelete;
        }

        private void OnDisable()
        {
            game.OnWarmup -= Clear;
            game.OnEnd -= NotateEndGame;
            uciBuffer.OnAdd -= OnAdd;
            uciBuffer.OnUndo -= OnUndo;
            uciBuffer.OnRedo -= OnRedo;
            uciBuffer.OnDelete -= OnDelete;
        }

        private void Clear()
        {
            foreach (NotationItem notationItem in _notationItems)
            {
                Destroy(notationItem.gameObject);
            }

            _index = 0;
            _notationItems.Clear();
        }

        private void NotateEndGame()
        {
            NotationItem notationItem = GetNotationItem();
            string algebraicNotation = string.Empty;

            if (game.IsDraw)
            {
                algebraicNotation = "½-½";
            }
            else if (game.IsWinnerWhite)
            {
                algebraicNotation = "1-0";
            }
            else if (game.IsWinnerBlack)
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
                _index += 1;
                NotationItem notationItem = GetNotationItem();
                notationItem.AddWhite(algebraicNotation, _notationItems.Count);
            }
            else if (moveData.TurnColor == PieceColor.Black)
            {
                NotationItem notationItem = _notationItems.LastOrDefault();
                if (!notationItem)
                {
                    notationItem = GetNotationItem();
                    notationItem.AddBlack(algebraicNotation, _notationItems.Count);
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
            if (_index < 0 || _index >= _notationItems.Count)
            {
                return;
            }

            NotationItem notationItem = _notationItems[_index];

            if (moveData.TurnColor == PieceColor.White)
            {
                _index -= 1;
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
                _index += 1;
                if (_index >= 0 && _index < _notationItems.Count)
                {
                    NotationItem notationItem = _notationItems[_index];
                    notationItem.UnFadeWhite();
                }
            }
            else if (moveData.TurnColor == PieceColor.Black)
            {
                if (_index >= 0 && _index < _notationItems.Count)
                {
                    NotationItem notationItem = _notationItems[_index];
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
            if (_index >= numOfVisibleItems)
            {
                // Do not count indices what already on screen
                float verticalNormalizedPosition = 1f - (_index + 1 - numOfVisibleItems)/(float)( _notationItems.Count - numOfVisibleItems);
                return verticalNormalizedPosition;
            }

            return 1f;
        }
    }
}