// using System.ComponentModel;
// using ChessGame.Logic.MenuStates;
// using GameAndScene;
// using Notation;
// using Settings;
// using MvvmTool;
// using Ui.Auxiliary;
// using UnityEngine;
//
// namespace Ui.InGame.ViewModels
// {
//     public partial class PopupViewModel : MonoBehaviour
//     {
//         [SerializeField] private SceneLoader sceneLoader;
//         [SerializeField] private ChessGame.Logic.Game game;
//         [SerializeField] private GameSettingsContainer gameSettingsContainer;
//         [SerializeField] private FenFromBoard fenFromBoard;
//
//         public DelegateCommand PopupYesCommand { get; set; }
//         public DelegateCommand PopupNoCommand { get; set; }
//
//         [ObservableProperty]
//         private bool _isPopupOpen;
//         [ObservableProperty]
//         private string _popupText;
//         [ObservableProperty]
//         private string _popupYesButtonText;
//         [ObservableProperty]
//         private string _popupNoButtonText;
//         [ObservableProperty]
//         private bool _isPopupNoButtonEnabled;
//         [ObservableProperty]
//         private bool _isSaveCheckBoxEnabled;
//         [ObservableProperty]
//         private bool _isSaveCheckBoxChecked;
//
//         public void OpenRematchPopup(object obj)
//         {
//             IsPopupOpen = true;
//             IsPopupNoButtonEnabled = true;
//             IsSaveCheckBoxEnabled = false;
//             PopupText = "Are you want to Rematch?";
//             PopupYesButtonText = "Yes";
//             PopupNoButtonText = "No";
//             PopupYesCommand.ReplaceCommand(Rematch);
//             PopupNoCommand.ReplaceCommand(ClosePopupToPause);
//
//             MenuStateMachine.Instance.OpenPopup();
//         }
//
//         public void OpenDrawPopup(object obj)
//         {
//             IsPopupOpen = true;
//             IsPopupNoButtonEnabled = true;
//             IsSaveCheckBoxEnabled = false;
//             PopupText = "Are you want to Draw?";
//             PopupYesButtonText = "Yes";
//             PopupNoButtonText = "No";
//             PopupYesCommand.ReplaceCommand(Draw);
//             PopupNoCommand.ReplaceCommand(ClosePopupToPause);
//
//             MenuStateMachine.Instance.OpenPopup();
//         }
//
//         public void OpenResignPopup(object obj)
//         {
//             IsPopupOpen = true;
//             IsPopupNoButtonEnabled = true;
//             IsSaveCheckBoxEnabled = false;
//             PopupText = "Are you want to Resign?";
//             PopupYesButtonText = "Yes";
//             PopupNoButtonText = "No";
//             PopupYesCommand.ReplaceCommand(Resign);
//             PopupNoCommand.ReplaceCommand(ClosePopupToPause);
//
//             MenuStateMachine.Instance.OpenPopup();
//         }
//
//         public void OpenExitPopup(object obj)
//         {
//             IsPopupOpen = true;
//             IsPopupNoButtonEnabled = true;
//             IsSaveCheckBoxEnabled = true;
//             PopupText = "Are you want to Exit?";
//             PopupYesButtonText = "Yes";
//             PopupNoButtonText = "No";
//             PopupYesCommand.ReplaceCommand(ExitToMainMenu);
//             PopupNoCommand.ReplaceCommand(ClosePopupToPause);
//
//             MenuStateMachine.Instance.OpenPopup();
//         }
//
//         public void ClosePopupToPause(object obj)
//         {
//             IsPopupOpen = false;
//             MenuStateMachine.Instance.ClosePopupToPause();
//         }
//
//         private void Rematch(object obj)
//         {
//             IsPopupOpen = false;
//             game.Rematch();
//             MenuStateMachine.Instance.ClosePopupToGame();
//         }
//
//         private void Draw(object obj)
//         {
//             IsPopupOpen = false;
//             game.DrawByAgreement();
//             MenuStateMachine.Instance.ClosePopupToGame();
//         }
//
//         private void Resign(object obj)
//         {
//             IsPopupOpen = false;
//             game.Resign();
//             MenuStateMachine.Instance.ClosePopupToGame();
//         }
//
//         private void ExitToMainMenu(object obj)
//         {
//             SaveBoard();
//             sceneLoader.LoadMainMenu();
//         }
//
//         private void SaveBoard()
//         {
//             if (IsSaveCheckBoxChecked)
//             {
//                 string fen = fenFromBoard.Get();
//                 gameSettingsContainer.SetSavedFen(fen);
//                 LogUi.Debug($"Board saved with: {fen}");
//             }
//         }
//     }
// }