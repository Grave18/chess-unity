using System.Collections;
using MvvmTool;
using UnityEngine;
using UtilsCommon;

namespace LobbyManagement
{
    [INotifyPropertyChanged]
    public partial class FriendItem
    {
        public string Id {get; set;}
        public string Name {get; set;}
        public Texture2D Image {get; set;}

        private bool _canInvite = true;
        private readonly WaitForSecondsRealtime _waitForSecondsRealtime = new(3);

        [RelayCommand(CanExecute = nameof(FriendClick_CanExecute))]
        private void FriendClick()
        {
            Debug.Log($"Friend clicked {Name}, {Id}");
            DisableFriendInvite().RunCoroutine();
        }

        private bool FriendClick_CanExecute()
        {
            return _canInvite;
        }

        private IEnumerator DisableFriendInvite()
        {
            _canInvite = false;
            FriendClickCommand.NotifyCanExecuteChanged();

            yield return _waitForSecondsRealtime;

            _canInvite = true;
            FriendClickCommand.NotifyCanExecuteChanged();
        }


    }
}
