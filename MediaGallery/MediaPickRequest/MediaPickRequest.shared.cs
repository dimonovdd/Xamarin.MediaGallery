namespace NativeMedia;

/// <summary>Configures a media file pick operation for <see cref="MediaGallery.PickAsync(MediaPickRequest, CancellationToken)"/>.</summary>
public class MediaPickRequest
{
    /// <summary>Creates a new pick request.</summary>
    /// <param name="selectionLimit"><inheritdoc cref="SelectionLimit" path="/summary"/></param>
    /// <param name="types"><inheritdoc cref="Types" path="/summary"/></param>
    public MediaPickRequest(int selectionLimit = 1, params MediaFileType[] types)
    {
        SelectionLimit = selectionLimit > 0 ? selectionLimit : 1;
        Types = types?.Length > 0
            ? types.Distinct().ToArray()
            : [MediaFileType.Image, MediaFileType.Video];
    }

    /// <summary>Maximum number of files the user can select. Values below 1 are clamped to 1. On Android with the default picker (API &lt; 33), this only enables/disables multi-select without enforcing the exact limit. On Android Photo Picker (API 33+), the limit is enforced. On iOS 14+, the limit is enforced; older iOS versions always allow single selection only.</summary>
    public int SelectionLimit { get; }

    /// <summary>Allowed media file types for picking. Defaults to both <see cref="MediaFileType.Image"/> and <see cref="MediaFileType.Video"/> if empty.</summary>
    public MediaFileType[] Types { get; }

    /// <summary>Optional title for the picker dialog. Used only on Android with the default picker (API &lt; 33); ignored on Android Photo Picker and iOS.</summary>
    public string Title { get; set; }

    /// <summary>Source rectangle for presenting the picker as a popover on iPadOS. When set, the picker arrow points to this rectangle. Has the same behavior as Launcher/Share PresentationSourceBounds in MAUI. Ignored on iPhone and Android.</summary>
    public Rect? PresentationSourceBounds { get; set; }
}