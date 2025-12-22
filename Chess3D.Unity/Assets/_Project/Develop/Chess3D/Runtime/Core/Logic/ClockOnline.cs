using System;
using Cysharp.Threading.Tasks;
using PurrNet;
using UniRx;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic
{
    public sealed class ClockOnline : NetworkBehaviour, IClock, IDisposable
    {
        private Game _game;
        private ISettingsContainer settingsContainer;

        private TimeSpan _initialWhiteTime;
        private TimeSpan _initialBlackTime;

        private readonly SyncVar<TimeSpan> _whiteTime = new();
        private readonly SyncVar<TimeSpan> _blackTime = new();

        private bool _isPlaying;

        public TimeSpan WhiteTime1 => _whiteTime.value;
        public TimeSpan BlackTime2 => _blackTime.value;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Construct(Game game, ISettingsContainer settingsContainer, CoreEvents coreEvents)
        {
            _game = game;
            this.settingsContainer = settingsContainer;

            coreEvents.OnWarmup.Subscribe(_ => InitTime()).AddTo(_disposables);
            coreEvents.OnStart.Subscribe(_ => StartTimer()).AddTo(_disposables);
            coreEvents.OnEndMove.Subscribe(_ => Play()).AddTo(_disposables);
            coreEvents.OnEnd.Subscribe(_ => Pause()).AddTo(_disposables);
            coreEvents.OnPlay.Subscribe(_ => Play()).AddTo(_disposables);
            coreEvents.OnPause.Subscribe(_ => Pause()).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public UniTask Load()
        {
            Vector2Int time = settingsContainer.S.GameSettings.Time;
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

        public IObservable<TimeSpan> WhiteTime { get; }
        public IObservable<TimeSpan> BlackTime { get; }
        public IObservable<Unit> WhiteTenSeconds { get; }
        public IObservable<Unit> BlackTenSeconds { get; }
    }
}