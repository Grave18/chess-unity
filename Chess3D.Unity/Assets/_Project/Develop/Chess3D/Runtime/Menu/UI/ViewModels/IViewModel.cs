namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    public interface IViewModel { }

    public interface IMainViewModel
    {
        IViewModel CurrentViewModel { get; set; }
    }
}