using Notation;
using Ui.Common;
using UnityEngine;

namespace Ui.MainMenu.TextBoxes
{
    public class FenTextBox : TextBoxBase
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        protected override void Awake()
        {
            base.Awake();
            SetText(gameSettingsContainer.GetCurrentFen());
        }

        protected override void OnEndEdit(string fen)
        {
            if (FenValidator.IsValid(fen, out string errorMessage))
            {
                gameSettingsContainer.SetFen(fen);
            }
            else
            {
                Debug.LogWarning(errorMessage);
            }
        }
    }
}