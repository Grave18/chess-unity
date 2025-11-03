using System;
using UnityEngine;

namespace Chess3D.Runtime.Core.Logic
{
    public class ClockOffline : MonoBehaviour, IClock
    {
        private Game _game;

        private TimeSpan _initialWhiteTime;
        private TimeSpan _initialBlackTime;

        private TimeSpan _whiteTime;
        private TimeSpan _blackTime;

        private bool _isPlaying;
        private bool _isInitialized;
        private bool _isOnline;

        public TimeSpan WhiteTime => _whiteTime;
        public TimeSpan BlackTime => _blackTime;

        public void Init(Game game, Vector2Int time)
        {
            if (_isInitialized)
            {
                Debug.Log("{nameof(Clock): Clock is already initialized}");
                return;
            }

            _game = game;
            _initialWhiteTime = TimeSpan.FromMinutes(time.x) + TimeSpan.FromSeconds(time.y);
            _initialBlackTime = _initialWhiteTime;

            _game.OnWarmup += InitTime;
            _game.OnStart += StartTimer;
            _game.OnEndMove += Play;
            _game.OnEnd += Pause;
            _game.OnPlay += Play;
            _game.OnPause += Pause;

            _isInitialized = true;
        }

        private void OnDestroy()
        {
            if (_game)
            {
                _game.OnWarmup -= InitTime;
                _game.OnStart -= StartTimer;
                _game.OnEndMove -= Play;
                _game.OnEnd -= Pause;
                _game.OnPlay -= Play;
                _game.OnPause -= Pause;
            }
        }

        private void StartTimer()
        {
            InitTime();
            Play();
        }

        private void InitTime()
        {
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
                _game.TimeOutSetup(pieceColor);
            }
        }
    }
}