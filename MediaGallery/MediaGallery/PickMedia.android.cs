using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Uri = Android.Net.Uri;
using FileColumns = Android.Provider.MediaStore.Files.FileColumns;
using Path = System.IO.Path;

namespace Xamarin.MediaGallery
{
    public static partial class MediaGallery
    {
        const string imageType = "image/*";
        const string videoType = "video/*";
        const string allType = "*/*";
        static TaskCompletionSource<Intent> tcs;

        static async Task<IEnumerable<IMediaFile>> PlatformPickAsync(int selectionLimit, params MediaFileType[] types)
        {
            var isVideo = types.Contains(MediaFileType.Video);
            var isImage = types.Contains(MediaFileType.Image);
            tcs = new TaskCompletionSource<Intent>();

            Intent intent;

            if(isImage && !isVideo)
            {
                intent = new Intent(Intent.ActionPick);
                intent.SetType(imageType);
            }
            else
            {
                intent = new Intent(Intent.ActionGetContent);

                intent.SetType(isImage ? allType : videoType);
                intent.PutExtra(Intent.ExtraMimeTypes, isImage
                    ? new string[] { imageType, videoType }
                    : new string[] { videoType });
            }

            intent.PutExtra(Intent.ExtraLocalOnly, true);
            intent.PutExtra(Intent.ExtraAllowMultiple, selectionLimit > 1);

            Platform.AppActivity.StartActivityForResult(intent, Platform.requestCode);

            var result = await tcs.Task;

            return GetFilesFromIntent(result);
        }

        internal static void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            if (CheckCanProcessResult(requestCode, resultCode, intent))
                tcs?.TrySetResult(resultCode == Result.Ok ? intent : null);
        }

        internal static bool CheckCanProcessResult(int requestCode, Result resultCode, Intent intent)
            => tcs != null && requestCode == Platform.requestCode;

        static IEnumerable<IMediaFile> GetFilesFromIntent(Intent intent)
        {
            var clipData = intent?.ClipData;
            var data = intent?.Data;

            if (clipData != null)
                for (var i = 0; i < clipData.ItemCount; i++)
                {
                   var item = clipData.GetItemAt(i);
                   yield return GetFileResult(item.Uri);
                }
            else if(data != null)
                yield return GetFileResult(data);
        }


        static IMediaFile GetFileResult(Uri uri)
        {
            var name = QueryContentResolverColumn(uri, FileColumns.DisplayName);
            var fileName = Path.GetFileNameWithoutExtension(name);
            var extension = Path.GetExtension(name);

            return MediaFile.Create(
                fileName,
                extension,
                () => Task.FromResult(
                    Platform.AppActivity.ContentResolver.OpenInputStream(uri)));
        }


        static string QueryContentResolverColumn(Uri contentUri, string columnName)
        {
            string data = null;

            try
            {
                using var cursor = Platform.AppActivity.ContentResolver
                    .Query(contentUri, new[] { columnName }, null, null, null);

                if (cursor?.MoveToFirst() ?? false)
                {
                    var columnIndex = cursor.GetColumnIndex(columnName);
                    if (columnIndex != -1)
                        data = cursor.GetString(columnIndex);
                }
            }
            catch
            {
            }

            return data;
        }
    }
}
