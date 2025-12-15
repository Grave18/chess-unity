using Chess3D.Runtime.Core.Logic;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI
{
    public class NotificationPanel : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject additionalPanel;

        [Header("Text")]
        [SerializeField] private TMP_Text notificationText;
        [SerializeField] private TMP_Text additionalText;

        private Game _game;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(Game game, CoreEvents coreEvents)
        {
            _game = game;

            coreEvents.OnWarmup.Subscribe(_ => UpdateNotificationText()).AddTo(_disposables);
            coreEvents.OnWarmup.Subscribe(_ => Warmup()).AddTo(_disposables);
            coreEvents.OnStart.Subscribe(_ => UpdateNotificationText()).AddTo(_disposables);
            coreEvents.OnIdle.Subscribe(_ => UpdateNotificationText()).AddTo(_disposables);
            coreEvents.OnEndMove.Subscribe(_ => UpdateNotificationText()).AddTo(_disposables);
            coreEvents.OnEnd.Subscribe(_ => UpdateNotificationText()).AddTo(_disposables);
            coreEvents.OnPause.Subscribe(_ => Pause()).AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables.Dispose();
        }

        private void UpdateNotificationText()
        {
            if (_game.IsCheck)
            {
                SetPanel(text: "Check", additional: "", isShowHeader: true, isShowAdditional: false);
            }
            else if (_game.IsCheckmate)
            {
                SetPanel(text: "Checkmate", _game.EndGameDescription, isShowHeader: true, isShowAdditional: true);
            }
            else if (_game.IsDraw)
            {
                SetPanel(text: "Draw", additional: _game.EndGameDescription, isShowHeader: true, isShowAdditional: true);
            }
            else if (_game.IsResign)
            {
                SetPanel(text: "Resign", additional: _game.EndGameDescription, isShowHeader: true, isShowAdditional: true);
            }
            else if (_game.IsTimeOut)
            {
                SetPanel(text: "Time is over",_game.EndGameDescription, isShowHeader: true, isShowAdditional: true);
            }
            else
            {
                SetPanel(text: "", "", isShowHeader: false, isShowAdditional: false);
            }
        }

        private void Pause()
        {
            SetPanel(text: "Pause", additional: "", isShowHeader: true, isShowAdditional: false);
        }

        private void Warmup()
        {
            SetPanel(text: "Warmup", additional: "", isShowHeader: true, isShowAdditional: false);
        }

        private void SetPanel(string text, string additional, bool isShowHeader, bool isShowAdditional)
        {
            notificationText.gameObject.SetActive(isShowHeader);
            additionalPanel.SetActive(isShowAdditional);
            notificationText.text = text;
            additionalText.text = additional;
        }
    }
}