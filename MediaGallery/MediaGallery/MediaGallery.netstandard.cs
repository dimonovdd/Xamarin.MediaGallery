using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xamarin.MediaGallery
{
    public static partial class MediaGallery
    {
        static Task<IEnumerable<IMediaFile>> PlatformPickAsync(int selectionLimit, params MediaFileType[] types)
            => throw ExeptionHelper.NotSupportedOrImplementedException;

        static Task PlatformSaveAsync(MediaFileType type, byte[] data, string fileName)
             => throw ExeptionHelper.NotSupportedOrImplementedException;

        static Task PlatformSaveAsync(MediaFileType type, string filePath)
            => throw ExeptionHelper.NotSupportedOrImplementedException;

        static Task PlatformSaveAsync(MediaFileType type, Stream fileStream, string fileName)
            => throw ExeptionHelper.NotSupportedOrImplementedException;
    }
}