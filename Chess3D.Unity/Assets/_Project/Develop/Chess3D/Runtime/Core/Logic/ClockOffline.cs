using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic
{
    public class ClockOffline : MonoBehaviour, IClock, IDisposable
    {
        private Game _game;
        private SettingsService _settingsService;
        private CoreEvents _coreEvents;

        private TimeSpan _initialWhiteTime;
        private TimeSpan _initialBlackTime;

        private TimeSpan _whiteTime;
        private TimeSpan _blackTime;

        private bool _isPlaying;

        public TimeSpan WhiteTime => _whiteTime;
        public TimeSpan BlackTime => _blackTime;

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