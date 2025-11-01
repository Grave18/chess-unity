using Chess3D.Runtime.Logic;
using TMPro;
using UnityEngine;

namespace Chess3D.Runtime.UnityUi.InGame
{
    public class NotificationPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Game game;

        [Header("Panels")]
        [SerializeField] private GameObject additionalPanel;

        [Header("Text")]
        [SerializeField] private TMP_Text notificationText;
        [SerializeField] private TMP_Text additionalText;

        private void OnEnable()
        {
            game.OnWarmup += UpdateNotificationText;
            game.OnStart += UpdateNotificationText;
            game.OnIdle += UpdateNotificationText;
            game.OnEndMove += UpdateNotificationText;
            game.OnEnd += UpdateNotificationText;
            game.OnPause += Pause;
            game.OnWarmup += Warmup;
        }

        private void OnDisable()
        {
            game.OnWarmup -= UpdateNotificationText;
            game.OnStart -= UpdateNotificationText;
            game.OnIdle -= UpdateNotificationText;
            game.OnEndMove -= UpdateNotificationText;
            game.OnEnd -= UpdateNotificationText;
            game.OnPause -= Pause;
            game.OnWarmup -= Warmup;
        }

        private void UpdateNotificationText()
        {
            if (game.IsCheck)
            {
                SetPanel(text: "Check", additional: "", isShowHeader: true, isShowAdditional: false);
            }
            else if (game.IsCheckmate)
            {
                SetPanel(text: "Checkmate", game.EndGameDescription, isShowHeader: true, isShowAdditional: true);
            }
            else if (game.IsDraw)
            {
                SetPanel(text: "Draw", additional: game.EndGameDescription, isShowHeader: true, isShowAdditional: true);
            }
            else if (game.IsResign)
            {
                SetPanel(text: "Resign", additional: game.EndGameDescription, isShowHeader: true, isShowAdditional: true);
            }
            else if (game.IsTimeOut)
            {
                SetPanel(text: "Time is over",game.EndGameDescription, isShowHeader: true, isShowAdditional: true);
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