using System;
using Chess3D.Runtime.Core.Sound;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.Logic
{
    [Preserve]
    public sealed class Clock : IClock, IDisposable
    {
        private readonly ReactiveProperty<TimeSpan> _whiteTime = new();
        private readonly ReactiveProperty<TimeSpan> _blackTime = new();

        private bool _isPlaying;

        private readonly Game _game;
        private readonly ISettingsContainer _settingsContainer;
        private readonly CoreEvents _coreEvents;

        private readonly CompositeDisposable _disposables = new();

        public IObservable<TimeSpan> WhiteTime => _whiteTime;
        public IObservable<TimeSpan> BlackTime => _blackTime;

        public IObservable<Unit> WhiteTenSeconds => _coreEvents.OnWarmup.SelectMany(_ =>
            _whiteTime
                .Where(t => (int)t.TotalSeconds == 10)
                .Take(1)
                .AsUnitObservable()
        );

        public IObservable<Unit> BlackTenSeconds => _coreEvents.OnWarmup.SelectMany(_ =>
            _blackTime
                .Where(t => (int)t.TotalSeconds == 10)
                .Take(1)
                .AsUnitObservable()
        );

        public Clock(Game game, ISettingsContainer settingsContainer, CoreEvents coreEvents,
            EffectsPlayer effectsPlayer)
        {
            _game = game;
            _settingsContainer = settingsContainer;
            _coreEvents = coreEvents;

            Observable.EveryGameObjectUpdate().Subscribe(Tick).AddTo(_disposables);
            coreEvents.OnWarmup.Subscribe(_ => Pause()).AddTo(_disposables);
            coreEvents.OnStart.Subscribe(_ => Play()).AddTo(_disposables);
            coreEvents.OnEndMove.Subscribe(_ => Play()).AddTo(_disposables);
            coreEvents.OnPlay.Subscribe(_ => Play()).AddTo(_disposables);
            coreEvents.OnPause.Subscribe(_ => Pause()).AddTo(_disposables);
            coreEvents.OnEnd.Subscribe(_ => Pause()).AddTo(_disposables);
            WhiteTenSeconds.Subscribe(_ => effectsPlayer.PlayTenSecondsSound()).AddTo(_disposables);
            BlackTenSeconds.Subscribe(_ => effectsPlayer.PlayTenSecondsSound()).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public UniTask Load()
        {
            TimeSpan time = TimeSpan.FromMinutes(_settingsContainer.GameSettings.Time.x)
                                   + TimeSpan.FromSeconds(_settingsContainer.GameSettings.Time.y);

            _whiteTime.Value = time;
            _blackTime.Value = time;

            return UniTask.CompletedTask;
        }

        private void Tick(long _)
        {
            if (!_isPlaying)
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

        private void ReduceTime(ReactiveProperty<TimeSpan> time, PieceColor pieceColor)
        {
            time.Value -= TimeSpan.FromSeconds(Time.deltaTime);

            if (time.Value.TotalSeconds <= 0)
            {
                _isPlaying = false;
                time.Value = TimeSpan.Zero;
                _game.TimeOutSetup(pieceColor);
            }
        }
    }
}