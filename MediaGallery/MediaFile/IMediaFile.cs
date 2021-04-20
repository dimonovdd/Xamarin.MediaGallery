using System;
using System.IO;
using System.Threading.Tasks;

namespace Xamarin.MediaGallery
{
    public interface IMediaFile : IDisposable
    {
        string FileNameWithoutExtension { get; }

        string Extension { get; }

        string ContentType { get; }

        MediaFileType? Type { get; }

        Task<Stream> OpenReadAsync();
    }
}