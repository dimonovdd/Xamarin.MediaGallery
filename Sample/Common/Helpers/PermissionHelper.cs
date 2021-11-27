using System.Threading.Tasks;

namespace Sample.Helpers;

public static class PermissionHelper
{
    public static async Task<bool> CheckAndRequest<T>(string info)
        where T : BasePermission, new()
    {
        var permission = new T();
        var status = await permission.CheckStatusAsync();

        if (status == PermissionStatus.Granted)
            return true;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            return false;

        status = await permission.RequestAsync();

        return status == PermissionStatus.Granted;

    }
}

