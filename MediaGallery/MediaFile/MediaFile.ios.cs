using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MobileCoreServices;

namespace Xamarin.MediaGallery
{
    public partial class MediaFile
    {
        internal static MediaFile Create(string fileName, Func<Task<Stream>> openReadAsync, string typeId, string extension = null)
        {
            typeId ??= UTType.CreatePreferredIdentifier(UTType.TagClassFilenameExtension, extension, null);
            extension ??= UTType.CopyAllTags(typeId, UTType.TagClassFilenameExtension)?.FirstOrDefault();
            var mimeType = UTType.CopyAllTags(typeId, UTType.TagClassMIMEType).FirstOrDefault();

            return new MediaFile(fileName, extension, mimeType, openReadAsync);
        }
    }
}
