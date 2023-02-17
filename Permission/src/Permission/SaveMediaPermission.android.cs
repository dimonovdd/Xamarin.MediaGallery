using System;
using Android;
using Android.OS;

namespace NativeMedia
{
    public partial class SaveMediaPermission
    {
        /// <summary>List of required declarations.</summary>
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
            => (int)Build.VERSION.SdkInt >= 29
                ? Array.Empty<(string, bool)>()
                : new (string, bool)[] { (Manifest.Permission.WriteExternalStorage, true) };
    }
}