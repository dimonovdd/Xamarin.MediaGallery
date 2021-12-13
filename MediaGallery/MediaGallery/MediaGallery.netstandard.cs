using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NativeMedia
{
    public static partial class MediaGallery
    {
        static Task<IEnumerable<IMediaFile>> PlatformPickAsync(MediaPickRequest request, CancellationToken token)
            => Task.FromResult<IEnumerable<IMediaFile>>(null);

        static Task PlatformSaveAsync(MediaFileType type, byte[] data, string fileName)
             => Task.CompletedTask;

        static Task PlatformSaveAsync(MediaFileType type, string filePath)
            => Task.CompletedTask;

        static Task PlatformSaveAsync(MediaFileType type, Stream fileStream, string fileName)
            => Task.CompletedTask;

        static bool PlatformCheckCapturePhotoSupport()
            => false;

        static Task<IMediaFile> PlatformCapturePhotoAsync(CancellationToken token)
            => Task.FromResult<IMediaFile>(null);
    }
}