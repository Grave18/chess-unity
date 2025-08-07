using UnityEngine;

namespace Ui.Game
{
    public class MenuButtonPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Logic.Game game;

        [Header("Ui")]
        [SerializeField] private GameObject restartButton;

        private void Awake()
        {
            game.OnWarmup += Disable;
            game.OnEndMove += Disable;
            game.OnEnd += Enable;
        }

        private void OnDestroy()
        {
            game.OnWarmup -= Disable;
            game.OnEndMove -= Disable;
            game.OnEnd -= Enable;
        }

        private void Enable()
        {
            restartButton.SetActive(true);
        }

        private void Disable()
        {
            restartButton.SetActive(false);
        }
    }
}