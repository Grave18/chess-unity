using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.MainCamera;
using Chess3D.Runtime.Core.Sound;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.ClockUi
{
    public class ClockPanelCanvas : MonoBehaviour
    {
        [SerializeField] private ClockPanel clockPanel;
        [SerializeField] private CurrentPlayerPointerPanel currentPlayerPointerPanel;
        [SerializeField] private Canvas canvas;

        [Inject]
        public void Construct(Game game, CoreEvents coreEvents, IClock clock, [Key(CameraKeys.Ui)]Camera uiCamera, EffectsPlayer effectsPlayer)
        {
            clockPanel.Construct(coreEvents, clock, effectsPlayer);
            currentPlayerPointerPanel.Construct(game, coreEvents);
            canvas.worldCamera = uiCamera;
        }
    }
}
