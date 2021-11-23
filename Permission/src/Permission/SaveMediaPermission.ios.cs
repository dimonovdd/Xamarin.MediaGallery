using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photos;
using UIKit;

namespace NativeMedia
{
    public partial class SaveMediaPermission
    {
        /// <summary>List of required keys in Info.plis.</summary>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
                 () => HasOSVersion(14)
                 ? new string[] { "NSPhotoLibraryAddUsageDescription" }
                 : new string[] { "NSPhotoLibraryUsageDescription" };

        /// <summary>Checks the status of <see cref="SaveMediaPermission"/>.</summary>
        /// <returns>The current status of the permission.</returns>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();
            var auth = HasOSVersion(14)
                ? PHPhotoLibrary.GetAuthorizationStatus(PHAccessLevel.AddOnly)
                : PHPhotoLibrary.AuthorizationStatus;

            return Task.FromResult(Convert(auth));
        }

        /// <summary>Request <see cref="SaveMediaPermission"/> from the user.</summary>
        /// <returns>The status of the permission that was requested.</returns>
        public override async Task<PermissionStatus> RequestAsync()
        {
            var status = await CheckStatusAsync();
            if (status == PermissionStatus.Granted)
                return status;

            var auth = HasOSVersion(14)
                ? await PHPhotoLibrary.RequestAuthorizationAsync(PHAccessLevel.AddOnly)
                : await PHPhotoLibrary.RequestAuthorizationAsync();

            return Convert(auth);
        }

        PermissionStatus Convert(PHAuthorizationStatus status)
            => status switch
                {
                    PHAuthorizationStatus.Authorized => PermissionStatus.Granted,
                    PHAuthorizationStatus.Limited => PermissionStatus.Granted,
                    PHAuthorizationStatus.Denied => PermissionStatus.Denied,
                    PHAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                    _ => PermissionStatus.Unknown,
                };

        static bool HasOSVersion(int major) =>
            UIDevice.CurrentDevice.CheckSystemVersion(major, 0);
    }
}