using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace NativeMedia
{
    /// <summary>Permission "NSPhotoLibraryAddUsageDescription" for iOS and "WRITE_EXTERNAL_STORAGE" for Android</summary>
    public sealed partial class SaveMediaPermission : BasePlatformPermission
    {
        internal static async Task EnsureGrantedAsync()
        {
            var status = await CheckStatusAsync<SaveMediaPermission>();

            if (status != PermissionStatus.Granted)
                throw ExceptionHelper.PermissionException(status);
        }
    }
}