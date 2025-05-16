using System;
using System.Globalization;
using Ai;
using ChessGame.Logic;
using Initialization;
using ParrelSync;
using UnityEngine;

namespace Ui.MainMenu
{
    public class GameSettingsContainer: MonoBehaviour
    {
        [Tooltip("Use it if no need to load settings")]
        [SerializeField] private bool isInitialized;
        [SerializeField] private GameSettings gameSettings;

        private ComputerSkillLevel _computerSkillLevel;

        public GameSettings GameSettings => gameSettings;

        public static string GameSettingsKey => ClonesManager.IsClone() ? "GameSettingsClone" : "GameSettings";
        private static string localhostServerKey => ClonesManager.IsClone() ? "IsServerClone" : "IsServer";
        public static bool IsLocalhostServer
        {
            get => PlayerPrefs.GetInt(localhostServerKey, 0) == 1;
            set => PlayerPrefs.SetInt(localhostServerKey, value ? 1 : 0);
        }

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
                string jsonString = PlayerPrefs.GetString(GameSettingsKey);
                gameSettings = JsonUtility.FromJson<GameSettings>(jsonString);
            }
            else
            {
                Debug.Log("GameSettings is not presented. Adding new");
                gameSettings = new GameSettings();
                Save();
            }

            isInitialized = true;
        }

        public void SetupGameWithComputer()
        {
            PlayerSettings computerPlayerSettings = gameSettings.Player2Settings;
            computerPlayerSettings.Name = "Computer";
            computerPlayerSettings.PlayerType = PlayerType.Computer;
            computerPlayerSettings.ComputerSkillLevel = _computerSkillLevel;

            Save();
        }

        public void SetupGameOffline()
        {
            PlayerSettings player1Settings = gameSettings.Player1Settings;
            player1Settings.Name = "Player White";
            player1Settings.PlayerType = PlayerType.Offline;

            PlayerSettings player2Settings = gameSettings.Player2Settings;
            player2Settings.Name = "Player Black";
            player2Settings.PlayerType = PlayerType.Offline;

            Save();
        }

        public void SetupGameOnline(PieceColor playerColor)
        {
            gameSettings.PlayerColor = playerColor;

            PlayerSettings player1Settings = gameSettings.Player1Settings;
            player1Settings.Name = "Player White";
            player1Settings.PlayerType = PlayerType.Online;

            PlayerSettings player2Settings = gameSettings.Player2Settings;
            player2Settings.Name = "Player Black";
            player2Settings.PlayerType = PlayerType.Online;

            Save();
        }

        public void SetTime(string time)
        {
            if (int.TryParse(time, out int result))
            {
                gameSettings.Time = new Vector2(result, 0);
                Save();
            }
        }

        public string GetTime()
        {
            return gameSettings.Time.x.ToString(CultureInfo.InvariantCulture);
        }

        public void SetDifficulty(string optionText)
        {
            if (Enum.TryParse(optionText, out ComputerSkillLevel computerSkillLevel))
            {
                 _computerSkillLevel = computerSkillLevel;
                 Save();
            }
        }

        public string GetDifficulty()
        {
            return gameSettings.Player2Settings.ComputerSkillLevel.ToString();
        }

        public void SetCurrentFen(string value)
        {
            gameSettings.CurrentFen = value;
            Save();
        }

        public string GetCurrentFen()
        {
            return gameSettings.CurrentFen;
        }


        public void SetDefaultFen()
        {
            gameSettings.CurrentFen = gameSettings.DefaultFen;
            Save();
        }

        public string GetDefaultFen()
        {
            return gameSettings.DefaultFen;
        }

        public void SetSavedFen()
        {
            gameSettings.CurrentFen = gameSettings.SavedFen;
            Save();
        }

        public string GetSavedFen()
        {
            return gameSettings.SavedFen;
        }

        public void SaveFen(string fen)
        {
            gameSettings.SavedFen = fen;
            Save();
        }

        private void Save()
        {
            string jsonString = JsonUtility.ToJson(gameSettings);
            PlayerPrefs.SetString(GameSettingsKey, jsonString);
        }
    }
}