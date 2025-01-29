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
            // if(!Debug.isDebugBuild)
            // {
                gameObject.SetActive(false);
                Destroy(gameObject);
            // }
        }

#else
        private void Start()
        {
            dropdown.onValueChanged.AddListener(gameManager.SetTurn);
            toggle.onValueChanged.AddListener(gameManager.SetAutoChange);
        }

        private void Update()
        {
            dropdown.value = (int)gameManager.CurrentTurn;
            toggle.isOn = gameManager.IsAutoChange;
        }

#endif
    }
}