using PurrNet;
using TMPro;
using UnityEngine;

namespace Ui.Game
{
    public class StateDebugPanel : MonoBehaviour
    {
        [SerializeField] private ChessGame.Logic.Game game;

        [SerializeField] private TMP_Text authorityText;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text checkText;

        private void Update()
        {
            string authority = "Offline";
            if (InstanceHandler.NetworkManager != null)
            {
                authority = InstanceHandler.NetworkManager.isServer ? "Server" : "Client";
            }
            authorityText.text = "Authority: " + authority;
            stateText.text = "State: " + game.Machine.StateName;
            turnText.text = "Turn: " + game.CurrentTurnColor;
            checkText.text = "Check: " + game.CheckType;
        }
    }
}
