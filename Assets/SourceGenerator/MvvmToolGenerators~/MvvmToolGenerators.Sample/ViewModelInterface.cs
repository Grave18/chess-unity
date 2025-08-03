using System.ComponentModel;
using MvvmTool;

namespace SourceGenerators.Sample;

public partial class ViewModelInterface : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [ObservableProperty]
    private int _lol;

    [RelayCommand]
    private void Test(object obj)
    {
        Lol++;
        Console.WriteLine(Lol);
    }
}