using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photos;
using Xamarin.Essentials;

namespace Xamarin.MediaGallery
{
    public partial class SaveMediaPermission
    {
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
                 () => MediaGallery.HasOSVersion(14)
                 ? new string[] { "NSPhotoLibraryAddUsageDescription" }
                 : new string[] { "NSPhotoLibraryUsageDescription" };

        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();
            var auth = MediaGallery.HasOSVersion(14)
                ? PHPhotoLibrary.GetAuthorizationStatus(PHAccessLevel.AddOnly)
                : PHPhotoLibrary.AuthorizationStatus;

            return Task.FromResult(Convert(auth));
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            var status = await CheckStatusAsync();
            if (status == PermissionStatus.Granted)
                return status;

            var auth = MediaGallery.HasOSVersion(14)
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
    }
}