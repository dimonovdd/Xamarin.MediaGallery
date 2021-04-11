# Xamarin.MediaGallery [![NuGet Badge](https://buildstats.info/nuget/Xamarin.MediaGallery)](https://www.nuget.org/packages/Xamarin.MediaGallery/)

![header](/header.svg)

This plugin is designed for picking and saving photos and video files from the native gallery of Android and iOS devices[^1]

[^1]: Unfortunately, at the time of the release of this plugin, [MediaPlugin](https://github.com/jamesmontemagno/MediaPlugin) by [@jamesmontemagno](https://github.com/jamesmontemagno) is no longer supported, and [Xamarin.Essentials](https://github.com/xamarin/Essentials) has not received updates for about 2 months. 
This plugin has fixed bugs and added some features that are missing in [Xamarin.Essentials](https://github.com/xamarin/Essentials). I hope that in the future it will be ported to [MAUI](https://github.com/dotnet/maui) so that developers have an easy way to add these features to their apps.

# Available Platforms

| Platform | Version |
| --- | --- |
| Android | MonoAndroid 10.0+|
| iOS | Xamarin.iOS10 |
| .NET Standard | 2.1 |

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
<string>This app needs access to the photo gallery for picking photos and videos</string>

<!-- for saving photo/video on older versions -->
<key>NSPhotoLibraryUsageDescription</key>
<string>This app needs access to the photo gallery for picking photos and videos</string>
 ```

# PickAsync

```csharp
//...
var results = await MediaGallery.PickAsync(1, MediaFileType.Image, MediaFileType.Video);

if (results?.Files == null)
    return;

foreach(var res in results.Files)
{
    var fileName = file.FileName;
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