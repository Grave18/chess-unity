using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Network.Localhost
{
    [RequireComponent(typeof(TMP_InputField))]
    public class LocalhostPlayersCount : MonoBehaviour
    {
        private TMP_InputField _inputField;

        public static int Get => PlayerPrefs.GetInt("NumberOfPlayers", 2);

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            Assert.IsNotNull(_inputField);
        }

        private void OnEnable()
        {
            _inputField.onEndEdit.AddListener(OnEdit);
        }

        private void OnDisable()
        {
            _inputField.onEndEdit.RemoveListener(OnEdit);
        }

        private static void OnEdit(string numberOfPlayersString)
        {
            SetPlayerCount(numberOfPlayersString);
        }

        private void Start()
        {
            SetPlayerCount(_inputField.text);
        }

        private static void SetPlayerCount(string numberOfPlayersString)
        {
            if (int.TryParse(numberOfPlayersString, out int numOfPlayers))
            {
                PlayerPrefs.SetInt("NumberOfPlayers", numOfPlayers);
            }
            else
            {
                Debug.LogWarning($"{nameof(LocalhostPlayersCount)}: Can't parse number of players");
            }
        }
    }
}