using System.IO;
using System.Threading.Tasks;
using Android.Webkit;
using Uri = Android.Net.Uri;

namespace NativeMedia
{
    partial class MediaFile
    {
        readonly Uri uri;
        readonly string tempFilePath;

        internal MediaFile(string fileName, Uri uri, string tempFilePath = null)
        {
            this.tempFilePath = tempFilePath;
            NameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            Extension = Path.GetExtension(fileName);
            ContentType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(Extension);
            this.uri = uri;
            Type = GetFileType(ContentType);
        }

        Task<Stream> PlatformOpenReadAsync()
            => Task.FromResult(Platform.AppActivity.ContentResolver.OpenInputStream(uri));

        void PlatformDispose()
        {
            if(!string.IsNullOrWhiteSpace(tempFilePath) && File.Exists(tempFilePath))
                File.Delete(tempFilePath);

            uri?.Dispose();
        }
    }
}
