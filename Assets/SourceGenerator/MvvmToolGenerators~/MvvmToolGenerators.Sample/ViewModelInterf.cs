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

    partial void OnTestPropInterf0Changed(int value)
    {
        Console.WriteLine($"TestPropInterf0 changed to: {value}");
    }

    [RelayCommand]
    public void TestMethodInterf0(object obj = null)
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