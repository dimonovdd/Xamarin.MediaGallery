using System.Windows.Input;

namespace Sample.Common.ViewModels;

public class HomeVm : BaseVM
{
    public HomeVm()
    {
        NavigateToPickCommand = new Command(() => NavigateAsync(new PickVM()));
        NavigateToSaveCommand = new Command(() => NavigateAsync(new SaveVM()));
    }

    public ICommand NavigateToPickCommand { get; }

    public ICommand NavigateToSaveCommand { get; }
}
