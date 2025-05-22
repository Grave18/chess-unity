using ChessGame.Logic;
using TMPro;
using UnityEngine;

namespace Ui.Game
{
    public class NotificationPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChessGame.Logic.Game game;

        [Header("Panels")]
        [SerializeField] private GameObject additionalPanel;

        [Header("Text")]
        [SerializeField] private TMP_Text notificationText;
        [SerializeField] private TMP_Text additionalText;

        private void OnEnable()
        {
            game.OnWarmup += UpdateNotificationText;
            game.OnStart += UpdateNotificationText;
            game.OnEndMove += UpdateNotificationText;
            game.OnEnd += UpdateNotificationText;
        }

        private void OnDisable()
        {
            game.OnWarmup -= UpdateNotificationText;
            game.OnStart -= UpdateNotificationText;
            game.OnEndMove -= UpdateNotificationText;
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
                case CheckType.Draw:
                    SetPanel(text: "Draw", additional: game.CheckDescription, isText: true, isPanel: true);
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
            additionalPanel.SetActive(isPanel);
            notificationText.text = text;
            additionalText.text = additional;
        }
    }
}