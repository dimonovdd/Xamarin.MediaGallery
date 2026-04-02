using Android;
using Android.OS;

namespace NativeMedia;

public partial class SaveMediaPermission
{
    /// <summary>Returns the required Android permissions: WRITE_EXTERNAL_STORAGE on API &lt; 29, none on API 29+.</summary>
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        => (int)Build.VERSION.SdkInt >= 29
            ? Array.Empty<(string, bool)>()
            : [(Manifest.Permission.WriteExternalStorage, true)];
}