using Chess3D.Runtime.LobbyManagement;
using LobbyManagement;
using Noesis;
using Ui.Auxiliary;
using Ui.Menu.Pages;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public class MainMenuViewModel : MonoBehaviour
    {
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private LobbyManager lobbyManager;

        public ImageSource ImageSource { get; private set; }

        private void Awake()
        {
            InitRenderTexture();
        }

        private void InitRenderTexture()
        {
            if(renderTexture)
            {
                ImageSource = new TextureSource(renderTexture);
            }
        }

        private void OnEnable()
        {
            lobbyManager.OnRoomJoined.AddListener(TryShowLobbyPage);
            lobbyManager.OnRoomJoinFailed.AddListener(ShowOnlinePage);
            lobbyManager.OnRoomLeft.AddListener(ShowOnlinePage);
        }

        private void OnDisable()
        {
            lobbyManager.OnRoomJoined.RemoveListener(TryShowLobbyPage);
            lobbyManager.OnRoomJoinFailed.RemoveListener(ShowOnlinePage);
            lobbyManager.OnRoomLeft.RemoveListener(ShowOnlinePage);
        }

        private void OnApplicationPause(bool isPause)
        {
            if (!isPause)
            {
                return;
            }

            TryShowLobbyPage();
        }

        private void TryShowLobbyPage(Lobby _ = default)
        {
            if (IsShowLobbyPage())
            {
                ShowLobbyPage();
            }
        }

        private bool IsShowLobbyPage()
        {
            return lobbyManager.IsInLobby
                   && !GameMenuBase.Instance.IsCurrentPage<LobbyPage>();
        }

        private static void ShowLobbyPage()
        {
            GameMenuBase.Instance.ChangePage<LobbyPage>();
        }

        private void ShowOnlinePage(string _)
        {
            ShowOnlinePage();
        }

        private void ShowOnlinePage()
        {
            GameMenuBase.Instance.ChangePage<PlayPage>("OnlineTab");
        }
    }
}