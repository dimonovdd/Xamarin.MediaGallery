using System.Windows.Input;
using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class HomeVm : BaseVM
    {
        public HomeVm()
        {
            NavigateToPickCommand = new Command(() => NavigateAsync(new PickVM()));
            NavigateToSaveCommand = new Command(() => NavigateAsync(new SaveVM()));
            NavigateToPermissionsCommand = new Command(() => NavigateAsync(new PermissionsVM()));
        }

        public ICommand NavigateToPickCommand { get; }

        public ICommand NavigateToSaveCommand { get; }

        public ICommand NavigateToPermissionsCommand { get; }
    }
}
