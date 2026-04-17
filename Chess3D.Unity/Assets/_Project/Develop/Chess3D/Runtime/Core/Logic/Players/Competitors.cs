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
        private readonly FenFromString _fenFromString;
        private readonly PlayerFactory _playerFactory;

        private IPlayer _playerWhite;
        private IPlayer _playerBlack;

        private bool isLoaded;

        private readonly CompositeDisposable _disposables = new();
        public Competitors(Game game, FenFromString fenFromString, PlayerFactory playerFactory,
            CoreEvents coreEvents, ISettingsContainer settingsContainer)
        {
            _game = game;
            _fenFromString = fenFromString;
            _playerFactory = playerFactory;
            _settingsContainer = settingsContainer;

            coreEvents.OnEndMove.Subscribe(_ => SwapCurrentPlayer()).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public async UniTask Load()
        {
            if (isLoaded)
            {
                return;
            }

            _playerWhite = await _playerFactory.Create(_settingsContainer.GameSettings.PlayerWhite.PlayerType);
            _playerBlack = await _playerFactory.Create(_settingsContainer.GameSettings.PlayerBlack.PlayerType);

            CurrentPlayer = _fenFromString.TurnColor == PieceColor.White ? _playerWhite : _playerBlack;

            isLoaded = true;
        }

        public async UniTask StartPlayer()
        {
            await Load(); // In case initial state is not warmup
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