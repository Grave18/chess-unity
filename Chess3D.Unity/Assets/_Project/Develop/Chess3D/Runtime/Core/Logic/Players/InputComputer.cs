using System;
using System.Threading;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.AI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.Logic.Players
{
    [Preserve]
    public sealed class InputComputer : IInputHandler
    {
        private readonly Stockfish _stockfish;
        private readonly ISettingsContainer settingsContainer;
        private readonly IGameStateMachine _gameStateMachine;

        private CancellationTokenSource _moveCts = new();

        public InputComputer(Stockfish stockfish, ISettingsContainer settingsContainer, IGameStateMachine gameStateMachine)
        {
            this.settingsContainer = settingsContainer;
            _stockfish = stockfish;
            _gameStateMachine = gameStateMachine;
        }

        public async UniTask Load()
        {
            await _stockfish.Load();
        }

        public async void StartInput()
        {
            string uci;
            try
            {
                PlayerSettings player2Settings = settingsContainer.S.GameSettings.PlayerBlack;
                uci = await _stockfish.GetUci(player2Settings, _moveCts.Token);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return;
            }

            if (uci == null)
            {
                return;
            }

            _gameStateMachine.Move(uci);
        }

        public void StopInput()
        {
            _moveCts?.Cancel();
            _moveCts = new CancellationTokenSource();
        }

        public void UpdateInput()
        {
            // Noop
        }
    }
}