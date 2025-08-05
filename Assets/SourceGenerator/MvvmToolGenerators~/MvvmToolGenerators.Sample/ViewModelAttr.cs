using MvvmTool;

namespace SourceGenerators.Sample;

[INotifyPropertyChanged]
public partial class ViewModelAttr
{
    [ObservableProperty]
    private int _testPropAttr0;

    [ObservableProperty]
    private int _testPropAttr1;

    [DelegateCommand]
    private void TestMethodAttr0(object obj)
    {
        TestPropAttr0++;
        TestPropAttr1++;
    }

    [DelegateCommand]
    private void TestMethodAttr1(object obj)
    {
        TestPropAttr0++;
        TestPropAttr1++;
    }
}