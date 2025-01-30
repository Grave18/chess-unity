using Logic;
using UnityEngine;

public class NotificationPanel : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject notificationText;

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
        notificationText.SetActive(checkType != CheckType.None);
    }
}
