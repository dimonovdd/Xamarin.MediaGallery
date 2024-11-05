namespace NativeMedia;

/// <summary>Describes the result of the <see cref="MediaGallery.PickAsync(MediaPickRequest, CancellationToken)" /> method</summary>
public sealed class MediaPickResult
{
    internal MediaPickResult(IEnumerable<IMediaFile> files)
        => Files = files;

    /// <summary>User-selected media files. Can return a null or empty value</summary>
    public IEnumerable<IMediaFile> Files { get; }
}