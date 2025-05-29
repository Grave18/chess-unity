using System;
using Initialization;
using PurrNet;
using UnityEngine;

namespace ChessGame.Logic
{
    public class ClockOnline : NetworkBehaviour, IClock
    {
        private Game _game;

        private TimeSpan _initialWhiteTime;
        private TimeSpan _initialBlackTime;

        private SyncVar<TimeSpan> _whiteTime = new();
        private SyncVar<TimeSpan> _blackTime = new();

        private bool _isPlaying;
        private bool _isInitialized;

        public TimeSpan WhiteTime => _whiteTime.value;
        public TimeSpan BlackTime => _blackTime.value;

        public void Init(Game game, GameSettings gameSettings)
        {
            if (_isInitialized)
            {
                Debug.Log("{nameof(Clock): Clock is already initialized}");
                return;
            }

            _game = game;
            _initialWhiteTime = TimeSpan.FromMinutes(gameSettings.Time.x)
                                + TimeSpan.FromSeconds(gameSettings.Time.y);
            _initialBlackTime = _initialWhiteTime;

            _game.OnWarmup += InitTime;
            _game.OnStart += StartTimer;
            _game.OnEndMove += Play;
            _game.OnEnd += Pause;
            _game.OnPlay += Play;
            _game.OnPause += Pause;

            _isInitialized = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

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
            if (!isHost)
            {
                return;
            }

            InitTime();
            Play();
        }

        private void InitTime()
        {
            if (!isHost)
            {
                return;
            }

            _whiteTime.value = _initialWhiteTime;
            _blackTime.value = _initialBlackTime;
        }

        [ObserversRpc]
        private void Play()
        {
            _isPlaying = true;
        }

        [ObserversRpc]
        private void Pause()
        {
            _isPlaying = false;
        }

        private void Update()
        {
            if (!_isPlaying || !_game)
            {
                return;
            }

            if (!isController)
            {
                return;
            }

            PieceColor turnColor = _game.CurrentTurnColor;
            if (turnColor == PieceColor.White)
            {
                ReduceTime(_whiteTime, turnColor);
            }
            else if (turnColor == PieceColor.Black)
            {
                ReduceTime(_blackTime, turnColor);
            }
        }

        private void ReduceTime(SyncVar<TimeSpan> time, PieceColor pieceColor)
        {
            time.value -= TimeSpan.FromSeconds(Time.deltaTime);

            if (time.value.TotalSeconds <= 0)
            {
                _isPlaying = false;
                time.value = TimeSpan.Zero;
                SetTimeoutAll(pieceColor);
            }
        }

        [ObserversRpc]
        private void SetTimeoutAll(PieceColor pieceColor)
        {
            _game.SetTimeOut(pieceColor);
        }
    }
}