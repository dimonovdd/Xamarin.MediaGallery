using Photos;

namespace NativeMedia;

public partial class SaveMediaPermission
{
    /// <summary>Returns the required Info.plist keys: "NSPhotoLibraryAddUsageDescription" on iOS 14+, "NSPhotoLibraryUsageDescription" on older versions.</summary>
    protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
        () => HasOSVersion(14)
            ? ["NSPhotoLibraryAddUsageDescription"]
            : ["NSPhotoLibraryUsageDescription"];

    /// <summary>Checks the current authorization status for saving media to the photo library without prompting the user.</summary>
    /// <returns>The current <see cref="PermissionStatus"/>. Uses PHAccessLevel.AddOnly on iOS 14+.</returns>
    public override Task<PermissionStatus> CheckStatusAsync()
    {
        EnsureDeclared();
        var auth = HasOSVersion(14)
            ? PHPhotoLibrary.GetAuthorizationStatus(PHAccessLevel.AddOnly)
            : PHPhotoLibrary.AuthorizationStatus;

        return Task.FromResult(Convert(auth));
    }

    /// <summary>Requests photo library save permission from the user. Shows the system permission dialog if not yet determined.</summary>
    /// <returns>The resulting <see cref="PermissionStatus"/> after the user responds.</returns>
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