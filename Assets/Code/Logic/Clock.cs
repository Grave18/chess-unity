using System;
using UnityEngine;

namespace Logic
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] private int timeMinutes = 5;

        private bool _isPlaying;

        private TimeSpan _initialWhiteTime;
        private TimeSpan _initialBlackTime;

        private TimeSpan _whiteTime;
        private TimeSpan _blackTime;

        private Game _game;

        public TimeSpan WhiteTime => _whiteTime;
        public TimeSpan BlackTime => _blackTime;

        public void Init(Game game)
        {
            _game = game;
            _initialWhiteTime = TimeSpan.FromMinutes(timeMinutes);
            _initialBlackTime = _initialWhiteTime;

            OnEnable();
        }

        private void OnEnable()
        {
            if(_game == null)
            {
                return;
            }

            _game.OnStart += StartTimer;
            _game.OnChangeTurn += Play;
            _game.OnEnd += Pause;
            _game.OnPlay += Play;
            _game.OnPause += Pause;
        }

        private void OnDisable()
        {
            _game.OnStart -= StartTimer;
            _game.OnChangeTurn -= Play;
            _game.OnEnd -= Pause;
            _game.OnPlay -= Play;
            _game.OnPause -= Pause;
        }

        private void StartTimer()
        {
            _isPlaying = true;
            _whiteTime = _initialWhiteTime;
            _blackTime = _initialBlackTime;
        }

        private void Play()
        {
            _isPlaying = true;
        }

        private void Pause()
        {
            _isPlaying = false;
        }

        private void Update()
        {
            if (!_isPlaying || _game == null)
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
                _isPlaying = false;
                time = TimeSpan.Zero;
                _game.SetTimeOut(pieceColor);
            }
        }
    }
}