using System;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [Preserve]
    public sealed class Competitors : ILoadUnit, IDisposable
    {
        public IPlayer CurrentPlayer { get; private set; }

        private readonly Game _game;
        private readonly ISettingsContainer _settingsContainer;
        private readonly ConfigContainer _configContainer;
        private readonly FenFromString _fenFromString;
        private readonly PlayerFactory _playerFactory;

        private IPlayer _playerWhite;
        private IPlayer _playerBlack;

        private bool isLoaded;

        private readonly CompositeDisposable _disposables = new();
        public Competitors(Game game, FenFromString fenFromString, PlayerFactory playerFactory,
            CoreEvents coreEvents, ISettingsContainer settingsContainer, ConfigContainer configContainer)
        {
            _game = game;
            _fenFromString = fenFromString;
            _playerFactory = playerFactory;
            _settingsContainer = settingsContainer;
            _configContainer = configContainer;

            coreEvents.OnEndMove.Subscribe(_ => SwapCurrentPlayer()).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public UniTask Load()
        {
            if (isLoaded)
            {
                return UniTask.CompletedTask;
            }

            _playerWhite = _playerFactory.Create(_settingsContainer.S.GameSettings.PlayerWhite.PlayerType,
                _configContainer.OnlineConfig.ConnectionType,
                PieceColor.White);

            _playerBlack = _playerFactory.Create(_settingsContainer.S.GameSettings.PlayerBlack.PlayerType,
                _configContainer.OnlineConfig.ConnectionType,
                PieceColor.Black);

            CurrentPlayer = _fenFromString.TurnColor == PieceColor.White ? _playerWhite : _playerBlack;

            isLoaded = true;
            return UniTask.CompletedTask;
        }

        public void StartPlayer()
        {
            if (!isLoaded)
            {
                Load(); // In case initial state is not warmup
            }

            CurrentPlayer.StartPlayer();
        }

        public void UpdatePlayer()
        {
            CurrentPlayer.UpdatePlayer();
        }

        public void StopPlayer()
        {
            CurrentPlayer.StopPlayer();
        }

        private void SwapCurrentPlayer()
        {
            if (_game.CurrentTurnColor == PieceColor.White)
            {
                CurrentPlayer = _playerWhite;
            }
            else if (_game.CurrentTurnColor == PieceColor.Black)
            {
                CurrentPlayer = _playerBlack;
            }
        }
    }
}