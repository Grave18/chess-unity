using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class CurrentPlayerPointerPanel : MonoBehaviour
    {
        [SerializeField] private Game game;
        [SerializeField] private Image whiteImage;
        [SerializeField] private Image blackImage;

        private void OnEnable()
        {
            game.OnStart += OnStart;
            game.OnChangeTurn += OnChangeTurn;
        }

        private void OnDisable()
        {
            game.OnStart -= OnStart;
            game.OnChangeTurn -= OnChangeTurn;
        }

        private void OnStart()
        {
            OnChangeTurn(game.CurrentTurnColor);
        }

        private void OnChangeTurn(PieceColor color)
        {
            if (color == PieceColor.White)
            {
                whiteImage.CrossFadeAlpha(1f, 0.2f, false);
                blackImage.CrossFadeAlpha(0f, 0.2f, false);
            }
            else if (color == PieceColor.Black)
            {
                whiteImage.CrossFadeAlpha(0f, 0.2f, false);
                blackImage.CrossFadeAlpha(1f, 0.2f, false);
            }
        }
    }
}