using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using Sample.Views;
using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class HomeVm : BaseVM
    {
        public HomeVm()
        {
            NavigateToPickCommand = new Command(() => NavigateAsync(new PickVM()));
            NavigateToSaveCommand = new Command(() => NavigateAsync(new SaveVM()));
            NavigateToPopupCommand = new Command(() => PopupNavigation.Instance.PushAsync(new PopupPickPage()));
        }

        public ICommand NavigateToPickCommand { get; }

        public ICommand NavigateToSaveCommand { get; }

        public ICommand NavigateToPopupCommand { get; }
    }
}
