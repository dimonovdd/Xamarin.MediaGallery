using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using Photos;

namespace NativeMedia
{
    public static partial class MediaGallery
    {
        static async Task PlatformSaveAsync(MediaFileType type, byte[] data, string fileName)
        {
            string filePath = null;

            try
            {
                filePath = GetFilePath(fileName);
                await File.WriteAllBytesAsync(filePath, data);

                await PlatformSaveAsync(type, filePath).ConfigureAwait(false);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        static async Task PlatformSaveAsync(MediaFileType type, Stream fileStream, string fileName)
        {
            string filePath = null;

            try
            {
                filePath = GetFilePath(fileName);
                using var stream = File.Create(filePath);
                await fileStream.CopyToAsync(stream);
                stream.Close();

                await PlatformSaveAsync(type, filePath).ConfigureAwait(false);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        static async Task PlatformSaveAsync(MediaFileType type, string filePath)
        {
            using var fileUri = new NSUrl(filePath);

            await PhotoLibraryPerformChanges(() =>
            {
                using var request = type == MediaFileType.Video
                ? PHAssetChangeRequest.FromVideo(fileUri)
                : PHAssetChangeRequest.FromImage(fileUri);
            }).ConfigureAwait(false);
        }

        static async Task PhotoLibraryPerformChanges(Action action)
        {
            var tcs = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);

            PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(
                () =>
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetResult(ex);
                    }
                },
                (success, error) =>
                    tcs.TrySetResult(error != null ? new NSErrorException(error) : null));

            var exception = await tcs.Task;
            if (exception != null)
                throw exception;
        }
    }
}
