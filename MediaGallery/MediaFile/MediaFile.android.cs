using System.IO;
using System.Threading.Tasks;
using Android.Webkit;
using Uri = Android.Net.Uri;

namespace Xamarin.MediaGallery
{
    public partial class MediaFile
    {
        internal MediaFile(string fileName, Uri uri)
        {
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            Extension = Path.GetExtension(fileName)?.TrimStart('.');
            ContentType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(Extension);
            openReadAsync =
                () => Task.FromResult(Platform.AppActivity.ContentResolver.OpenInputStream(uri));
        }
    }
}
