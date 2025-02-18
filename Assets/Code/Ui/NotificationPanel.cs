using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Ui
{
    public class NotificationPanel : MonoBehaviour
    {
        [FormerlySerializedAs("gameManager")]
        [SerializeField] private Game game;
        [SerializeField] private TMP_Text notificationText;
        [SerializeField] private Button restartButton;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(() => _ = game.RestartAsync());

            game.OnEndTurn += UpdateNotificationText;
        }

        private void OnDisable()
        {
            game.OnEndTurn -= UpdateNotificationText;
        }

        private void UpdateNotificationText(PieceColor pieceColor, CheckType checkType)
        {
            switch (checkType)
            {
                case CheckType.None:
                    SetPanel(text: "", isText: false, isButton: false);
                    break;
                case CheckType.Check:
                    SetPanel(text: "Check", isText: true, isButton: false);
                    break;
                case CheckType.DoubleCheck:
                    SetPanel(text: "Double check", isText: true, isButton: false);
                    break;
                case CheckType.CheckMate:
                    SetPanel(text: "Checkmate", isText: true, isButton: true);
                    break;
                case CheckType.Stalemate:
                    SetPanel(text: "Stalemate", isText: true, isButton: true);
                    break;
                default:
                    SetPanel(text: "", isText: false, isButton: false);
                    break;
            }
        }

        private void SetPanel(string text, bool isText, bool isButton)
        {
            notificationText.gameObject.SetActive(isText);
            restartButton.gameObject.SetActive(isButton);
            notificationText.text = text;
        }
    }
}