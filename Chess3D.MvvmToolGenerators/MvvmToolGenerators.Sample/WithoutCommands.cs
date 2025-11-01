using System.ComponentModel;
using MvvmTool;

namespace SourceGenerators.Sample;

public partial class WithoutCommands : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [ObservableProperty]
    private int _testPropWithoutCommnads0;

    [ObservableProperty]
    private int _testPropWithoutCommnads1;
}