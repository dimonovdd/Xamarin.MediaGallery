using Sample.ViewModels;
using Sample.Views;

namespace Sample;

public partial class App
{
    public App()
    {
        InitializeComponent();

        var page = new HomePage { BindingContext = new HomeVm() };
        MainPage = new NavigationPage(page);
    }
}