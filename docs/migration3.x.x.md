# Migration from v2.X.X to v3.X.X

This version drops Xamarin support and targets only .NET MAUI (net8.0-ios, net8.0-android).

## Removed Xamarin Support

The library no longer supports Xamarin.Forms or Xamarin.Native projects. Only .NET 8 MAUI projects (`net8.0-ios`, `net8.0-android`) are supported.

## SaveMediaPermission

`SaveMediaPermission` is now included in the main `Xamarin.MediaGallery` package.

The separate NuGet packages `Xamarin.MediaGallery.Permision` and `Xamarin.MediaGallery.Permision.Maui` are no longer needed and should be removed from your project.

## MediaPickRequest

### PresentationSourceBounds

The `PresentationSourceBounds` property type has been changed from `object` to `Rect?`.

**Older:**

```csharp
public object PresentationSourceBounds { get; set; }
```

**New:**

```csharp
public Rect? PresentationSourceBounds { get; set; }
```

### UseCreateChooser

The `UseCreateChooser` property has been removed. The Android Photo Picker is now used automatically when available.

**Removed:**

```csharp
public bool UseCreateChooser { get; set; }
```
