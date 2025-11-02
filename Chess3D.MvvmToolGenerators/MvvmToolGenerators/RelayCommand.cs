#nullable disable

namespace MvvmToolGenerators;

[System.AttributeUsage(System.AttributeTargets.Method)]
public class RelayCommandAttribute : System.Attribute
{
    public string CanExecute;
}

public interface IRelayCommand : System.Windows.Input.ICommand
{
    void NotifyCanExecuteChanged();
}

public class RelayCommand : IRelayCommand
{
    private System.Action _execute;
    private System.Func<bool> _canExecute;

    public event System.EventHandler CanExecuteChanged;

    public RelayCommand(System.Action execute = null, System.Func<bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public void Execute(object parameter)
    {
        _execute?.Invoke();
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);
    }

    public void Replace(System.Action execute, System.Func<bool> canExecute = null)
    {
        _canExecute = canExecute;
        _execute = execute;
    }
}

public interface IRelayCommand<T> : System.Windows.Input.ICommand
{
    void NotifyCanExecuteChanged();
}

public class RelayCommand<T> : IRelayCommand<T>
{
    private System.Action<T> _execute;
    private System.Func<T, bool> _canExecute;

    public event System.EventHandler CanExecuteChanged;

    public RelayCommand(System.Action<T> execute = null, System.Func<T, bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        if (_canExecute == null)
        {
            return true;
        }

        if (parameter == null && typeof(T).IsValueType)
        {
            return _canExecute(default);
        }

        return parameter is T typedParam && _canExecute(typedParam);
    }

    public void Execute(object parameter)
    {
        if (parameter == null && typeof(T).IsValueType)
        {
            _execute?.Invoke(default);
        }
        else if (parameter is T typedParam)
        {
            _execute?.Invoke(typedParam);
        }
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);
    }

    public void Replace(System.Action<T> execute, System.Func<T, bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
}