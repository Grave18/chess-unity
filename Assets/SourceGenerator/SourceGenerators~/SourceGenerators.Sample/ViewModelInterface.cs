using System.ComponentModel;
using MvvmTool;

namespace SourceGenerators.Sample;

public partial class ViewModelInterface : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [ObservableProperty]
    private int _lol;

    private void Test()
    {
        Lol++;
        Console.WriteLine(Lol);
    }
}