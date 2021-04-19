using System.IO;
using System.Threading.Tasks;

namespace Xamarin.MediaGallery
{
    public partial class MediaFile : IMediaFile
    {
        public string FileName => $"{FileNameWithoutExtension}.{Extension}";

        public string FileNameWithoutExtension { get; protected internal set; }

        public string Extension { get; protected internal set; }

        public string ContentType { get; protected internal set; }

        public MediaFileType? Type => ContentType.StartsWith("image")
                ? MediaFileType.Image
                : ContentType.StartsWith("video")
                    ? MediaFileType.Video
                    : (MediaFileType?)null;

        public Task<Stream> OpenReadAsync()
            => PlatformOpenReadAsync();
    }
}
