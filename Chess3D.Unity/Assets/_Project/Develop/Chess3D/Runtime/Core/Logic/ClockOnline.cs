using System;
using Cysharp.Threading.Tasks;
using PurrNet;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic
{
    public sealed class ClockOnline : NetworkBehaviour, IClock, IDisposable
    {
        private Game _game;
        private SettingsService _settingsService;
        private CoreEvents _coreEvents;

        private TimeSpan _initialWhiteTime;
        private TimeSpan _initialBlackTime;

        private readonly SyncVar<TimeSpan> _whiteTime = new();
        private readonly SyncVar<TimeSpan> _blackTime = new();

        private bool _isPlaying;

        public TimeSpan WhiteTime => _whiteTime.value;
        public TimeSpan BlackTime => _blackTime.value;

        [Inject]
        public void Construct(Game game, SettingsService settingsService, CoreEvents coreEvents)
        {
            _game = game;
            _settingsService = settingsService;
            _coreEvents = coreEvents;

            _coreEvents.OnWarmup += InitTime;
            _coreEvents.OnStart += StartTimer;
            _coreEvents.OnEndMove += Play;
            _coreEvents.OnEnd += Pause;
            _coreEvents.OnPlay += Play;
            _coreEvents.OnPause += Pause;
        }

        public void Dispose()
        {
            if (_coreEvents is null)
            {
                return;
            }

            _coreEvents.OnWarmup -= InitTime;
            _coreEvents.OnStart -= StartTimer;
            _coreEvents.OnEndMove -= Play;
            _coreEvents.OnEnd -= Pause;
            _coreEvents.OnPlay -= Play;
            _coreEvents.OnPause -= Pause;
        }

        public UniTask Load()
        {
            Vector2Int time = _settingsService.S.GameSettings.Time;
            _initialWhiteTime = TimeSpan.FromMinutes(time.x) + TimeSpan.FromSeconds(time.y);
            _initialBlackTime = _initialWhiteTime;

            return UniTask.CompletedTask;
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
            if (!_isPlaying)
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
            _game.TimeOutSetup(pieceColor);
        }
    }
}