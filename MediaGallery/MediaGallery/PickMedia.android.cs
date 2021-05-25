using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Uri = Android.Net.Uri;
using Android.Provider;
#if MONOANDROID11_0
using MediaColumns = Android.Provider.MediaStore.IMediaColumns;
#else
using MediaColumns = Android.Provider.MediaStore.MediaColumns;
#endif


namespace NativeMedia
{
    public static partial class MediaGallery
    {
        const string imageType = "image/*";
        const string videoType = "video/*";
        static TaskCompletionSource<Intent> tcs;

        static async Task<IEnumerable<IMediaFile>> PlatformPickAsync(int selectionLimit, object presentationSourceBounds, params MediaFileType[] types)
        {
            var isImage = types.Contains(MediaFileType.Image);
            tcs = new TaskCompletionSource<Intent>();

            Intent intent;

            // https://github.com/dimonovdd/Xamarin.MediaGallery/pull/5
            if (isImage && types.Length == 1)
            {
                intent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                intent.SetType(imageType);
                intent.PutExtra(Intent.ExtraMimeTypes, new string[] { imageType });
            }
            else
            {
                intent = new Intent(Intent.ActionGetContent);

                intent.SetType(isImage ? $"{imageType}, {videoType}" : videoType);
                intent.PutExtra(
                    Intent.ExtraMimeTypes,
                    isImage
                    ? new string[] { imageType, videoType }
                    : new string[] { videoType });
            }

            intent.PutExtra(Intent.ExtraLocalOnly, false);
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
            var clipCount = intent?.ClipData?.ItemCount ?? 0;
            var data = intent?.Data;

            if (data != null && !(clipCount > 1))
            {
                var res = GetFileResult(data);
                if (res != null)
                    yield return res;
            }
            else if (clipCount > 0)
                for (var i = 0; i < clipCount; i++)
                {
                    var item = intent.ClipData.GetItemAt(i);
                    var res = GetFileResult(item.Uri);
                    if (res != null)
                        yield return res;
                }
        }

        static IMediaFile GetFileResult(Uri uri)
        {
            var name = QueryContentResolverColumn(uri, MediaColumns.DisplayName);
            return string.IsNullOrWhiteSpace(name)
                ? null
                : new MediaFile(name, uri);
        }

        static string QueryContentResolverColumn(Uri contentUri, string columnName)
        {
            try
            {
                using var cursor = Platform.AppActivity.ContentResolver
                    .Query(contentUri, new[] { columnName }, null, null, null);

                if (cursor?.MoveToFirst() ?? false)
                {
                    var columnIndex = cursor.GetColumnIndex(columnName);
                    if (columnIndex != -1)
                        return cursor.GetString(columnIndex);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
