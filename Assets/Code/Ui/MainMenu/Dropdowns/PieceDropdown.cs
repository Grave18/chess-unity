using System.Collections.Generic;
using BoardEditor;
using Ui.Common;
using UnityEngine;

namespace Ui.MainMenu.Dropdowns
{
    public class PieceDropdown : DropdownBase
    {
        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private VisualBoardSpawner visualBoardSpawner;

        [Header("Options")]
        [SerializeField] private List<string> pieceOptions = new()
        {
            "Prototype Pieces",
            "Day4 Pieces",
        };

        protected override List<string> AddOptionsToDropdown()
        {
            return pieceOptions;
        }

        protected override int SetCurrentOptionInDropdown(List<string> options)
        {
            string pieceModelAddress = gameSettingsContainer.GetPieceModelAddress();
            int pieceModelIndex = options.IndexOf(pieceModelAddress);
            if(pieceModelIndex == -1)
            {
                pieceModelIndex = 0;
            }

            return pieceModelIndex;
        }

        protected override void ApplyOption(string optionText, int index)
        {
            gameSettingsContainer.SetPieceModelAddress(optionText);
            visualBoardSpawner.SpawnPieces(optionText);
        }
    }
}