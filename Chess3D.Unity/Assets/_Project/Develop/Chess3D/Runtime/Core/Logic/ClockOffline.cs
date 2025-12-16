using System;
using Chess3D.Runtime.Bootstrap.Settings;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic
{
    public class ClockOffline : MonoBehaviour, IClock, IDisposable
    {
        private Game _game;
        private ISettingsContainer settingsContainer;

        private TimeSpan _initialWhiteTime;
        private TimeSpan _initialBlackTime;

        private TimeSpan _whiteTime;
        private TimeSpan _blackTime;

        private bool _isPlaying;

        public TimeSpan WhiteTime => _whiteTime;
        public TimeSpan BlackTime => _blackTime;

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