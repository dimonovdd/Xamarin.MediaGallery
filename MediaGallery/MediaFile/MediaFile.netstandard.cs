using System.IO;
using System.Threading.Tasks;

namespace Xamarin.MediaGallery
{
    partial class MediaFile : IMediaFile
    {
        Task<Stream> PlatformOpenReadAsync()
            => throw ExeptionHelper.NotSupportedOrImplementedException;

        void PlatformDispose()
            => throw ExeptionHelper.NotSupportedOrImplementedException;
    }
}
