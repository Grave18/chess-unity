using System;
using System.Windows;

namespace chess_3d_blend
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnActivated(EventArgs e)
		{
			MainWindow.DataContext = new ViewModel();
		}
	}
}
