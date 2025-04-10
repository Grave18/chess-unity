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

        private ComputerSkillLevel _computerSkillLevel;
        private bool _isInitialized;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (_isInitialized)
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

            _isInitialized = true;
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
            gameSettings.Fen = value;
        }

        public string GetFen()
        {
            return gameSettings.Fen;
        }

        public void SetComputerGame()
        {
            PlayerSettings computerPlayerSettings = gameSettings.Player2Settings;
            computerPlayerSettings.Name = "Computer";
            computerPlayerSettings.PlayerType = PlayerType.Computer;
            computerPlayerSettings.ComputerSkillLevel = _computerSkillLevel;

            Save();
        }

        public void Set2PlayersOfflineGame()
        {
            gameSettings.Player1Settings.PlayerType = PlayerType.Offline;
            gameSettings.Player2Settings.PlayerType = PlayerType.Offline;

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
    }
}