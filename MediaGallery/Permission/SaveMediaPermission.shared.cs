namespace NativeMedia;

/// <summary>Platform permission required for saving media files to the device gallery. On iOS, requires "NSPhotoLibraryAddUsageDescription" (iOS 14+) or "NSPhotoLibraryUsageDescription" (older versions) in Info.plist. On Android below API 29, requires "WRITE_EXTERNAL_STORAGE" runtime permission; on API 29+ no permission is needed.</summary>
public sealed partial class SaveMediaPermission : Permissions.BasePlatformPermission;