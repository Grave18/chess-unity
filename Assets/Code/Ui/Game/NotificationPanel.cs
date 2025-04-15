using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Game
{
    public class NotificationPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Logic.Game game;

        [Header("UI")]
        [SerializeField] private GameObject buttonPanel;
        [SerializeField] private TMP_Text notificationText;
        [SerializeField] private TMP_Text additionalText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button homeButton;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(game.StartGame);
            game.OnEndMove += UpdateNotificationText;
            game.OnStart += UpdateNotificationText;
            game.OnEnd += UpdateNotificationText;
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(game.StartGame);
            game.OnEndMove -= UpdateNotificationText;
            game.OnStart -= UpdateNotificationText;
            game.OnEnd -= UpdateNotificationText;
        }

        private void UpdateNotificationText()
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
                    ProcessCheckmate(game.CurrentTurnColor);
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

        private void ProcessTimeOut(CheckType checkType)
        {
            string additional = checkType switch
            {
                CheckType.TimeOutWhite => "White runs out of time",
                CheckType.TimeOutBlack => "Black runs out of time",
                _ => "Invalid time out",
            };

            SetPanel(text: "Time is over", additional, isText: true, isPanel: true);
        }

        private void ProcessCheckmate(PieceColor currentColor)
        {
            string additional = currentColor switch
            {
                PieceColor.Black => "White wins this game",
                PieceColor.White => "Black wins this game",
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