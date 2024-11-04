namespace NativeMedia;

/// <summary>Request for picking a media file.</summary>
public class MediaPickRequest
{
    /// <summary></summary>
    /// <param name="selectionLimit"><inheritdoc cref="SelectionLimit" path="/summary"/></param>
    /// <param name="types"><inheritdoc cref="Types" path="/summary"/></param>
    public MediaPickRequest(int selectionLimit = 1, params MediaFileType[] types)
    {
        SelectionLimit = selectionLimit > 0 ? selectionLimit : 1;
        Types = types?.Length > 0
            ? types.Distinct().ToArray()
            : [ MediaFileType.Image, MediaFileType.Video];
    }

    /// <summary>Maximum count of files to pick. On Android the option just sets multiple pick allowed.</summary>
    public int SelectionLimit { get; }

    /// <summary>Media file types available for picking.</summary>
    public MediaFileType[] Types { get; }

    /// <summary>Media file types available for picking.</summary>
    public string Title { get; set; }

    /// <summary>Gets or sets the source rectangle to display the Picker UI from. This is only used on iPad currently.</summary>
    public Rect? PresentationSourceBounds { get; set; }
}