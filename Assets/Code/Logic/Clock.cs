using System;
using PurrNet;
using UnityEngine;

namespace Logic
{
    public class Clock : NetworkBehaviour
    {
        [SerializeField] private Vector2Int time = new(30, 0);

        private bool _isPlaying;

        private SyncVar<TimeSpan> _initialWhiteTime = new();
        private SyncVar<TimeSpan> _initialBlackTime = new();

        private SyncVar<TimeSpan> _whiteTime = new();
        private SyncVar<TimeSpan> _blackTime = new();

        private Game _game;

        public TimeSpan WhiteTime => _whiteTime.value;
        public TimeSpan BlackTime => _blackTime.value;

        private bool _isInitialized;

        public void Init(Game game)
        {
            _game = game;
            _initialWhiteTime.value = TimeSpan.FromMinutes(time.x) + TimeSpan.FromSeconds(time.y);
            _initialBlackTime.value = _initialWhiteTime.value;

            OnEnable();
        }

        private void OnEnable()
        {
            if(_game == null || _isInitialized)
            {
                return;
            }

            if (!isController)
            {
                return;
            }

            _game.OnStart += StartTimer;
            _game.OnEndMove += Play;
            _game.OnEnd += Pause;
            _game.OnPlay += Play;
            _game.OnPause += Pause;

            _isInitialized = true;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            if (!isController)
            {
                return;
            }

            OnEnable();
        }

        private void OnDisable()
        {
            if (!isController)
            {
                return;
            }

            _game.OnStart -= StartTimer;
            _game.OnEndMove -= Play;
            _game.OnEnd -= Pause;
            _game.OnPlay -= Play;
            _game.OnPause -= Pause;
        }

        private void StartTimer()
        {
            _isPlaying = true;
            _whiteTime.value = _initialWhiteTime.value;
            _blackTime.value = _initialBlackTime.value;
        }

        private void Play(PieceColor color)
        {
            Play();
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
                _game.SetTimeOut(pieceColor);
            }
        }
    }
}