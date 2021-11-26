# Migration from v1.X.X to v2.X.X

This plugin has some API changes between v1.X.X to v2.X.X. These changes are necessary to support .NET MAUI.

## SaveMediaPermission

`SaveMediaPermission` has been moved to new nuget packages [`Xamarin.MediaGallery.Permision`](https://www.nuget.org/packages/Xamarin.MediaGallery.Permision) and [`Xamarin.MediaGallery.Permision.Maui`](https://www.nuget.org/packages/Xamarin.MediaGallery.Permision.Maui).

And it is based on `Xamarin.Essentials.Permissions.BasePlatformPermission` for Xamarin and `Microsoft.Maui.Essentials.Permissions.BasePlatformPermission` for .net6(MAUI)

## MediaPickRequest

`PresentationSourceBounds` property type has been changed to `object`.

**Older:**

```cssharp
public System.Drawing.Rectangle PresentationSourceBounds { get; set; }
```

**New:**

```cssharp
public object PresentationSourceBounds { get; set; }
```

`PresentationSourceBounds` will be converted to `System.Drawing.Rectangle` for Xamarin and to `Microsoft.Maui.Graphics.Rectangle` for .net6(MAUI).If the value is null or does not compatible specified types, it will be used as `System.Drawing.Rectangle.Empty` or `Microsoft.Maui.Graphics.Rectangle.Zero`.
