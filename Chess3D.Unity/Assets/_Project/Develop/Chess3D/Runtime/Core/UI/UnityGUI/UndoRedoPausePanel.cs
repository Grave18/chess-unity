using System;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Online;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI
{
    public class UndoRedoPausePanel : MonoBehaviour, ILoadUnit, IDisposable
    {
        [Header("UI")]
        [SerializeField] private Button undo;
        [SerializeField] private Button redo;
        [SerializeField] private Button pause;
        [SerializeField] private Image pauseImage;

        [Header("Assets")]
        [SerializeField] private Sprite playSprite;
        [SerializeField] private Sprite pauseSprite;

        private Game _game;
        private ISettingsContainer settingsContainer;
        private IGameStateMachine _gameStateMachine;

        private bool _isPause;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Construct(Game game, CoreEvents coreEvents, ISettingsContainer settingsContainer, IGameStateMachine gameStateMachine)
        {
            _game = game;
            this.settingsContainer = settingsContainer;
            _gameStateMachine = gameStateMachine;

            coreEvents.OnPlay.Subscribe(_ => OnPlay()).AddTo(_disposables);
            coreEvents.OnPause.Subscribe(_ => OnPause()).AddTo(_disposables);
            coreEvents.OnStart.Subscribe(_ => DisablePauseIfOnlineAndNotMoving()).AddTo(_disposables);
            coreEvents.OnEnd.Subscribe(_ => DisablePauseIfOnlineAndNotMoving()).AddTo(_disposables);
            undo.onClick.AddListener(OnUndoPressed);
            redo.onClick.AddListener(OnRedoPressed);
            pause.onClick.AddListener(OnPlayPausePressed);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            undo.onClick.RemoveListener(OnUndoPressed);
            redo.onClick.RemoveListener(OnRedoPressed);
            pause.onClick.RemoveListener(OnPlayPausePressed);
        }

        public UniTask Load()
        {
            OnPlay();
            if (OnlineInstanceHandler.IsOnline)
            {
                undo.interactable = false;
                redo.interactable = false;
            }
            else
            {
                undo.interactable = true;
                redo.interactable = true;
            }
            return UniTask.CompletedTask;
        }

        private void DisablePauseIfOnlineAndNotMoving()
        {
            if (OnlineInstanceHandler.IsOnline)
            {
                PieceColor playerColor = settingsContainer.S.GameSettings.PlayerColor;
                pause.interactable = playerColor == _game.CurrentTurnColor;
            }
            else
            {
                pause.interactable = true;
            }
        }


        private void OnPlay()
        {
            pauseImage.sprite = pauseSprite;
            _isPause = false;
        }

        private void OnPause()
        {
            pauseImage.sprite = playSprite;
            _isPause = true;
        }

        private void OnUndoPressed()
        {
            _gameStateMachine.Undo();
        }

        private void OnRedoPressed()
        {
            _gameStateMachine.Redo();
        }

        private void OnPlayPausePressed()
        {
            if (_isPause)
            {
                _gameStateMachine.Play();
            }
            else
            {
                _gameStateMachine.Pause();
            }
        }
    }
}
