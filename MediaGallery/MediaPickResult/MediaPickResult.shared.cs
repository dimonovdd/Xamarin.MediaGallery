using System.Collections.Generic;

namespace NativeMedia
{
    /// <summary>Describes the result of the <see cref="MediaGallery.PickAsync"/> method</summary>
    public sealed class MediaPickResult
    {
        internal MediaPickResult(IEnumerable<IMediaFile> files)
            => Files = files;

        /// <summary>User-selected media files. Can return an null or empty value</summary>
        public IEnumerable<IMediaFile> Files { get; }
    }
}
