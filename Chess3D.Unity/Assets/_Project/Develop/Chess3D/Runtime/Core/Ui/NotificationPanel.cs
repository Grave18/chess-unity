using Chess3D.Runtime.Core.Logic;
using TMPro;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Ui
{
    public class NotificationPanel : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject additionalPanel;

        [Header("Text")]
        [SerializeField] private TMP_Text notificationText;
        [SerializeField] private TMP_Text additionalText;

        private Game _game;
        private CoreEvents _coreEvents;

        [Inject]
        private void Construct(Game game, CoreEvents coreEvents)
        {
            _game = game;
            _coreEvents = coreEvents;

            _coreEvents.OnWarmup += UpdateNotificationText;
            _coreEvents.OnStart += UpdateNotificationText;
            _coreEvents.OnIdle += UpdateNotificationText;
            _coreEvents.OnEndMove += UpdateNotificationText;
            _coreEvents.OnEnd += UpdateNotificationText;
            _coreEvents.OnPause += Pause;
            _coreEvents.OnWarmup += Warmup;
        }

        private void OnDisable()
        {
            if (_coreEvents is null)
            {
                return;
            }

            _coreEvents.OnWarmup -= UpdateNotificationText;
            _coreEvents.OnStart -= UpdateNotificationText;
            _coreEvents.OnIdle -= UpdateNotificationText;
            _coreEvents.OnEndMove -= UpdateNotificationText;
            _coreEvents.OnEnd -= UpdateNotificationText;
            _coreEvents.OnPause -= Pause;
            _coreEvents.OnWarmup -= Warmup;
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