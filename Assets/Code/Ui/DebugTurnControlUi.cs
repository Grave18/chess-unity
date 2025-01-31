using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class DebugTurnControlUi : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
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
            dropdown.onValueChanged.AddListener(gameManager.SetTurn);
            toggle.onValueChanged.AddListener(gameManager.SetAutoChange);
            gameManager.OnTurnChanged += UpdateValues;
        }

        private void UpdateValues(PieceColor discard1, CheckType discard2)
        {
            dropdown.onValueChanged.RemoveListener(gameManager.SetTurn);
            toggle.onValueChanged.RemoveListener(gameManager.SetAutoChange);

            // Must not trigger functions in gameManager while updated from manager
            dropdown.value = (int)gameManager.CurrentTurnColor;
            toggle.isOn = gameManager.IsAutoChange;

            dropdown.onValueChanged.AddListener(gameManager.SetTurn);
            toggle.onValueChanged.AddListener(gameManager.SetAutoChange);
        }

#endif
    }
}