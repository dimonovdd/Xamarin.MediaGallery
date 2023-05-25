using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Foundation;
using Photos;

namespace NativeMedia
{
    public static partial class MediaGallery
    {
        static async Task PlatformSaveAsync(MediaFileType type, byte[] data, string fileName, string albumName = null)
        {
            string filePath = null;

            try
            {
                filePath = GetFilePath(fileName);
                await File.WriteAllBytesAsync(filePath, data);

                await PlatformSaveAsync(type, filePath, albumName).ConfigureAwait(false);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        static async Task PlatformSaveAsync(MediaFileType type, Stream fileStream, string fileName, string albumName = null)
        {
            string filePath = null;

            try
            {
                filePath = GetFilePath(fileName);
                using var stream = File.Create(filePath);
                await fileStream.CopyToAsync(stream);
                stream.Close();

                await PlatformSaveAsync(type, filePath, albumName).ConfigureAwait(false);
            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        static async Task PlatformSaveAsync(MediaFileType type, string filePath, string albumName = null)
        {
            using var fileUri = new NSUrl(filePath);
            
            PHAssetCollection collection = null;
            // If albumName is null we do what we always used to do and not create an album.
            // If albumName is an empty string we don't wish to create an album (which is the same as null for iOS)
            // Otherwise we wish to create an album.
            if (String.IsNullOrEmpty(albumName) == false)
            {
                // Fetch album.
                var fetchOptions = new PHFetchOptions()
                {
                    Predicate = NSPredicate.FromFormat($"title=\"{albumName}\"")
                };
                collection = PHAssetCollection.FetchAssetCollections(PHAssetCollectionType.Album, PHAssetCollectionSubtype.AlbumRegular, fetchOptions).FirstObject as PHAssetCollection;

                // Album does not exist, create it.
                if (collection == null)
                {
                    collection = await PhotoLibraryCreateAlbum(albumName).ConfigureAwait(false);
                }
            }

            await PhotoLibraryPerformChanges(() =>
            {
                using var request = type == MediaFileType.Video
                ? PHAssetChangeRequest.FromVideo(fileUri)
                : PHAssetChangeRequest.FromImage(fileUri);

                // If we have a collection we should put the asset into it.
                if (collection != null)
                {
                    var assetCollectionChangeRequest = PHAssetCollectionChangeRequest.ChangeRequest(collection);
                    assetCollectionChangeRequest.AddAssets(new PHObject[] { request.PlaceholderForCreatedAsset });
                }

            }).ConfigureAwait(false);
        }

        static async Task<PHAssetCollection> PhotoLibraryCreateAlbum(string albumName)
        {
            var tcs = new TaskCompletionSource<PHAssetCollection>(TaskCreationOptions.RunContinuationsAsynchronously);

            PHObjectPlaceholder placeholderForCreatedAssetCollection = null;
            PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
            {
                var createAlbum = PHAssetCollectionChangeRequest.CreateAssetCollection(albumName);
                placeholderForCreatedAssetCollection = createAlbum.PlaceholderForCreatedAssetCollection;
            }, (bool success, NSError error) =>
            {
                if (success)
                {
                    var collectionFetchResult = PHAssetCollection.FetchAssetCollections(new string[] { placeholderForCreatedAssetCollection.LocalIdentifier }, null);
                    var newCollection = collectionFetchResult.FirstObject as PHAssetCollection;
                    tcs.TrySetResult(newCollection);
                }
                else
                {
                    tcs.TrySetResult(null);
                }
            });

            return await tcs.Task;
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
