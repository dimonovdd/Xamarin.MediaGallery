using System;
using System.Threading.Tasks;
using Sample.Common.Helpers;

namespace Sample.Common.ViewModels;

public class BaseVM : BaseNotifier
{
    public virtual void OnAppearing()
    {
    }

    public virtual void OnDisappearing()
    {
    }

    public event Func<string, Task> DoDisplayAlert;

    public event Func<BaseVM, bool, Task> DoNavigate;

    protected Task DisplayAlertAsync(string message)
        => DoDisplayAlert?.Invoke(message) ?? Task.CompletedTask;

    protected Task NavigateAsync(BaseVM vm, bool showModal = false)
        => DoNavigate?.Invoke(vm, showModal) ?? Task.CompletedTask;
}
