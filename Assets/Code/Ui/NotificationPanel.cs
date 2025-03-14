using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class NotificationPanel : MonoBehaviour
    {
        [SerializeField] private Game game;
        [SerializeField] private TMP_Text notificationText;
        [SerializeField] private Button restartButton;

        private void UpdateNotificationText()
        {
            switch (game.CheckType)
            {
                case CheckType.None:        SetPanel(text: "", isText: false, isButton: false);            break;
                case CheckType.Check:       SetPanel(text: "Check", isText: true, isButton: false);        break;
                case CheckType.DoubleCheck: SetPanel(text: "Double check", isText: true, isButton: false); break;
                case CheckType.CheckMate:   SetPanel(text: "Checkmate", isText: true, isButton: true);     break;
                case CheckType.Stalemate:   SetPanel(text: "Stalemate", isText: true, isButton: true);     break;
                case CheckType.TimeOut:     SetPanel(text: "Time is over", isText: true, isButton: true);  break;
                default:                    SetPanel(text: "", isText: false, isButton: false);            break;
            }
        }

        private void SetPanel(string text, bool isText, bool isButton)
        {
            notificationText.gameObject.SetActive(isText);
            restartButton.gameObject.SetActive(isButton);
            notificationText.text = text;
        }

        private void OnEnable()
        {
            restartButton.onClick.AddListener(game.StartGame);

            game.OnEndTurn += UpdateNotificationText;
            game.OnStart += UpdateNotificationText;
            game.OnEnd += UpdateNotificationText;
        }

        private void OnDisable()
        {
            game.OnEndTurn -= UpdateNotificationText;
            game.OnStart -= UpdateNotificationText;
            game.OnEnd -= UpdateNotificationText;
        }
    }
}