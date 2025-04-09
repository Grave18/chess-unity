using UnityEngine;

namespace Ui.MainMenu.TextBoxes
{
    public class FenTextBox : TextBoxBase
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        protected override void Awake()
        {
            base.Awake();
            TextBox.text = gameSettingsContainer.GetFen();
        }

        protected override void OnEndEdit(string value)
        {
            gameSettingsContainer.SetFen(value);
        }
    }
}