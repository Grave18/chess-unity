using System;
using System.Windows.Input;

namespace Ui.Noesis
{
    public class SelectionChanged : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {

        }

        public event EventHandler CanExecuteChanged;
    }
}