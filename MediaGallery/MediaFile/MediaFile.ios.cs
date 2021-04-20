using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MobileCoreServices;

namespace Xamarin.MediaGallery
{
    public partial class MediaFile
    {
        protected virtual Task<Stream> PlatformOpenReadAsync()
           => Task.FromResult<Stream>(null);

        protected virtual void PlatformDispose() { }

        protected string GetExtension(string identifier)
            => UTType.CopyAllTags(identifier, UTType.TagClassFilenameExtension)?.FirstOrDefault();

        protected string GetMIMEType(string identifier)
            => UTType.CopyAllTags(identifier, UTType.TagClassMIMEType)?.FirstOrDefault();
    }

    internal class PHPickerFile : MediaFile
    {
        readonly string identifier;
        NSItemProvider provider;

        internal PHPickerFile(NSItemProvider provider)
        {
            this.provider = provider;
            FileNameWithoutExtension = provider?.SuggestedName;
            identifier = provider?.RegisteredTypeIdentifiers?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(identifier))
                return;

            Extension = GetExtension(identifier);
            ContentType = GetMIMEType(identifier);
        }

        protected override async Task<Stream> PlatformOpenReadAsync()
            => (await provider?.LoadDataRepresentationAsync(identifier))?.AsStream();

        protected override void PlatformDispose()
        {
            provider?.Dispose();
            provider = null;
            base.PlatformDispose();
        }
    }
}
