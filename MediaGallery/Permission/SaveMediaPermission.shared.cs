using System.Threading.Tasks;

namespace NativeMedia
{
    /// <summary>Permission "NSPhotoLibraryAddUsageDescription" for iOS and "WRITE_EXTERNAL_STORAGE" for Android</summary>
    public sealed partial class SaveMediaPermission : Permissions.BasePlatformPermission
    {
        internal static async Task EnsureGrantedAsync()
        {
            var status = await Permissions.CheckStatusAsync<SaveMediaPermission>();

            if (status != PermissionStatus.Granted)
                throw ExceptionHelper.PermissionException(status);
        }
    }
}