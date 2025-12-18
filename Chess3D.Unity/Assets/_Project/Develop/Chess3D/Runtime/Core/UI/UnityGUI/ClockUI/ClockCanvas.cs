using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.MainCamera;
using Chess3D.Runtime.Core.Sound;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.ClockUI
{
    public class ClockCanvas : MonoBehaviour
    {
        [FormerlySerializedAs("clockPanel")] [SerializeField] private ClockView clockView;
        [FormerlySerializedAs("currentPlayerPointerPanel")] [SerializeField] private CurrentPlayerPointerView currentPlayerPointerView;
        [SerializeField] private Canvas canvas;

        [Inject]
        public void Construct(Game game, CoreEvents coreEvents, IClock clock, [Key(CameraKeys.Ui)]Camera uiCamera, EffectsPlayer effectsPlayer)
        {
            clockView.Construct(coreEvents, clock, effectsPlayer);
            currentPlayerPointerView.Construct(game, coreEvents);
            canvas.worldCamera = uiCamera;
        }
    }
}
