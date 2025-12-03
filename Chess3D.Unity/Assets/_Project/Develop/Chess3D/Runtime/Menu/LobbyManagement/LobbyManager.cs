using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chess3D.Runtime.Menu.LobbyManagement.Providers;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Logging;
using Cysharp.Threading.Tasks;
using PurrNet;
using UnityEngine;
using UnityEngine.Events;

namespace Chess3D.Runtime.Menu.LobbyManagement
{
    public class LobbyManager : MonoBehaviour, ILoadUnit, IDisposable
    {
        private const int PlayersToStart = 2;

        public CreateRoomArgs createRoomArgs = new();
        public SerializableDictionary<string, string> searchRoomArgs = new();
        [SerializeField] private MonoBehaviour currentProvider;

        private ILobbyProvider _currentProvider;
        private readonly Queue<Action> _delayedActions = new();
        private int _taskLock;
        private LobbyDataHolder _lobbyDataHolder;
        private Lobby _lastKnownState;
        private bool _isStarting;

        // Events exposed by the manager
        public UnityEvent<Lobby> OnRoomJoined = new();
        public UnityEvent<string> OnRoomJoinFailed = new();
        public UnityEvent OnRoomLeft = new();
        public UnityEvent<Lobby> OnRoomUpdated = new();
        public UnityEvent<List<LobbyUser>> OnPlayerListUpdated = new();
        public UnityEvent<List<Lobby>> OnRoomSearchResults = new();
        public UnityEvent<List<FriendItem>> OnFriendListPulled = new();
        public UnityEvent OnAllReady = new();
        public UnityEvent<string> OnError = new();

        public UnityEvent onInitialized = new();
        public UnityEvent onShutdown = new();

        private Lobby _oldLobby;

        public Lobby CurrentLobby
        {
            get => _lobbyDataHolder ? _lobbyDataHolder.CurrentLobby : default;
            private set => _lobbyDataHolder.SetCurrentLobby(value);
        }

        public bool IsInLobby => CurrentLobby.IsValid;
        public ILobbyProvider CurrentProvider => currentProvider as ILobbyProvider;

        public UniTask Load()
        {
            _lastKnownState = new Lobby { IsValid = false };

            if (CurrentProvider != null)
            {
                SetProvider(CurrentProvider);
            }
            else
            {
                Log.Lobby.W("No lobby provider assigned to LobbyManager.");
            }

            SetupDataHolder();

            return UniTask.CompletedTask;
        }

        private void SetupDataHolder()
        {
            _lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
            if (!_lobbyDataHolder)
            {
                var newObject = new GameObject("LobbyDataHolder");
                _lobbyDataHolder = newObject.AddComponent<LobbyDataHolder>();
                return;
            }

            if (_lobbyDataHolder.CurrentLobby.IsValid)
            {
                LeaveLobby(_lobbyDataHolder.CurrentLobby.LobbyId);
                _lobbyDataHolder.SetCurrentLobby(default);
            }
        }

        public void Dispose()
        {
            _currentProvider = null;
            _lobbyDataHolder = null;
        }

        private void Update()
        {
            while (_delayedActions.Count > 0)
            {
                _delayedActions.Dequeue()?.Invoke();
            }
        }

        /// <summary>
        /// Set or switch the current provider
        /// </summary>
        /// <param name="provider"> </param>
        public void SetProvider(ILobbyProvider provider)
        {
            if (_currentProvider != null)
            {
                UnsubscribeFromProviderEvents();
                _currentProvider.Shutdown();
            }

            _currentProvider = provider;

            if (_currentProvider != null)
            {
                SubscribeToProviderEvents();
                RunTask(async () =>
                {
                    await _currentProvider.InitializeAsync();
                    InvokeDelayed(() => onInitialized?.Invoke());
                });
            }
        }

        // Unsubscribe from provider events
        private void UnsubscribeFromProviderEvents()
        {
            _currentProvider.OnError -= OnCurrentProviderOnError;
            _currentProvider.OnLobbyPlayerListUpdated -= OnCurrentProviderOnLobbyPlayerListUpdated;
            _currentProvider.OnLobbyUpdated -= OnCurrentProviderOnLobbyUpdated;
            _currentProvider.OnLobbyLeft -= OnCurrentProviderOnLobbyLeft;
            _currentProvider.OnLobbyJoinFailed -= OnCurrentProviderOnLobbyJoinFailed;
        }

        // Subscribe to provider events
        private void SubscribeToProviderEvents()
        {
            _currentProvider.OnError += OnCurrentProviderOnError;
            _currentProvider.OnLobbyPlayerListUpdated += OnCurrentProviderOnLobbyPlayerListUpdated;
            _currentProvider.OnLobbyUpdated += OnCurrentProviderOnLobbyUpdated;
            _currentProvider.OnLobbyLeft += OnCurrentProviderOnLobbyLeft;
            _currentProvider.OnLobbyJoinFailed += OnCurrentProviderOnLobbyJoinFailed;
        }

        private void OnCurrentProviderOnError(string error)
        {
            InvokeDelayed(() => OnError.Invoke(error));
        }

        private void OnCurrentProviderOnLobbyPlayerListUpdated(List<LobbyUser> players)
        {
            InvokeDelayed(() => OnPlayerListUpdated.Invoke(players));
        }

        private void OnCurrentProviderOnLobbyUpdated(Lobby lobby)
        {
            InvokeDelayed(() =>
            {
                if (!_lastKnownState.HasChanged(lobby) || lobby.Members.Count <= 0 || !lobby.IsValid)
                {
                    return;
                }

                if (lobby.IsValid && lobby.Members.Count != _oldLobby.Members?.Count)
                {
                    _oldLobby = lobby;
                    OnRoomJoined?.Invoke(lobby);
                }

                _lastKnownState = lobby;
                CurrentLobby = lobby;
                OnRoomUpdated?.Invoke(lobby);

                if (!_isStarting && lobby.Members.TrueForAll(x => x.IsReady))
                {
                    _isStarting = true; //Prevent calling ready again if lobby is updated after all ready
                    CallOnAllReady();
                }
            });
        }

        private void OnCurrentProviderOnLobbyLeft()
        {
            InvokeDelayed(() =>
            {
                _oldLobby = default;
                CurrentLobby = default;
                OnRoomLeft?.Invoke();
            });
        }

        private void OnCurrentProviderOnLobbyJoinFailed(string message)
        {
            InvokeDelayed(() => OnRoomJoinFailed.Invoke(message));
        }

        private void InvokeDelayed(Action action)
        {
            try
            {
                _delayedActions.Enqueue(action);
            }
            catch (Exception ex)
            {
                Log.Lobby.E($"Error in InvokeDelayed: {ex.Message}");
            }
        }

        /// <summary>
        /// Prompts the provider to pull friends from the platform's friend list.
        /// </summary>
        public void PullFriends(FriendFilter filter)
        {
            RunTask(async () =>
            {
                EnsureProviderSet();
                List<FriendItem> friends = await _currentProvider.GetFriendsAsync(filter);
                OnFriendListPulled?.Invoke(friends);
            });
        }

        /// <summary>
        /// Invite the given user to the current lobby.
        /// </summary>
        /// <param name="user"></param>
        public void InviteFriend(FriendItem user)
        {
            RunTask(async () =>
            {
                EnsureProviderSet();
                await _currentProvider.InviteFriendAsync(user);
            });
        }

        /// <summary>
        /// Creates a room using the inspector CreateRoomArgs values.
        /// </summary>
        public void CreateRoom()
        {
            CreateRoom(createRoomArgs.maxPlayers, createRoomArgs.roomProperties.ToDictionary());
        }

        /// <summary>
        /// Creates a room using custom settings set through code
        /// </summary>
        public void CreateRoom(int maxPlayers, Dictionary<string, string> roomProperties = null)
        {
            RunTask(async () =>
            {
                EnsureProviderSet();
                Lobby room = await _currentProvider.CreateLobbyAsync(maxPlayers, roomProperties);
                _oldLobby = default;
                CurrentLobby = room;
                OnRoomUpdated?.Invoke(room);
            });
        }

        /// <summary>
        /// Leave the lobby
        /// </summary>
        public void LeaveLobby()
        {
            RunTask(async () =>
            {
                EnsureProviderSet();
                await _currentProvider.LeaveLobbyAsync();
            });
        }

        /// <summary>
        /// Leave a specific lobby
        /// </summary>
        public void LeaveLobby(string lobbyId)
        {
            RunTask(async () =>
            {
                EnsureProviderSet();
                await _currentProvider.LeaveLobbyAsync(lobbyId);
            });
        }

        /// <summary>
        /// Join the lobby with the given ID
        /// </summary>
        /// <param name="roomId">ID of the lobby to join</param>
        public void JoinLobby(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                OnRoomJoinFailed?.Invoke("Null or empty room ID.");
                return;
            }

            RunTask(async () =>
            {
                EnsureProviderSet();
                Lobby room = await _currentProvider.JoinLobbyAsync(roomId);
                if (room.IsValid)
                {
                    OnRoomJoined?.Invoke(room);
                }
                else
                {
                    OnRoomJoinFailed?.Invoke($"Failed to join room {roomId}");
                }
            });
        }

        /// <summary>
        /// Prompts the provider to search lobbies with given filters
        /// </summary>
        /// <param name="maxRoomsToFind">Max amount of rooms to find</param>
        /// <param name="filters">Filters to use for search - only works if the provider supports it</param>
        public void SearchLobbies(int maxRoomsToFind = 10, Dictionary<string, string> filters = null)
        {
            if(filters == null)
            {
                filters = searchRoomArgs.ToDictionary();
            }

            RunTask(async () =>
            {
                EnsureProviderSet();
                var rooms = await _currentProvider.SearchLobbiesAsync(maxRoomsToFind, filters);
                OnRoomSearchResults?.Invoke(rooms);
            });
        }

        /// <summary>
        /// Sets metadata on the current lobby we're in
        /// </summary>
        /// <param name="key">Key/Identifier of the metadata</param>
        /// <param name="value">The value of the metadata to be stored</param>
        public void SetLobbyData(string key, string value)
        {
            RunTask(async () =>
            {
                EnsureProviderSet();
                await _currentProvider.SetLobbyDataAsync(key, value);
            });
        }

        /// <summary>
        /// Gets metadata from the current lobby we're in
        /// </summary>
        /// <param name="key">Key/Identifier of the metadata we want</param>
        /// <returns>Value of the metadata we get</returns>
        public async Task<string> GetLobbyData(string key)
        {
            EnsureProviderSet();
            return await _currentProvider.GetLobbyDataAsync(key);
        }

        /// <summary>
        /// Shuts down and clears the current provider
        /// </summary>
        public void Shutdown()
        {
            EnsureProviderSet();
            _currentProvider.Shutdown();
            onShutdown?.Invoke();
        }

        /// <summary>
        /// Toggles the local users ready state automatically
        /// </summary>
        public async UniTaskVoid ToggleLocalReady()
        {
            if (!CurrentLobby.IsValid)
            {
                Log.Lobby.E("Can't toggle ready state, current lobby is invalid.");
                return;
            }

            var localUserId = await _currentProvider.GetLocalUserIdAsync();
            if (string.IsNullOrEmpty(localUserId))
            {
                Log.Lobby.E("Can't toggle ready state, local user ID is null or empty.");
                return;
            }

            LobbyUser localLobbyUser = CurrentLobby.Members.Find(lobbyUser => lobbyUser.Id == localUserId);
            SetIsReady(localUserId, !localLobbyUser.IsReady);
        }

        /// <summary>
        /// Set's the given User to Ready
        /// </summary>
        /// <param name="userId">User ID of player</param>
        /// <param name="isReady">Ready state to set</param>
        public void SetIsReady(string userId, bool isReady)
        {
            RunTask(async () =>
            {
                EnsureProviderSet();
                await _currentProvider.SetIsReadyAsync(userId, isReady);
            });
        }

        private async void CallOnAllReady()
        {
            await WaitForAllTasksAsync();

            if(CurrentLobby.IsValid
               && CurrentLobby.Members.Count == PlayersToStart
               && CurrentLobby.Members.TrueForAll(x => x.IsReady))
            {
                await _currentProvider.SetAllReadyAsync();

                OnAllReady?.Invoke();
            }
        }

        public async Task WaitForAllTasksAsync()
        {
            while (_taskLock > 0)
            {
                await Task.Yield();
            }
        }

        private async void RunTask(Func<Task> taskFunc)
        {
            if (taskFunc == null || _currentProvider == null)
            {
                return;
            }

            _taskLock++;
            try
            {
                await taskFunc();
            }
            catch (Exception ex)
            {
                Log.Lobby.E($"Task Error: {ex.Message}");
            }
            finally
            {
                _taskLock--;
                if (_taskLock < 0)
                {
                    _taskLock = 0;
                }
            }
        }

        private void EnsureProviderSet()
        {
            if (_currentProvider == null)
            {
                throw new InvalidOperationException("No lobby provider has been set.");
            }
        }

        public void SetLobbyStarted()
        {
            _currentProvider.SetLobbyStartedAsync();
        }

        public int GetPlayerIndexInLobby()
        {
            string localUserId = _currentProvider.GetLocalUserIdAsync().Result;
            int playerIndexInLobby = CurrentLobby.Members.FindIndex(lobbyUser => lobbyUser.Id == localUserId);

            return playerIndexInLobby;
        }

        [Serializable]
        public class CreateRoomArgs
        {
            public int maxPlayers = 5;
            public SerializableDictionary<string, string> roomProperties;
        }

        [Serializable]
        public enum FriendFilter
        {
            InThisGame,
            Online,
            All
        }
    }
}