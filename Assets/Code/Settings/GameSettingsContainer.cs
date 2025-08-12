using System;
using System.Globalization;
using Ai;
using Initialization;
using Logic;
using ParrelSync;
using UnityEngine;

namespace Settings
{
    [DefaultExecutionOrder(-1)]
    public class GameSettingsContainer : MonoBehaviour
    {
        [Tooltip("Use it if no need to load settings")]
        [SerializeField] private bool isInitialized;
        [SerializeField] private GameSettings gameSettings;

        public GameSettings GameSettings => gameSettings;

        // TODO: this is for temporary localhost server. Need to be removed
#if UNITY_EDITOR
        public static string GameSettingsKey => ClonesManager.IsClone() ? "GameSettingsClone" : "GameSettings";
        private static string LocalhostServerKey => ClonesManager.IsClone() ? "IsServerClone" : "IsServer";
#else
        public static string GameSettingsKey => "GameSettings";
        private static string localhostServerKey => "IsServer";
#endif

        public static bool IsLocalhostServer
        {
            get => PlayerPrefs.GetInt(LocalhostServerKey, 0) == 1;
            set => PlayerPrefs.SetInt(LocalhostServerKey, value ? 1 : 0);
        }
        // end todo

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (isInitialized)
            {
                return;
            }

            if (PlayerPrefs.HasKey(GameSettingsKey))
            {
                LoadGameSettings();
            }
            else
            {
                FirstTimeInitGameSettings();
            }

            isInitialized = true;
        }

        private void LoadGameSettings()
        {
            string jsonString = PlayerPrefs.GetString(GameSettingsKey);
            gameSettings = JsonUtility.FromJson<GameSettings>(jsonString);
        }

        private void FirstTimeInitGameSettings()
        {
            Debug.Log("GameSettings is not presented. Adding new");
            gameSettings = new GameSettings
            {
                BoardModelAddress = "Day4 Board",
                PiecesModelAddress = "Day4 Pieces"
            };

            Save();
        }

        /// Two hotseat players
        public void SetupGameOffline()
        {
            PlayerSettings player1Settings = gameSettings.Player1Settings;
            player1Settings.PlayerType = PlayerType.Human;

            PlayerSettings player2Settings = gameSettings.Player2Settings;
            player2Settings.PlayerType = PlayerType.Human;

            Save();
        }

        public void SetupGameWithComputer()
        {
            PlayerSettings computerPlayerSettings = gameSettings.Player2Settings;
            computerPlayerSettings.Name = "Computer";
            computerPlayerSettings.PlayerType = PlayerType.Computer;

            Save();
        }

        public void SetupGameOnline(PieceColor playerColor)
        {
            gameSettings.PlayerColor = playerColor;

            PlayerSettings player1Settings = gameSettings.Player1Settings;
            player1Settings.PlayerType = PlayerType.Online;

            PlayerSettings player2Settings = gameSettings.Player2Settings;
            player2Settings.PlayerType = PlayerType.Online;

            Save();
        }

        public string GetPlayerName()
        {
            return gameSettings.Player1Settings.Name;
        }

        public void SetPlayerName(string playerName)
        {
            gameSettings.Player1Settings.Name = playerName;
            Save();
        }

        public string GetTimeString()
        {
            Vector2 time = GetTime();

            string timeString = time.x.ToString(CultureInfo.InvariantCulture);
            return timeString;
        }

        public Vector2Int GetTime()
        {
            Vector2Int time = gameSettings.Time;

            if (time is { x: 0, y: 0 })
            {
                time = new Vector2Int(30, 0);
                gameSettings.Time = time;
                Save();
            }

            return time;
        }

        public void SetTime(string time)
        {
            if (int.TryParse(time, out int result))
            {
                gameSettings.Time = new Vector2Int(result, 0);
                Save();
            }
        }

        public string GetDifficulty()
        {
            return gameSettings.Player2Settings.ComputerSkillLevel.ToString();
        }

        public void SetDifficulty(string optionText)
        {
            if (Enum.TryParse(optionText, out ComputerSkillLevel computerSkillLevel))
            {
                gameSettings.Player2Settings.ComputerSkillLevel = computerSkillLevel;
                Save();
            }
        }

        public void SetCurrentFen(string value)
        {
            gameSettings.CurrentFen = value;
            Save();
        }

        public void SetCurrentFromDefaultFen()
        {
            gameSettings.CurrentFen = gameSettings.DefaultFen;
            Save();
        }

        public void SetCurrentFromSavedFen()
        {
            gameSettings.CurrentFen = gameSettings.SavedFen;
            Save();
        }

        public void SetCurrentFromCustomFen()
        {
            gameSettings.CurrentFen = gameSettings.CustomFen;
            Save();
        }

        public string GetCurrentFen()
        {
            return gameSettings.CurrentFen;
        }

        public void SetCustomFen(string value)
        {
            gameSettings.CustomFen = value;
            Save();
        }

        public string GetCustomFen()
        {
            return gameSettings.CustomFen;
        }

        public string GetDefaultFen()
        {
            return gameSettings.DefaultFen;
        }

        public string GetSavedFen()
        {
            return gameSettings.SavedFen;
        }

        public void SetSavedFen(string fen)
        {
            gameSettings.SavedFen = fen;
            Save();
        }

        public string GetPieceModelAddress()
        {
            string piecesModelAddress = gameSettings.PiecesModelAddress;

            return piecesModelAddress;
        }

        public void SetPieceModelAddress(string address)
        {
            gameSettings.PiecesModelAddress = address;
            Save();
        }

        public string GetBoardModelAddress()
        {
            string boardModelAddress = gameSettings.BoardModelAddress;

            return boardModelAddress;
        }

        public void SetBoardModelAddress(string address)
        {
            gameSettings.BoardModelAddress = address;
            Save();
        }

        public string GetPlayer1Name()
        {
            return gameSettings.Player1Settings.Name;
        }

        public void SetPlayer1Name(string playerName)
        {
            gameSettings.Player1Settings.Name = playerName;
            Save();
        }
        public string GetPlayer2Name()
        {
            return gameSettings.Player2Settings.Name;
        }

        public void SetPlayer2Name(string playerName)
        {
            gameSettings.Player2Settings.Name = playerName;
            Save();
        }

        private void Save()
        {
            string jsonString = JsonUtility.ToJson(gameSettings);
            PlayerPrefs.SetString(GameSettingsKey, jsonString);
        }
    }
}