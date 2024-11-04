using Sample.Helpers;
#if __ANDROID__
using Application = Android.App.Application;
using Uri = Android.Net.Uri;
using Android.Content;
using Android.Provider;
#else
using Foundation;
using UIKit;
#endif

namespace Sample.ViewModels;

public class BaseVM : BaseNotifier
{
    public virtual void OnAppearing()
    {
    }

    public virtual void OnDisappearing()
    {
    }

    public event Func<string, Task>? DoDisplayAlert;

    public event Func<string, string, Task<bool>>? DoDisplayConfirm;

    public event Func<BaseVM, bool, Task>? DoNavigate;

    protected Task DisplayAlertAsync(string message)
        => DoDisplayAlert?.Invoke(message) ?? Task.CompletedTask;

    protected Task<bool> DisplayConfirmAsync(string message, string accept)
        => DoDisplayConfirm?.Invoke(message, accept) ?? Task.FromResult(false);

    protected Task NavigateAsync(BaseVM vm, bool showModal = false)
        => DoNavigate?.Invoke(vm, showModal) ?? Task.CompletedTask;

    protected async Task<bool> CheckAndRequestAsync<T>(string message, string goSettingsMessage)
        where T : Permissions.BasePermission, new()
    {
        try
        {
            var permission = new T();
            var status = await permission.CheckStatusAsync();

            if (status == PermissionStatus.Granted)
                return true;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await OpenNavigateSettingsPopup<T>(goSettingsMessage);
                return false;
            }

            if (permission.ShouldShowRationale())
                await DisplayAlertAsync(message);

            status = await permission.RequestAsync();

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.Android && !permission.ShouldShowRationale())
                await OpenNavigateSettingsPopup<T>(goSettingsMessage);

            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(ex.Message);
            return false;
        }
    }

    async Task OpenNavigateSettingsPopup<T>(string message)
    {
        if (!await DisplayConfirmAsync(message, "Settings"))
            return;
#if __ANDROID__
        var settingsIntent = new Intent(Settings.ActionApplicationDetailsSettings,
            Uri.Parse("package:" + Application.Context.PackageName));

        settingsIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.NoHistory | ActivityFlags.ExcludeFromRecents);
        Platform.CurrentActivity?.StartActivity(settingsIntent);
#else
        await UIApplication.SharedApplication.OpenUrlAsync(new NSUrl(UIApplication.OpenSettingsUrlString), new UIApplicationOpenUrlOptions());
#endif
    }
}