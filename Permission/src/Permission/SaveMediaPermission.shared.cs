using System.Threading.Tasks;

namespace NativeMedia
{
    /// <summary>Permission "NSPhotoLibraryAddUsageDescription" for iOS and "WRITE_EXTERNAL_STORAGE" for Android < 10</summary>
    public sealed partial class SaveMediaPermission : EssentialsEx.Permissions.BasePlatformPermission
    {
    }
}