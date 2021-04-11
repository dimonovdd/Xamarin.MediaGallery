using System.Collections.Generic;

namespace Xamarin.MediaGallery
{
    public class MediaPickResult
    {
        internal MediaPickResult(IEnumerable<IMediaFile> files)
            => Files = files;

        public IEnumerable<IMediaFile> Files { get; }
    }
}
