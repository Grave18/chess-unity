using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class NotificationPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Game game;

        [Header("UI")]
        [SerializeField] private GameObject buttonPanel;
        [SerializeField] private TMP_Text notificationText;
        [SerializeField] private TMP_Text additionalText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button homeButton;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(game.StartGame);
            game.OnChangeTurn += UpdateNotificationText;
            game.OnStart += UpdateNotificationText;
            game.OnEnd += UpdateNotificationText;
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(game.StartGame);
            game.OnChangeTurn -= UpdateNotificationText;
            game.OnStart -= UpdateNotificationText;
            game.OnEnd -= UpdateNotificationText;
        }

        private void UpdateNotificationText()
        {
            UpdateNotificationText(game.PreviousTurnColor);
        }

        private void UpdateNotificationText(PieceColor color)
        {
            switch (game.CheckType)
            {
                case CheckType.None:
                    SetPanel(text: "", additional: "", isText: false, isPanel: false);
                    break;
                case CheckType.Check:
                    SetPanel(text: "Check", additional: "", isText: true, isPanel: false);
                    break;
                case CheckType.DoubleCheck:
                    SetPanel(text: "Double check", additional: "", isText: true, isPanel: false);
                    break;
                case CheckType.CheckMate:
                    ProcessCheckmate(color);
                    break;
                case CheckType.Stalemate:
                    SetPanel(text: "Stalemate", additional: "", isText: true, isPanel: true);
                    break;
                case CheckType.TimeOutWhite or CheckType.TimeOutBlack:
                    ProcessTimeOut(game.CheckType);
                    break;
                default:
                    SetPanel(text: "", "", isText: false, isPanel: false);
                    break;
            }
        }

        private void ProcessTimeOut(CheckType color)
        {
            string additional = color switch
            {
                CheckType.TimeOutWhite => "White runs out of time",
                CheckType.TimeOutBlack => "Black runs out of time",
                _ => "Invalid time out",
            };

            SetPanel(text: "Time is over", additional, isText: true, isPanel: true);
        }

        private void ProcessCheckmate(PieceColor color)
        {
            string additional = color switch
            {
                PieceColor.White => "White wins this game",
                PieceColor.Black => "Black wins this game",
                _ => "Invalid color"
            };

            SetPanel(text: "Checkmate", additional, isText: true, isPanel: true);
        }

        private void SetPanel(string text, string additional, bool isText, bool isPanel)
        {
            notificationText.gameObject.SetActive(isText);
            buttonPanel.SetActive(isPanel);
            notificationText.text = text;
            additionalText.text = additional;
        }
    }
}