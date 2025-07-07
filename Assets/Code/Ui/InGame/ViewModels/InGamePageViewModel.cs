using GameAndScene;
using Ui.Auxiliary;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    public class InGamePageViewModel : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;

        public DelegateCommand ExitToMainMenuCommand { get; set; }

        private void Awake()
        {
            ExitToMainMenuCommand = new DelegateCommand(ExitToMainMenu);
        }

        private void ExitToMainMenu(object obj)
        {
            sceneLoader.LoadMainMenu();
        }
    }
}