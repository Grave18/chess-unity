using MvvmTool;

namespace SourceGenerators.Sample;

[INotifyPropertyChanged]
public partial class RelayCommandTest
{
    [ObservableProperty]
    private int _testPropAttr0;

    [ObservableProperty]
    private int _testPropAttr1;

    [RelayCommand(CanExecute = nameof(TestMethodAttr0_CanExecute))]
    private void TestMethodAttr0()
    {
        TestPropAttr0++;
        TestPropAttr1++;
    }

    private bool TestMethodAttr0_CanExecute()
    {
        return true;
    }

    [RelayCommand(CanExecute = nameof(TestMethodAttr1_CanExecute))]
    private void TestMethodAttr1(object obj)
    {

    }

    private bool TestMethodAttr1_CanExecute(object obj)
    {
        return true;
    }

    [RelayCommand]
    private void TestMethodAttr2()
    {

    }

    [RelayCommand]
    private void TestMethodAttr3(string str)
    {

    }

    [RelayCommand(CanExecute = nameof(TestMethodAttr4_CanExecute))]
    private void TestMethodAttr4()
    {
        TestPropAttr0++;
        TestPropAttr1++;
    }

    private bool TestMethodAttr4_CanExecute(int i)
    {
        return true;
    }

}