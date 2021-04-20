using System.IO;
using System.Threading.Tasks;

namespace Xamarin.MediaGallery
{
    public partial class MediaFile : IMediaFile
    {
        Task<Stream> PlatformOpenReadAsync()
            => throw MediaGallery.NotSupportedOrImplementedException;

        void PlatformDispose()
            => throw MediaGallery.NotSupportedOrImplementedException;
    }
}
