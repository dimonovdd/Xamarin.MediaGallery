using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using CoreImage;
using Foundation;
using ImageIO;
using MobileCoreServices;
using UIKit;

namespace NativeMedia
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

            identifier = GetIdentifier(provider?.RegisteredTypeIdentifiers);

            if (string.IsNullOrWhiteSpace(identifier))
                return;

            Extension = GetExtension(identifier);
            ContentType = GetMIMEType(identifier);
            Type = GetFileType(ContentType);
        }

        protected override async Task<Stream> PlatformOpenReadAsync()
            => (await provider?.LoadDataRepresentationAsync(identifier))?.AsStream();

        protected override void PlatformDispose()
        {
            provider?.Dispose();
            provider = null;
            base.PlatformDispose();
        }

        private string GetIdentifier(string[] identifiers)
        {
            if (!(identifiers?.Length > 0))
                return null;
            if (identifiers.Any(i => i.StartsWith(UTType.LivePhoto)) && identifiers.Contains(UTType.JPEG))
                return identifiers.FirstOrDefault(i => i == UTType.JPEG);
            if (identifiers.Contains(UTType.QuickTimeMovie))
                return identifiers.FirstOrDefault(i => i == UTType.QuickTimeMovie);
            return identifiers.FirstOrDefault();
        }
    }

    class UIDocumentFile : MediaFile
    {
        UIDocument document;
        NSUrl assetUrl;

        internal UIDocumentFile(NSUrl assetUrl, string fileName)
        {
            this.assetUrl = assetUrl;
            document = new UIDocument(assetUrl);
            Extension = document.FileUrl.PathExtension;
            ContentType = GetMIMEType(document.FileType);
            NameWithoutExtension = !string.IsNullOrWhiteSpace(fileName)
                ? Path.GetFileNameWithoutExtension(fileName)
                : null;
            Type = GetFileType(ContentType);
        }

        protected override Task<Stream> PlatformOpenReadAsync()
            => Task.FromResult<Stream>(File.OpenRead(document.FileUrl.Path));

        protected override void PlatformDispose()
        {
            document?.Dispose();
            document = null;
            assetUrl?.Dispose();
            assetUrl = null;
            base.PlatformDispose();
        }
    }

    class PhotoFile : MediaFile
    {
        UIImage img;
        NSDictionary metadata;
        NSMutableData imgWithMetadata;

        internal PhotoFile(UIImage img, NSDictionary metadata, string name)
        {
            this.img = img;
            this.metadata = metadata;
            NameWithoutExtension = name;
            ContentType = GetMIMEType(UTType.JPEG);
            Extension = GetExtension(UTType.JPEG);
            Type = GetFileType(ContentType);
        }

        protected override Task<Stream> PlatformOpenReadAsync()
        {
            imgWithMetadata ??= GetImageWithMeta();
            return Task.FromResult(imgWithMetadata?.AsStream());
        }

        public NSMutableData GetImageWithMeta()
        {
            if (img == null || metadata == null)
                return null;

            using var source = CGImageSource.FromData(img.AsJPEG());
            var destData = new NSMutableData();
            using var destination = CGImageDestination.Create(destData, source.TypeIdentifier, 1, null);
            destination.AddImage(source, 0, metadata);
            destination.Close();
            DisposeSources();
            return destData;
        }

        protected override void PlatformDispose()
        {
            imgWithMetadata?.Dispose();
            imgWithMetadata = null;
            DisposeSources();
            base.PlatformDispose();
        }

        void DisposeSources()
        {
            img?.Dispose();
            img = null;
            metadata?.Dispose();
            metadata = null;
        }
    }
}
