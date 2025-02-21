using System;
using UnityEngine;

namespace Logic
{
    public class Clock : MonoBehaviour
    {
        private TimeSpan _whiteTime;
        private TimeSpan _blackTime;

        private TimeSpan _initialWhiteTime;
        private TimeSpan _initialBlackTime;

        private bool _isStarted;
        private Game _game;

        public TimeSpan WhiteTime => _whiteTime;
        public TimeSpan BlackTime => _blackTime;

        public void Init(Game game, float timeMinutes)
        {
            _game = game;
            _initialWhiteTime = TimeSpan.FromMinutes(timeMinutes);
            _initialBlackTime = _initialWhiteTime;

            _game.OnRestart += StartTimer;
        }

        public void StartTimer()
        {
            _isStarted = true;
            _whiteTime = _initialWhiteTime;
            _blackTime = _initialBlackTime;
        }

        public void Pause()
        {
            _isStarted = false;
        }

        private void Update()
        {
            if (!_isStarted || _game == null)
            {
                return;
            }

            PieceColor turnColor = _game.CurrentTurnColor;
            if (turnColor == PieceColor.White)
            {
                ReduceTime(ref _whiteTime, turnColor);
            }
            else if (turnColor == PieceColor.Black)
            {
                ReduceTime(ref _blackTime, turnColor);
            }
        }

        private void ReduceTime(ref TimeSpan time, PieceColor pieceColor)
        {
            time -= TimeSpan.FromSeconds(Time.deltaTime);

            if (time.TotalSeconds <= 0)
            {
                _isStarted = false;
                time = TimeSpan.Zero;
                _game.SetTimeOut(pieceColor);
            }
        }

        private void OnDestroy()
        {
            _game.OnRestart -= StartTimer;
        }
    }
}