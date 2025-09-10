using Logic;
using UnityEngine;

namespace UnityUi.InGame
{
    public class ClockPanelCanvas : MonoBehaviour
    {
        [SerializeField] private ClockPanel clockPanel;
        [SerializeField] private CurrentPlayerPointerPanel currentPlayerPointerPanel;
        [SerializeField] private Canvas canvas;

        public void Init(Game game, IClock clock, Camera uiCamera)
        {
            clockPanel.Init(game, clock);
            currentPlayerPointerPanel.Init(game);
            canvas.worldCamera = uiCamera;
        }
    }
}
