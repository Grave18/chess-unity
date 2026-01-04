using System;
using Chess3D.Runtime.Core.Sound;
using Cysharp.Threading.Tasks;
using PurrNet;
using UniRx;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic
{
    [UnityEngine.Scripting.Preserve]
    public sealed class ClockOnline : NetworkBehaviour, IClock
    {
        private readonly SyncVar<TimeSpan> _whiteTime = new();
        private readonly SyncVar<TimeSpan> _blackTime = new();

        private readonly Subject<TimeSpan> _whiteTimeObservable = new();
        private readonly Subject<TimeSpan> _blackTimeObservable = new();

        private bool _isPlaying;

        private Game _game;
        private ISettingsContainer _settingsContainer;
        private CoreEvents _coreEvents;

        public IObservable<TimeSpan> WhiteTime => _whiteTimeObservable;
        public IObservable<TimeSpan> BlackTime => _blackTimeObservable;

        public IObservable<Unit> WhiteTenSeconds => _coreEvents.OnWarmup.SelectMany(_ =>
            WhiteTime
                .Where(t => (int)t.TotalSeconds == 10)
                .Take(1)
                .AsUnitObservable()
        );

        public IObservable<Unit> BlackTenSeconds => _coreEvents.OnWarmup.SelectMany(_ =>
            BlackTime
                .Where(t => (int)t.TotalSeconds == 10)
                .Take(1)
                .AsUnitObservable()
        );

        [Inject]
        public void Construct(Game game, ISettingsContainer settingsContainer, CoreEvents coreEvents,
            EffectsPlayer effectsPlayer)
        {
            _game = game;
            _settingsContainer = settingsContainer;
            _coreEvents = coreEvents;

            Observable.EveryGameObjectUpdate().Subscribe(Tick).AddTo(this);
            coreEvents.OnWarmup.Subscribe(_ => Pause()).AddTo(this);
            coreEvents.OnStart.Subscribe(_ => Play()).AddTo(this);
            coreEvents.OnEndMove.Subscribe(_ => Play()).AddTo(this);
            coreEvents.OnPlay.Subscribe(_ => Play()).AddTo(this);
            coreEvents.OnPause.Subscribe(_ => Pause()).AddTo(this);
            coreEvents.OnEnd.Subscribe(_ => Pause()).AddTo(this);
            WhiteTenSeconds.Subscribe(_ => effectsPlayer.PlayTenSecondsSound()).AddTo(this);
            BlackTenSeconds.Subscribe(_ => effectsPlayer.PlayTenSecondsSound()).AddTo(this);

            _whiteTime.onChanged += UpdateWhite;
            _blackTime.onChanged += UpdateBlack;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _whiteTime.onChanged -= UpdateWhite;
            _blackTime.onChanged -= UpdateBlack;
        }

        private void UpdateBlack(TimeSpan x) => _blackTimeObservable.OnNext(x);
        private void UpdateWhite(TimeSpan x) => _whiteTimeObservable.OnNext(x);

        public UniTask Load()
        {
            TimeSpan time = TimeSpan.FromMinutes(_settingsContainer.S.GameSettings.Time.x)
                                   + TimeSpan.FromSeconds(_settingsContainer.S.GameSettings.Time.y);

            if (isController)
            {
                _whiteTime.value = time;
                _blackTime.value = time;
            }

            return UniTask.CompletedTask;
        }

        private void Tick(long _)
        {
            if (!_isPlaying || !isController)
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

        private void Play()
        {
            _isPlaying = true;
        }

        private void Pause()
        {
            _isPlaying = false;
        }

        private void ReduceTime(SyncVar<TimeSpan> time, PieceColor pieceColor)
        {
            time.value -= TimeSpan.FromSeconds(Time.deltaTime);

            if (time.value.TotalSeconds <= 0)
            {
                _isPlaying = false;
                time.value = TimeSpan.Zero;
                _game.TimeOutSetup(pieceColor);
            }
        }
    }
}