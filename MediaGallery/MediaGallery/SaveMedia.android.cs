using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Provider;
using Android.Webkit;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Path = System.IO.Path;
using Stream = System.IO.Stream;
using Uri = Android.Net.Uri;

namespace NativeMedia
{
    public static partial class MediaGallery
    {
        static async Task PlatformSaveAsync(MediaFileType type, byte[] data, string fileName)
        {
            using var ms = new MemoryStream(data);
            await PlatformSaveAsync(type, ms, fileName).ConfigureAwait(false);
        }

        static async Task PlatformSaveAsync(MediaFileType type, string filePath)
        {
            using var fileStream = System.IO.File.OpenRead(filePath);
            await PlatformSaveAsync(type, fileStream, Path.GetFileName(filePath)).ConfigureAwait(false);
        }

        static async Task PlatformSaveAsync(MediaFileType type, Stream fileStream, string fileName)
        {
            var albumName = AppInfo.Name;

            var context = Platform.AppActivity;
            var dateTimeNow = DateTime.Now;

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName).ToLower();
            var newFileName = $"{GetNewImageName(dateTimeNow, fileNameWithoutExtension)}{extension}";

            using var values = new ContentValues();

            values.Put(MediaColumns.DateAdded, TimeSeconds(dateTimeNow));
            values.Put(MediaColumns.Title, fileNameWithoutExtension);
            values.Put(MediaColumns.DisplayName, newFileName);

            var mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension.Replace(".", string.Empty));
            if (!string.IsNullOrWhiteSpace(mimeType))
                values.Put(MediaColumns.MimeType, mimeType);

            using var externalContentUri = type == MediaFileType.Image
                ? MediaStore.Images.Media.ExternalContentUri
                : MediaStore.Video.Media.ExternalContentUri;

            var relativePath = type == MediaFileType.Image
                ? Environment.DirectoryPictures
                : Environment.DirectoryMovies;

            if (Platform.HasSdkVersion(29))
            {
                values.Put(MediaColumns.RelativePath, Path.Combine(relativePath, albumName));
                values.Put(MediaColumns.IsPending, true);

                using var uri = context.ContentResolver.Insert(externalContentUri, values);
                using var stream = context.ContentResolver.OpenOutputStream(uri);
                await fileStream.CopyToAsync(stream);
                stream.Close();

                values.Put(MediaColumns.IsPending, false);
                context.ContentResolver.Update(uri, values, null, null);
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                using var directory = new File(Environment.GetExternalStoragePublicDirectory(relativePath), albumName);

                directory.Mkdirs();
                using var file = new File(directory, newFileName);

                using var fileOutputStream = System.IO.File.Create(file.AbsolutePath);
                await fileStream.CopyToAsync(fileOutputStream);
                fileOutputStream.Close();

                values.Put(MediaColumns.Data, file.AbsolutePath);
                context.ContentResolver.Insert(externalContentUri, values);

                using var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Uri.FromFile(file));
                context.SendBroadcast(mediaScanIntent);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        static readonly DateTime jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // JavaSystem.CurrentTimeMillis()

        static long TimeMillis(DateTime current)
            => (long)CalcTimeDifference(current).TotalMilliseconds;

        static long TimeSeconds(DateTime current)
            => (long)CalcTimeDifference(current).TotalSeconds;

        static TimeSpan CalcTimeDifference(DateTime current)
            => current.ToUniversalTime() - jan1st1970;
    }
}
