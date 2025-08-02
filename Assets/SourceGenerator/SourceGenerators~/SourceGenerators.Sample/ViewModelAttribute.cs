using MvvmTool;

namespace SourceGenerators.Sample;

[INotifyPropertyChanged]
public partial class ViewModelAttribute
{
    [ObservableProperty]
    private int _g;

    [RelayCommand]
    private void Test(object obj)
    {
        G++;
        Console.WriteLine(G);
    }
}