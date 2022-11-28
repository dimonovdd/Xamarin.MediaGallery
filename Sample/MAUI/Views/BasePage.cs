using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Sample.Common.ViewModels;

namespace Sample.Maui
{
    [ContentProperty(nameof(ContentPage.Content))]
    public class BasePage : ContentPage
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is BaseVM vm)
            {
                vm.DoDisplayAlert += OnDisplayAlert;
                vm.DoNavigate += OnNavigate;
                vm.OnAppearing();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is BaseVM vm)
            {
                vm.OnDisappearing();
                vm.DoDisplayAlert -= OnDisplayAlert;
                vm.DoNavigate -= OnNavigate;
            }
        }

        async Task OnDisplayAlert(string message)
            => await MainThread.InvokeOnMainThreadAsync(
                ()=> DisplayAlert(null, message, "Ok"));

        Task OnNavigate(BaseVM vm, bool showModal)
        {
            var name = vm.GetType().Name.Replace("VM", "Page");

            var pageType = Type.GetType($"{GetType().Namespace}.{name}");

            var page = (BasePage)Activator.CreateInstance(pageType);
            page.BindingContext = vm;

            return showModal
                ? Navigation.PushModalAsync(page)
                : Navigation.PushAsync(page);
        }
    }
}
