using System;
using System.Collections.Generic;
using PurrLobby;
using UnityEngine;

namespace UnityUi.Menu.Lobby
{
    public class LobbyMemberList : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private MemberEntry memberEntryPrefab;
        [SerializeField] private Transform content;

        private void OnEnable()
        {
            lobbyManager.OnRoomLeft.AddListener(OnLobbyLeave);
            lobbyManager.OnRoomUpdated.AddListener(LobbyDataUpdate);
        }

        private void OnDisable()
        {
            lobbyManager.OnRoomLeft.RemoveListener(OnLobbyLeave);
            lobbyManager.OnRoomUpdated.RemoveListener(LobbyDataUpdate);
        }

        private void LobbyDataUpdate(PurrLobby.Lobby room)
        {
            if(!room.IsValid)
            {
                return;
            }

            HandleExistingMembers(room);
            HandleNewMembers(room);
            HandleLeftMembers(room);
        }

        private void OnLobbyLeave()
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
        }

        private void HandleExistingMembers(PurrLobby.Lobby room)
        {
            foreach (Transform child in content)
            {
                if (!child.TryGetComponent(out MemberEntry member))
                {
                    continue;
                }

                LobbyUser matchingMember = room.Members.Find((LobbyUser x) => x.Id == member.MemberId);
                if (!string.IsNullOrEmpty(matchingMember.Id))
                {
                    member.SetReady(matchingMember.IsReady);
                }
            }
        }

        private void HandleNewMembers(PurrLobby.Lobby room)
        {
            var existingMembers = content.GetComponentsInChildren<MemberEntry>();

            foreach (LobbyUser member in room.Members)
            {
                if (Array.Exists(existingMembers, x => x.MemberId == member.Id))
                {
                    continue;
                }

                MemberEntry entry = Instantiate(memberEntryPrefab, content);
                entry.Init(member);
            }
        }

        private void HandleLeftMembers(PurrLobby.Lobby room)
        {
            var childrenToRemove = new List<Transform>();

            for (int i = 0; i < content.childCount; i++)
            {
                Transform child = content.GetChild(i);
                if (!child.TryGetComponent(out MemberEntry member))
                {
                    continue;
                }

                if (!room.Members.Exists(x => x.Id == member.MemberId))
                {
                    childrenToRemove.Add(child);
                }
            }

            foreach (Transform child in childrenToRemove)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
