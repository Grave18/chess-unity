using Chess3D.Runtime.Core.Logic.Players;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.LeftPanel
{
    public sealed class UndoRedoPauseView : MonoBehaviour
    {
        [Header("UI")]
        [FormerlySerializedAs("undo")]
        [SerializeField] private Button undoButton;
        [FormerlySerializedAs("redo")]
        [SerializeField] private Button redoButton;
        [FormerlySerializedAs("pause")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Image pauseImage;

        [Header("Assets")]
        [SerializeField] private Sprite playSprite;
        [SerializeField] private Sprite pauseSprite;

        private IGameStateMachine _gameStateMachine;

        private bool _isPause;

        [Inject]
        public void Construct(CoreEvents coreEvents, IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;

            coreEvents.OnWarmup.Subscribe(_ => Initialize()).AddTo(this);
            coreEvents.OnPlay.Subscribe(_ => OnPlay()).AddTo(this);
            coreEvents.OnPause.Subscribe(_ => OnPause()).AddTo(this);
            coreEvents.OnStart.Subscribe(_ => DisablePauseIfOnlineAndNotMoving()).AddTo(this);
            coreEvents.OnEnd.Subscribe(_ => DisablePauseIfOnlineAndNotMoving()).AddTo(this);
            undoButton.OnClickAsObservable().Subscribe(_ => _gameStateMachine.Undo()).AddTo(this);
            redoButton.OnClickAsObservable().Subscribe(_ => _gameStateMachine.Redo()).AddTo(this);
            pauseButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (_isPause) _gameStateMachine.Play();
                else _gameStateMachine.Pause();
            }).AddTo(this);
        }

        private void Initialize()
        {
            OnPlay();
            undoButton.interactable = true;
            redoButton.interactable = true;
        }

        private void DisablePauseIfOnlineAndNotMoving()
        {
            pauseButton.interactable = true;
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
    }
}
