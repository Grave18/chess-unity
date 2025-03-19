using System;
using UnityEngine;

namespace Logic
{
    // Todo: update to new system
    public class Clock : MonoBehaviour
    {
        [SerializeField] private int timeMinutes = 5;

        public bool IsPlaying { get; private set; }

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

            SubscribeToGameEvents();
        }

        public void StartTimer()
        {
            IsPlaying = true;
            _whiteTime = _initialWhiteTime;
            _blackTime = _initialBlackTime;
        }

        public void Play()
        {
            IsPlaying = true;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        private void Update()
        {
            if (!IsPlaying || _game == null)
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
                IsPlaying = false;
                time = TimeSpan.Zero;
                _game.SetTimeOut(pieceColor);
            }
        }

        private void SubscribeToGameEvents()
        {
            // _game.OnStart += StartTimer;
            // _game.OnEnd += Pause;
            // _game.OnPlay += Play;
            // _game.OnPause += Pause;
        }

        private void OnDestroy()
        {
            // _game.OnStart -= StartTimer;
            // _game.OnEnd -= Pause;
            // _game.OnPlay -= Play;
            // _game.OnPause -= Pause;
        }
    }
}