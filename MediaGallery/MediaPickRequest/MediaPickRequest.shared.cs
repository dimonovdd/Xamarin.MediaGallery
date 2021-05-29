using System.Drawing;

using System.Linq;

namespace NativeMedia
{
    /// <summary>Request for picking a media file.</summary>
    public class MediaPickRequest
    {
        /// <summary></summary>
        /// <param name="selectionLimit">Maximum count of files to pick. On Android the option just sets multiple pick allowed.</param>
        /// <param name="types">Media file types available for picking</param>
        public MediaPickRequest(int selectionLimit = 1, params MediaFileType[] types)
        {
            SelectionLimit = selectionLimit > 0 ? selectionLimit : 1;
            Types = types?.Length > 0
                ? types.Distinct().ToArray()
                : new MediaFileType[] { MediaFileType.Image, MediaFileType.Video };
        }


        public int SelectionLimit { get; }
        public MediaFileType[] Types { get; }


        /// <summary>Gets or sets the source rectangle to display the Share UI from. This is only used on iPad currently.</summary>
        public Rectangle PresentationSourceBounds { get; set; } = default;
    }
}
