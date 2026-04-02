# MediaGallery for .NET Android and iOS (MAUI)

A .NET MAUI library for picking, saving, and capturing media files (photos and videos) using native platform APIs on Android and iOS.

## Available Platforms

| Platform | Minimum OS Version | TargetFramework |
| --- | --- | --- |
| Android | 5.0 (API 21) | `net8.0-android` |
| iOS | 13.6 | `net8.0-ios` |

## Features

- **PickAsync** — pick one or multiple photos/videos from the native gallery. Supports Android Photo Picker (API 33+), selection limit, cancellation token, and iPad popover positioning via `PresentationSourceBounds`
- **SaveAsync** — save media files to the device gallery from `Stream`, `byte[]`, or a local file path. Requires `SaveMediaPermission`
- **CapturePhotoAsync** — capture a photo using the device camera with full EXIF metadata preserved. Requires `Permissions.Camera`
- **CheckCapturePhotoSupport** — check if the device supports camera capture
- **SaveMediaPermission** — built-in permission class for `NSPhotoLibraryAddUsageDescription` (iOS) and `WRITE_EXTERNAL_STORAGE` (Android < 10)

## Public API

```csharp
namespace NativeMedia;

// Pick media files
Task<MediaPickResult> MediaGallery.PickAsync(int selectionLimit = 1, params MediaFileType[] types)
Task<MediaPickResult> MediaGallery.PickAsync(MediaPickRequest request, CancellationToken token = default)

// Save media files
Task MediaGallery.SaveAsync(MediaFileType type, Stream fileStream, string fileName)
Task MediaGallery.SaveAsync(MediaFileType type, byte[] data, string fileName)
Task MediaGallery.SaveAsync(MediaFileType type, string filePath)

// Capture photo
bool MediaGallery.CheckCapturePhotoSupport()
Task<IMediaFile> MediaGallery.CapturePhotoAsync(CancellationToken token = default)

// Media file interface
interface IMediaFile : IDisposable {
    string NameWithoutExtension { get; }
    string Extension { get; }
    string ContentType { get; }
    MediaFileType? Type { get; }
    Task<Stream> OpenReadAsync();
}

// Pick request configuration
class MediaPickRequest {
    int SelectionLimit { get; }
    MediaFileType[] Types { get; }
    string Title { get; set; }
    Rect? PresentationSourceBounds { get; set; }
}

enum MediaFileType { Image, Video }
```

## Setup

Android — initialize in `Activity.OnCreate` and forward `OnActivityResult`:

```csharp
NativeMedia.Platform.Init(this, savedInstanceState);
NativeMedia.Platform.OnActivityResult(requestCode, resultCode, intent);
```

iOS (optional) — initialize in `AppDelegate.FinishedLaunching`:

```csharp
NativeMedia.Platform.Init(GetTopViewController);
```

[Full documentation, samples, and FAQ in the repository](https://github.com/dimonovdd/Xamarin.MediaGallery#mediagallery-for-net-android-and-ios-maui)
