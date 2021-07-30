using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace Sample.Helpers
{
    public static class PermissionHelper
    {
        public static async Task<PermissionStatus> CheckAndReques<T>(
            string info, Func<string, Task> doDisplayAlert)
            where T : BasePermission, new()
        {
            var status = await CheckStatusAsync<T>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await doDisplayAlert("Please go to settings and enable the permission");
                return status;
            }

            if (ShouldShowRationale<T>())
                await doDisplayAlert(info);

            status = await RequestAsync<T>();

            return status;
        }
    }
}
