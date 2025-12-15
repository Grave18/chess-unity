using System;
using Chess3D.Runtime.Bootstrap.Settings;
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
        private readonly ISettingsService _settingsService;
        private readonly FenFromString _fenFromString;
        private readonly PlayerFactory _playerFactory;

        private IPlayer _playerWhite;
        private IPlayer _playerBlack;

        private readonly CompositeDisposable _disposables = new();
        public Competitors(Game game, FenFromString fenFromString, PlayerFactory playerFactory, CoreEvents coreEvents, ISettingsService settingsService)
        {
            _game = game;
            _fenFromString = fenFromString;
            _playerFactory = playerFactory;
            _settingsService = settingsService;

            coreEvents.OnEndMove.Subscribe(_ => SwapCurrentPlayer()).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public UniTask Load()
        {
            _playerWhite = _playerFactory.Create(_settingsService.S.GameSettings.PlayerWhite.PlayerType);
            _playerBlack = _playerFactory.Create(_settingsService.S.GameSettings.PlayerBlack.PlayerType);
            CurrentPlayer = _fenFromString.TurnColor == PieceColor.White ? _playerWhite : _playerBlack;

            return UniTask.CompletedTask;
        }

        public void StartPlayer()
        {
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