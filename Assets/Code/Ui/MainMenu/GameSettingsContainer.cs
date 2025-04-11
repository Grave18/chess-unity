using System;
using System.Globalization;
using Ai;
using GameAndScene.Initialization;
using UnityEngine;

namespace Ui.MainMenu
{
    public class GameSettingsContainer: MonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private bool isInitialized;

        private ComputerSkillLevel _computerSkillLevel;

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

            if (PlayerPrefs.HasKey(GameSettings.Key))
            {
                string jsonString = PlayerPrefs.GetString(GameSettings.Key);
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

        public void SetTime(string time)
        {
            if (int.TryParse(time, out int result))
            {
                gameSettings.Time = new Vector2(result, 0);
            }
        }

        public void SetDifficulty(string optionText)
        {
            if (Enum.TryParse(optionText, out ComputerSkillLevel computerSkillLevel))
            {
                 _computerSkillLevel = computerSkillLevel;
            }
        }

        public string GetTime()
        {
            return gameSettings.Time.x.ToString(CultureInfo.InvariantCulture);
        }

        public string GetDifficulty()
        {
            return gameSettings.Player2Settings.ComputerSkillLevel.ToString();
        }

        public void SetFen(string value)
        {
            gameSettings.CurrentFen = value;
        }

        public string GetCurrentFen()
        {
            return gameSettings.CurrentFen;
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

        private void Save()
        {
            string jsonString = JsonUtility.ToJson(gameSettings);
            PlayerPrefs.SetString(GameSettings.Key, jsonString);
        }

        public GameSettings GetGameSettings()
        {
            return gameSettings;
        }

        public void SetDefaultFen()
        {
            gameSettings.CurrentFen = gameSettings.DefaultFen;
        }

        public void SetSavedFen()
        {
            gameSettings.CurrentFen = gameSettings.SavedFen;
        }

        public string GetSavedFen()
        {
            return gameSettings.SavedFen;
        }
    }
}