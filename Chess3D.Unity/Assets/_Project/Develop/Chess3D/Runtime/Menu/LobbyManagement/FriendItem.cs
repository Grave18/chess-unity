using System.Collections;
using Chess3D.Runtime.Utilities.Common;
using LobbyManagement;
using MvvmTool;
using UnityEngine;

namespace Chess3D.Runtime.Menu.LobbyManagement
{
    [INotifyPropertyChanged]
    public partial class FriendItem
    {
        public string Id {get; set;}
        public string Name {get; set;}
        public Texture2D Image {get; set;}

        private bool _canInvite = true;
        private readonly WaitForSecondsRealtime _waitForSecondsRealtime = new(3);

        private LobbyManager _lobbyManager;

        public void Init(LobbyManager lobbyManager)
        {
            _lobbyManager = lobbyManager;
        }

        [RelayCommand(CanExecute = nameof(FriendClick_CanExecute))]
        private void FriendClick()
        {
            TemporaryDisableFriendInviteRoutine().RunCoroutine();

            Invite();
        }

        private void Invite()
        {
            _lobbyManager?.InviteFriend(this);
        }

        private bool FriendClick_CanExecute()
        {
            return _canInvite;
        }

        private IEnumerator TemporaryDisableFriendInviteRoutine()
        {
            _canInvite = false;
            FriendClickCommand.NotifyCanExecuteChanged();

            yield return _waitForSecondsRealtime;

            _canInvite = true;
            FriendClickCommand.NotifyCanExecuteChanged();
        }
    }
}
