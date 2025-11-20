using System;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [Preserve]
    public sealed class Competitors : ILoadUnit, IDisposable
    {
        public IPlayer CurrentPlayer { get; private set; }

        private readonly Game _game;
        private readonly FenFromString _fenFromString;
        private readonly Func<IPlayer> _playerFactory;
        private readonly CoreEvents _coreEvents;

        private IPlayer _playerWhite;
        private IPlayer _playerBlack;

        public Competitors(Game game, FenFromString fenFromString, Func<IPlayer> playerFactory, CoreEvents coreEvents)
        {
            _game = game;
            _fenFromString = fenFromString;
            _playerFactory = playerFactory;
            _coreEvents = coreEvents;

            _coreEvents.OnEndMove += SwapCurrentPlayer;
        }

        public void Dispose()
        {
            if (_coreEvents is null)
            {
                return;
            }

            _coreEvents.OnEndMove -= SwapCurrentPlayer;
        }

        public UniTask Load()
        {
            _playerWhite = _playerFactory(); // TODO: send config as argument
            _playerBlack = _playerFactory();
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