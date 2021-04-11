using System;
using System.IO;
using System.Threading.Tasks;
using Android.Webkit;

namespace Xamarin.MediaGallery
{
    public partial class MediaFile
    {
        internal static MediaFile Create(string fileName, string extension, Func<Task<Stream>> openReadAsync)
            => new MediaFile(
                fileName,
                extension,
                MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension.TrimStart('.')),
                openReadAsync);
    }
}
