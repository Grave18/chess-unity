using System.Collections.Generic;
using Settings;
using Ui.BoardInMainMenu;
using UnityEngine;
using UnityUi.Common;

namespace UnityUi.Menu.Dropdowns
{
    public class BoardDropdown : DropdownBase
    {
        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private VisualBoardSpawner visualBoardSpawner;

        [Header("Options")]
        [SerializeField] private List<string> boardOptions = new()
        {
            "Prototype Board",
            "Day4 Board",
        };

        protected override List<string> AddOptionsToDropdown()
        {
            return boardOptions;
        }

        protected override int SetCurrentOptionInDropdown(List<string> options)
        {
            string boardModelAddress = gameSettingsContainer.GetBoardModelAddress();
            int boardModelIndex = options.IndexOf(boardModelAddress);
            if(boardModelIndex == -1)
            {
                boardModelIndex = 0;
            }

            return boardModelIndex;
        }

        protected override void ApplyOption(string optionText, int index)
        {
            gameSettingsContainer.SetBoardModelAddress(optionText);
            visualBoardSpawner.SpawnBoard(optionText);
        }
    }
}