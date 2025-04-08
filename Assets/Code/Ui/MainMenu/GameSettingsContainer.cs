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
        private Vector2 _time;
        private ComputerSkillLevel _computerSkillLevel;

        private void Awake()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (PlayerPrefs.HasKey(GameSettings.Key))
            {
                string jsonString = PlayerPrefs.GetString(GameSettings.Key);
                gameSettings = JsonUtility.FromJson<GameSettings>(jsonString);
            }
            else
            {
                gameSettings = new GameSettings();
            }
        }

        public void SetTime(string time)
        {
            if (int.TryParse(time, out int result))
            {
                _time = new Vector2(result, 0);
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

        public void SetComputerGame()
        {
            gameSettings.Player2Settings.PlayerType = PlayerType.Computer;
            gameSettings.Player2Settings.ComputerSkillLevel = _computerSkillLevel;
            gameSettings.Time = _time;

            Save();
        }

        private void Save()
        {
            string jsonString = JsonUtility.ToJson(gameSettings);
            PlayerPrefs.SetString(GameSettings.Key, jsonString);
        }
    }
}