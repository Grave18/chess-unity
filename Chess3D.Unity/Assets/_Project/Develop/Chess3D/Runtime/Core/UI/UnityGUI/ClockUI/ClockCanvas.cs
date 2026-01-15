using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.MainCamera;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.ClockUI
{
    public sealed class ClockCanvas : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        [Header("References")]
        [SerializeField] private ClockView whiteClockView;
        [SerializeField] private ClockView blackClockView;
        [SerializeField] private CurrentPlayerPointerView whiteCurrentPlayerPointerView;
        [SerializeField] private CurrentPlayerPointerView blackCurrentPlayerPointerView;
        [SerializeField] private GameObject whiteClockRoot;
        [SerializeField] private GameObject blackClockRoot;

        [Inject]
        public void Construct(Game game, CoreEvents coreEvents, IClock clock, ISettingsContainer settingsContainer,
            [Key(CameraKeys.Ui)] Camera uiCamera)
        {
            canvas.worldCamera = uiCamera;

            if (settingsContainer.GameSettings.PlayerColor == PieceColor.White)
            {
                whiteCurrentPlayerPointerView.Construct(game, coreEvents);
                whiteClockView.Construct(coreEvents, clock);
                whiteClockRoot.SetActive(true);
                blackClockRoot.SetActive(false);
            }
            else
            {
                blackCurrentPlayerPointerView.Construct(game, coreEvents);
                blackClockView.Construct(coreEvents, clock);
                whiteClockRoot.SetActive(false);
                blackClockRoot.SetActive(true);
            }
        }
    }
}
