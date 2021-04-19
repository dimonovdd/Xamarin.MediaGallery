using System;
using System.IO;
using System.Threading.Tasks;

namespace Xamarin.MediaGallery
{
    public partial class MediaFile : IMediaFile
    {
        readonly Func<Task<Stream>> openReadAsync;

        internal MediaFile(string fileName, string extension, string contentType, Func<Task<Stream>> openReadAsync)
        {
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            Extension = extension.TrimStart('.');
            ContentType = contentType.ToLower();
            this.openReadAsync = openReadAsync;
        }

        public string FileName => $"{FileNameWithoutExtension}.{Extension}";

        public string FileNameWithoutExtension { get; }

        public string Extension { get; }

        public string ContentType { get; }

        public MediaFileType? Type => ContentType.StartsWith("image")
                ? MediaFileType.Image
                : ContentType.StartsWith("video")
                    ? MediaFileType.Video
                    : (MediaFileType?)null;

        public Task<Stream> OpenReadAsync()
            => openReadAsync?.Invoke();
    }
}
