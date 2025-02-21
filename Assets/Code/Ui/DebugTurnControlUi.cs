using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Ui
{
    public class DebugTurnControlUi : MonoBehaviour
    {
        [FormerlySerializedAs("gameManager")]
        [SerializeField] private Game game;
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private Toggle toggle;

#if !UNITY_EDITOR && !DEVELOPMENT_BUILD

        private void Awake()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

#else
        private void Start()
        {
            dropdown.onValueChanged.AddListener(game.SetTurn);
            toggle.onValueChanged.AddListener(game.SetAutoChange);
            game.OnEndTurn += UpdateValues;
        }

        private void UpdateValues()
        {
            dropdown.onValueChanged.RemoveListener(game.SetTurn);
            toggle.onValueChanged.RemoveListener(game.SetAutoChange);

            // Must not trigger functions in gameManager while updated from manager
            dropdown.value = (int)game.CurrentTurnColor;
            toggle.isOn = game.IsAutoChange;

            dropdown.onValueChanged.AddListener(game.SetTurn);
            toggle.onValueChanged.AddListener(game.SetAutoChange);
        }

#endif
    }
}