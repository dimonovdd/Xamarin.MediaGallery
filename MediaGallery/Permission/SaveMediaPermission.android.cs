using Android;

namespace NativeMedia
{
    public partial class SaveMediaPermission
    {
        /// <summary>List of required declarations.</summary>
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
                 new (string, bool)[] { (Manifest.Permission.WriteExternalStorage, true) };
    }
}