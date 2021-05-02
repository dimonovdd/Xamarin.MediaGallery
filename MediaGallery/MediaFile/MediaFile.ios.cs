using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MobileCoreServices;
using UIKit;

namespace Xamarin.MediaGallery
{
    partial class MediaFile
    {
        protected virtual Task<Stream> PlatformOpenReadAsync()
           => Task.FromResult<Stream>(null);

        protected virtual void PlatformDispose() { }

        protected string GetExtension(string identifier)
            => UTType.CopyAllTags(identifier, UTType.TagClassFilenameExtension)?.FirstOrDefault();

        protected string GetMIMEType(string identifier)
            => UTType.CopyAllTags(identifier, UTType.TagClassMIMEType)?.FirstOrDefault();
    }

    class PHPickerFile : MediaFile
    {
        readonly string identifier;
        NSItemProvider provider;

        internal PHPickerFile(NSItemProvider provider)
        {
            this.provider = provider;
            NameWithoutExtension = provider?.SuggestedName;
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

    class UIDocumentFile : MediaFile
    {
        UIDocument document;

        internal UIDocumentFile(NSUrl assetUrl, string fileName)
        {
            document = new UIDocument(assetUrl);
            Extension = document.FileUrl.PathExtension;
            ContentType = GetMIMEType(document.FileType);
            NameWithoutExtension = !string.IsNullOrWhiteSpace(fileName)
                ? Path.GetFileNameWithoutExtension(fileName)
                : null;
        }

        protected override Task<Stream> PlatformOpenReadAsync()
            => Task.FromResult<Stream>(File.OpenRead(document.FileUrl.Path));

        protected override void PlatformDispose()
        {
            document?.Dispose();
            document = null;
            base.PlatformDispose();
        }
    }
}
