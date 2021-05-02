using System;
using System.IO;
using System.Threading.Tasks;

namespace NativeMedia
{
    /// <summary>Describes and allows to open a media file</summary>
    public interface IMediaFile : IDisposable
    {
        /// <summary>Returns the file name without extension. Can return an null or empty value</summary>
        string NameWithoutExtension { get; }

        /// <summary>Returns the file extension without a dot (eg:, "png").</summary>
        string Extension { get; }

        /// <summary>Returns the file's content type as a MIME type (eg: "image/png").</summary>
        string ContentType { get; }

        /// <summary>Returns the file type. Can return an null value.</summary>
        MediaFileType? Type { get; }

        /// <summary>Opens a read-only stream containing the file.</summary>
        /// <returns>The task object representing the asynchronous operation. The task returns a Stream used to read data from the file.</returns>
        Task<Stream> OpenReadAsync();
    }
}
