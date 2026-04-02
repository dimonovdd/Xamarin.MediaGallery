namespace NativeMedia;

/// <summary>Contains the result of <see cref="MediaGallery.PickAsync(MediaPickRequest, CancellationToken)"/>. Each <see cref="IMediaFile"/> in <see cref="Files"/> must be disposed after use.</summary>
public sealed class MediaPickResult
{
    internal MediaPickResult(IEnumerable<IMediaFile> files)
        => Files = files;

    /// <summary>The media files selected by the user. May be <c>null</c> or empty if the user cancelled the picker without selecting any files.</summary>
    public IEnumerable<IMediaFile> Files { get; }
}