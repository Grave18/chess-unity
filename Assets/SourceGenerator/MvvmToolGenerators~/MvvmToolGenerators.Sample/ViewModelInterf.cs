using System.ComponentModel;
using MvvmTool;

namespace SourceGenerators.Sample;

public partial class ViewModelInterf : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [ObservableProperty]
    private int _testPropInterf0;

    [ObservableProperty]
    private int _testPropInterf1;

    [RelayCommand]
    private void TestMethodInterf0(object obj)
    {
        TestPropInterf0++;
        TestPropInterf1++;
    }

    [RelayCommand]
    private void TestMethodInterf1(object obj)
    {
    TestPropInterf0++;
    TestPropInterf1++;
    }
}