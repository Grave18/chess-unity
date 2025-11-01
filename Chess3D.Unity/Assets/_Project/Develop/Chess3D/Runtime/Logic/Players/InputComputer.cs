using System;
using System.Threading;
using Chess3D.Runtime.Ai;
using Chess3D.Runtime.Settings;
using UnityEngine;

namespace Chess3D.Runtime.Logic.Players
{
    public class InputComputer : IInputHandler
    {
        private readonly Game _game;
        private readonly Stockfish _stockfish;
        private readonly PlayerSettings _playerSettings;

        private CancellationTokenSource _moveCts = new();

        public InputComputer(Game game, PlayerSettings playerSettings, Stockfish stockfish)
        {
            _game = game;
            _playerSettings = playerSettings;
            _stockfish = stockfish;
        }

        public async void StartInput()
        {
            string uci;
            try
            {
                 uci = await _stockfish.GetUci(_playerSettings, _moveCts.Token);
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

            _game.GameStateMachine.Move(uci);
        }

        public void UpdateInput()
        {
            // Noop
        }

        public void StopInput()
        {
            _moveCts?.Cancel();
            _moveCts = new CancellationTokenSource();
        }
    }
}