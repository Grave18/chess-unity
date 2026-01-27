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
        [SerializeField] private GameObject whiteClockRoot;
        [SerializeField] private ClockView whiteClockView;
        [SerializeField] private CurrentPlayerPointerView whiteCurrentPlayerPointerView;

        [SerializeField] private GameObject blackClockRoot;
        [SerializeField] private ClockView blackClockView;
        [SerializeField] private CurrentPlayerPointerView blackCurrentPlayerPointerView;

        [Inject]
        public void Construct(Game game, CoreEvents coreEvents, IClock clock, ISettingsContainer settingsContainer,
            [Key(CameraKeys.Ui)] Camera uiCamera)
        {
            canvas.worldCamera = uiCamera;

            if (settingsContainer.GameSettings.PlayerColor == PieceColor.White)
            {
                whiteClockRoot.SetActive(true);
                whiteCurrentPlayerPointerView.Construct(game, coreEvents);
                whiteClockView.Construct(coreEvents, clock);

                blackClockRoot.SetActive(false);
            }
            else
            {
                blackClockRoot.SetActive(true);
                blackCurrentPlayerPointerView.Construct(game, coreEvents);
                blackClockView.Construct(coreEvents, clock);

                whiteClockRoot.SetActive(false);
            }
        }
    }
}
