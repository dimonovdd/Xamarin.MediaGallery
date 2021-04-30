# Xamarin.MediaGallery

![header](https://raw.githubusercontent.com/dimonovdd/Xamarin.MediaGallery/main/header.svg)

[![NuGet Badge](https://img.shields.io/nuget/v/Xamarin.MediaGallery?style=plastic)](https://www.nuget.org/packages/Xamarin.MediaGallery/) [![NuGet downloads](https://img.shields.io/nuget/dt/Xamarin.MediaGallery?style=plastic)](https://www.nuget.org/packages/Xamarin.MediaGallery/) [![license](https://img.shields.io/github/license/dimonovdd/Xamarin.MediaGallery?style=plastic)](https://github.com/dimonovdd/Xamarin.MediaGallery/blob/main/LICENSE)

This plugin is designed for picking and saving photos and video files from the native gallery of Android and iOS devices.

*Unfortunately, at the time of the release of this plugin, [MediaPlugin](https://github.com/jamesmontemagno/MediaPlugin) by [@jamesmontemagno](https://github.com/jamesmontemagno) is no longer supported, and [Xamarin.Essentials](https://github.com/xamarin/Essentials) has not received updates for about 2 months.*
*This plugin has fixed bugs and added some features that are missing in [Xamarin.Essentials](https://github.com/xamarin/Essentials). I hope that in the future it will be ported to [MAUI](https://github.com/dotnet/maui) so that developers have an easy way to add these features to their apps.*

# Available Platforms

| Platform | Version | Minimum OS Version |
| --- | --- | --- |
| Android | MonoAndroid 10.0+| 5.0 |
| iOS | Xamarin.iOS10 | 11.0 |
| .NET Standard | 2.0 | - |

# Getting started

## Android

In the Android project's MainLauncher or any Activity that is launched, this plugin must be initialized in the OnCreate method:

```csharp
protected override void OnCreate(Bundle savedInstanceState)
{
    //...
    base.OnCreate(savedInstanceState);
    Xamarin.MediaGallery.Platform.Init(this, savedInstanceState);
    //...
}
 ```

 To handle runtime results on Android, this plugin must receive any `OnActivityResult`.

 ```csharp
protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
{
    if (Xamarin.MediaGallery.Platform.CheckCanProcessResult(requestCode, resultCode, intent))
    Xamarin.MediaGallery.Platform.OnActivityResult(requestCode, resultCode, intent);
    
    base.OnActivityResult(requestCode, resultCode, intent);
}
 ```

 Open the AndroidManifest.xml file under the Properties folder and add the following inside of the manifest node.

 ```xml
<!-- for saving photo/video -->
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
 ```

## iOS

In your `Info.plist` add the following keys:

 ```xml
<!-- for saving photo/video on iOS 14+ -->
<key>NSPhotoLibraryAddUsageDescription</key>
<string>This app needs access to the photo gallery for saving photos and videos</string>

<!-- for saving photo/video on older versions -->
<key>NSPhotoLibraryUsageDescription</key>
<string>This app needs access to the photo gallery for saving photos and videos</string>
 ```

# PickAsync

This method does not require requesting permissions from users

```csharp
//...
var results = await MediaGallery.PickAsync(1, MediaFileType.Image, MediaFileType.Video);

if (results?.Files == null)
    return;

foreach(var res in results.Files)
{
    var fileName = file.NameWithoutExtension; //Can return an null or empty value
    var extension = file.Extension;
    var contentType = file.ContentType;
    using var stream = await file.OpenReadAsync();
//...
}
 ```

# SaveAsync

```csharp
//...
var status = await Permissions.CheckStatusAsync<SaveMediaPermission>();

if (status != PermissionStatus.Granted)
    return;

await MediaGallery.SaveAsync(MediaFileType.Video, filePath);
//...
 ```

# Platform Differences
## Android

- When saving media files, the date and time are appended to the file name
- When using `PickAsync` method `selectionLimit` parameter just sets multiple pick allowed

## iOS

- Multi picking is supported from iOS version 14.0+ On older versions, the plugin will prompt the user to select a single file
- `NameWithoutExtension` property in iOS versions before 14 returns an null value if permission to access photos was not granted

# Screenshots

|______________|   iOS   | Android |______________|
|:------------:|:---:|:-------:|:------------:|
| |![iOS](https://raw.githubusercontent.com/dimonovdd/Xamarin.MediaGallery/main/Screenshots/ios.jpg)|![Android](https://raw.githubusercontent.com/dimonovdd/Xamarin.MediaGallery/main/Screenshots/droid.jpg)| |
