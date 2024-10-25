using Sample.Maui.ViewModels;
using Sample.Maui.Views;

namespace Sample.Maui;

public partial class App
{
    public App()
    {
        InitializeComponent();

        var page = new HomePage { BindingContext = new HomeVm() };
        MainPage = new NavigationPage(page);
    }
}