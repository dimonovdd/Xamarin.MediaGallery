using Microsoft.Maui.Controls;
using Sample.Common.ViewModels;
using Application = Microsoft.Maui.Controls.Application;

namespace Sample.Maui
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			var page = new HomePage();
            page.BindingContext = new HomeVm();
            MainPage = new NavigationPage(page);
		}
	}
}
