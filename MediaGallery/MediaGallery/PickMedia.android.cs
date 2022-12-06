using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Uri = Android.Net.Uri;
using Android.Provider;
using System.Threading;
using Android.Content.PM;

namespace NativeMedia
{
    public static partial class MediaGallery
    {
        const string imageType = "image/*";
        const string videoType = "video/*";
        static TaskCompletionSource<(Intent, Result)> tcsPick;
        static TaskCompletionSource<(Intent, Result)> tcsCamera;

        static async Task<IEnumerable<IMediaFile>> PlatformPickAsync(MediaPickRequest request, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Intent intent = null;

            try
            {
                tcsPick = new TaskCompletionSource<(Intent, Result)>(TaskCreationOptions.RunContinuationsAsynchronously);

                CancelTaskIfRequested(token, tcsPick, false);

                intent = GetPickerIntent(request);

                CancelTaskIfRequested(token, tcsPick);

                StartActivity(intent, Platform.pickRequestCode, token, tcsPick);

                CancelTaskIfRequested(token, tcsPick, false);
                var result = await tcsPick.Task.ConfigureAwait(false);
                return GetFilesFromIntent(result.Item1);
            }
            finally
            {
                intent?.Dispose();
                intent = null;
                tcsPick = null;
            }
        }

        static Intent GetPickerIntent(MediaPickRequest request)
        {
#if MONOANDROID13_0 || ANDROID33_0_OR_GREATER
            if (ActionPickImagesIsSupported())
                return GetPickerActionPickImagesIntent(request);
#endif
            return GetPickerActionGetContentIntent(request);
        }

        static Intent GetPickerActionGetContentIntent(MediaPickRequest request)
        {
            var mimeTypes = request.Types.Select(GetMimeType).ToArray();
            var intent = new Intent(Intent.ActionGetContent);

            intent.SetType(string.Join(", ", mimeTypes));
            intent.PutExtra(Intent.ExtraMimeTypes, mimeTypes);
            intent.AddCategory(Intent.CategoryOpenable);
            intent.PutExtra(Intent.ExtraLocalOnly, true);
            intent.PutExtra(Intent.ExtraAllowMultiple, request.SelectionLimit > 1);

            if (!string.IsNullOrWhiteSpace(request.Title))
                intent.PutExtra(Intent.ExtraTitle, request.Title);
            return intent;
        }

#if MONOANDROID13_0 || ANDROID33_0_OR_GREATER
        static bool ActionPickImagesIsSupported()
        {
            if (Platform.HasSdkVersion(33))
                return true;
            if (Platform.HasSdkVersion(30))
                return Android.OS.Ext.SdkExtensions.GetExtensionVersion(30) >= 2;
            return false;
        }

        static Intent GetPickerActionPickImagesIntent(MediaPickRequest request)
        {
            var intent = new Intent(MediaStore.ActionPickImages);
            if (request.SelectionLimit > 1)
                intent.PutExtra(MediaStore.ExtraPickImagesMax, request.SelectionLimit);
            if(request.Types.Length == 1)
                intent.SetType(GetMimeType(request.Types[0]));
            return intent;
        }
#endif

        static bool PlatformCheckCapturePhotoSupport()
        {
            if (!Platform.AppActivity?.PackageManager?.HasSystemFeature(PackageManager.FeatureCameraAny) ?? false)
                return false;
            using var intent = GetCameraIntent();
            return Platform.IsIntentSupported(intent);
        }

        static async Task<IMediaFile> PlatformCapturePhotoAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Intent intent = null;
            Uri outputUri = null;

            try
            {
                tcsCamera = new TaskCompletionSource<(Intent, Result)>(TaskCreationOptions.RunContinuationsAsynchronously);
                intent = GetCameraIntent();

                var fileName = $"{GetNewImageName()}.jpg";
                var tempFilePath = GetFilePath(fileName);
                using var file = new Java.IO.File(tempFilePath);
                if (!file.Exists())
                    file.CreateNewFile();
                outputUri = MediaFileProvider.GetUriForFile(Platform.AppActivity, file);
                intent.PutExtra(MediaStore.ExtraOutput, outputUri);

                CancelTaskIfRequested(token, tcsCamera);

                StartActivity(intent, Platform.cameraRequestCode, token, tcsCamera);

                CancelTaskIfRequested(token, tcsCamera, false);
                var result = await tcsCamera.Task.ConfigureAwait(false);
                if (result.Item2 == Result.Ok)
                    return new MediaFile(fileName, outputUri, tempFilePath);

                outputUri?.Dispose();
                return null;
            }
            catch
            {
                outputUri?.Dispose();
                throw;
            }
            finally
            {
                intent?.Dispose();
                intent = null;
                tcsCamera = null;
            }
        }

        internal static void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            if (!CheckCanProcessResult(requestCode, resultCode, intent))
                return;
            (requestCode == Platform.cameraRequestCode ? tcsCamera : tcsPick)?
               .TrySetResult(resultCode == Result.Ok ? (intent, resultCode) : (null, resultCode));
        }

        internal static bool CheckCanProcessResult(int requestCode, Result resultCode, Intent intent)
            => (tcsPick != null && requestCode == Platform.pickRequestCode) || (tcsCamera != null && requestCode == Platform.cameraRequestCode);

        static Intent GetCameraIntent() => new(MediaStore.ActionImageCapture);

        static void CancelTaskIfRequested(CancellationToken token, TaskCompletionSource<(Intent, Result)> tcs, bool needThrow = true)
        {
            if (!token.IsCancellationRequested)
                return;
            tcs?.TrySetCanceled(token);
            if (needThrow)
                token.ThrowIfCancellationRequested();
        }

        static void StartActivity(Intent intent, int requestCode, CancellationToken token, TaskCompletionSource<(Intent, Result)> tcs)
        {
            if (token.CanBeCanceled)
            {
                token.Register(() =>
                {
                    Platform.AppActivity.FinishActivity(requestCode);
                    tcs?.TrySetCanceled(token);
                });
            }

            Platform.AppActivity.StartActivityForResult(intent, requestCode);
        }

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
            {
                for (var i = 0; i < clipCount; i++)
                {
                    var item = intent!.ClipData!.GetItemAt(i);
                    var res = GetFileResult(item!.Uri);
                    if (res != null)
                        yield return res;
                }
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
                using var cursor = Platform.AppActivity?.ContentResolver?
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

        static string GetMimeType(MediaFileType type)
            => type switch
            {
                MediaFileType.Image => imageType,
                MediaFileType.Video => videoType,
                _ => string.Empty,
            };
    }
}
