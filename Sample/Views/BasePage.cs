using Sample.ViewModels;

namespace Sample.Views;

[ContentProperty(nameof(Content))]
public class BasePage : ContentPage
{
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is BaseVM vm)
        {
            vm.DoDisplayAlert += OnDisplayAlert;
            vm.DoDisplayConfirm += OnDisplayConfirm;
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
            vm.DoDisplayConfirm -= OnDisplayConfirm;
            vm.DoNavigate -= OnNavigate;
        }
    }

    async Task OnDisplayAlert(string message)
        => await MainThread.InvokeOnMainThreadAsync(
            () => DisplayAlert(null, message, "Ok"));

    async Task<bool> OnDisplayConfirm(string message, string accept)
        => await MainThread.InvokeOnMainThreadAsync(
            () => DisplayAlert(null, message, accept, "Cancel"));

    Task OnNavigate(BaseVM vm, bool showModal)
    {
        var name = vm.GetType().Name.Replace("VM", "Page");

        var pageType = Type.GetType($"{GetType().Namespace}.{name}");

        if (pageType is null)
            throw new NullReferenceException("Page type not found");

        var page = (BasePage?)Activator.CreateInstance(pageType);

        if (page is null)
            throw new NullReferenceException("Page not found");

        page.BindingContext = vm;

        return showModal
            ? Navigation.PushModalAsync(page)
            : Navigation.PushAsync(page);
    }
}