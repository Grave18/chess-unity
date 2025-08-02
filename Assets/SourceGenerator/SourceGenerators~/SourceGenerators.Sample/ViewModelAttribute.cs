using MvvmTool;

namespace SourceGenerators.Sample;

[INotifyPropertyChanged]
public partial class ViewModelAttribute
{
    [ObservableProperty]
    private int _g;

    private void Test()
    {
        G++;
        Console.WriteLine(G);
    }
}