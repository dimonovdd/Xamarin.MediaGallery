using System;
using System.Threading.Tasks;
using Sample.Helpers;

namespace Sample.ViewModels
{
    public class BaseVM : BaseNotifier
    {
        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }

        internal event Func<string, Task> DoDisplayAlert;

        internal event Func<BaseVM, bool, Task> DoNavigate;

        public Task DisplayAlertAsync(string message)
        {
            return DoDisplayAlert?.Invoke(message) ?? Task.CompletedTask;
        }

        public Task NavigateAsync(BaseVM vm, bool showModal = false)
        {
            return DoNavigate?.Invoke(vm, showModal) ?? Task.CompletedTask;
        }
    }
}
