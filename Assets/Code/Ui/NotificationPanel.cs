using Logic;
using TMPro;
using UnityEngine;

namespace Ui
{
    public class NotificationPanel : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TMP_Text notificationText;

        private GameObject _textGo;
        private void Awake()
        {
            _textGo = notificationText.gameObject;
        }

        private void OnEnable()
        {
            gameManager.OnTurnChanged += UpdateNotificationText;
        }

        private void OnDisable()
        {
            gameManager.OnTurnChanged -= UpdateNotificationText;
        }

        private void UpdateNotificationText(PieceColor pieceColor, CheckType checkType)
        {
            switch (checkType)
            {
                case CheckType.Check:
                    _textGo.SetActive(true);
                    notificationText.text = "Check";
                    break;
                case CheckType.DoubleCheck:
                    _textGo.SetActive(true);
                    notificationText.text = "Double Check";
                    break;
                case CheckType.CheckMate:
                    _textGo.SetActive(true);
                    notificationText.text = "Check Mate";
                    break;
                case CheckType.None:
                    notificationText.text = "";
                    _textGo.SetActive(false);
                    break;
            }
        }
    }
}