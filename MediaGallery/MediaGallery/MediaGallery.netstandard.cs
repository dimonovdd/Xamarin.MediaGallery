using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NativeMedia
{
    public static partial class MediaGallery
    {
        static Task<IEnumerable<IMediaFile>> PlatformPickAsync(int selectionLimit, CancellationToken token, params MediaFileType[] types)
            => Task.FromResult<IEnumerable<IMediaFile>>(null);

        static Task PlatformSaveAsync(MediaFileType type, byte[] data, string fileName)
             => Task.CompletedTask;

        static Task PlatformSaveAsync(MediaFileType type, string filePath)
            => Task.CompletedTask;

        static Task PlatformSaveAsync(MediaFileType type, Stream fileStream, string fileName)
            => Task.CompletedTask;
    }
}