using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xamarin.MediaGallery
{
    public static partial class MediaGallery
    {
        static Task<IEnumerable<IMediaFile>> PlatformPickAsync(int selectionLimit, params MediaFileType[] types)
            => throw NotSupportedOrImplementedException;

        static Task PlatformSaveAsync(MediaFileType type, byte[] data, string fileName)
             => throw NotSupportedOrImplementedException;

        static Task PlatformSaveAsync(MediaFileType type, string filePath)
            => throw NotSupportedOrImplementedException;

        static Task PlatformSaveAsync(MediaFileType type, Stream fileStream, string fileName)
            => throw NotSupportedOrImplementedException;

        static Exception NotSupportedOrImplementedException
            => new NotImplementedException("This functionality is not implemented in the portable version of this assembly. " +
                "You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
}