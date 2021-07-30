using System.IO;
using System.Threading.Tasks;

namespace NativeMedia
{
    partial class MediaFile : IMediaFile
    {
        Task<Stream> PlatformOpenReadAsync()
            => throw ExceptionHelper.NotSupportedOrImplementedException;

        void PlatformDispose()
            => throw ExceptionHelper.NotSupportedOrImplementedException;
    }
}
