using EditorCools;
using Ui.MainMenu;
using UnityEngine;

namespace Initialization
{
    public class GameSettingsConfigurator : MonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings;

        [Button(space: 10)]
        private void Save()
        {
            string json = JsonUtility.ToJson(gameSettings, prettyPrint: true);
            PlayerPrefs.SetString(GameSettingsContainer.GameSettingsKey, json);
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            Save();
        }

#endif
    }
}