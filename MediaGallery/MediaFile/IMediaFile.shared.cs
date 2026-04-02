namespace NativeMedia;

/// <summary>Represents a media file (photo or video) selected from the gallery or captured by camera. Must be disposed after use to release native resources.</summary>
public interface IMediaFile : IDisposable
{
    /// <summary>File name without extension. May return <c>null</c> or empty if the platform does not provide a name (e.g. iOS below 14 without photo library permission).</summary>
    string NameWithoutExtension { get; }

    /// <summary>File extension without a leading dot (e.g. "png", "mp4").</summary>
    string Extension { get; }

    /// <summary>MIME content type of the file (e.g. "image/png", "video/mp4").</summary>
    string ContentType { get; }

    /// <summary>Media type of the file. May return <c>null</c> if the type cannot be determined from the content type.</summary>
    MediaFileType? Type { get; }

    /// <summary>Opens a read-only stream to access the file content. The caller is responsible for disposing the returned stream.</summary>
    /// <returns>A <see cref="Stream"/> for reading the file data.</returns>
    Task<Stream> OpenReadAsync();
}