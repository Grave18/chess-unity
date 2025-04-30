using PurrLobby;
using Ui.MainMenu;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LobbyManager lobbyManager;

    [Header("Panels")]
    [SerializeField] private MenuPanel lobbyPanel;
    [SerializeField] private MenuPanel onlinePanel;

    public MenuPanel CurrentPanel { get; private set; }

    private void Awake()
    {
        FindOpenedPanel();
    }

    private void FindOpenedPanel()
    {
        MenuPanel[] panels = FindObjectsByType<MenuPanel>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (MenuPanel panel in panels)
        {
            SetCurrentPanel(panel);
        }
    }

    public void SetCurrentPanel(MenuPanel panel)
    {
        CurrentPanel?.Hide();
        CurrentPanel = panel;
    }

    private void OnEnable()
    {
        lobbyManager.OnRoomJoined.AddListener(ShowLobbyPanel);
        lobbyManager.OnRoomJoinFailed.AddListener(ShowOnlinePanel);
        lobbyManager.OnRoomLeft.AddListener(ShowOnlinePanel);
    }

    private void OnDisable()
    {
        lobbyManager.OnRoomJoined.RemoveListener(ShowLobbyPanel);
        lobbyManager.OnRoomJoinFailed.RemoveListener(ShowOnlinePanel);
        lobbyManager.OnRoomLeft.RemoveListener(ShowOnlinePanel);
    }

    // Show lobby panel when go back to game
    private void OnApplicationPause(bool pauseStatus)
    {
        if (IsShowLobbyPanel(pauseStatus))
        {
            ShowLobbyPanel(default);
        }
    }

    private bool IsShowLobbyPanel(bool pauseStatus)
    {
        return lobbyManager.IsInLobby && CurrentPanel != lobbyPanel && !pauseStatus;
    }

    private void ShowLobbyPanel(Lobby lobby)
    {
        lobbyPanel.Show();
    }

    private void ShowOnlinePanel()
    {
        ShowOnlinePanel(null);
    }

    private void ShowOnlinePanel(string arg)
    {
        onlinePanel.Show();
    }
}