using System;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;

namespace NativeMedia
{
    /// <summary>Performs operations with media files.</summary>
    public static partial class MediaGallery
    {
        /// <summary>Opens media files Picker</summary>
        /// <param name="selectionLimit">Maximum count of files to pick. On Android the option just sets multiple pick allowed.</param>
        /// <param name="types">Media file types available for picking</param>
        /// <returns>Media files selected by a user.</returns>
        public static Task<MediaPickResult> PickAsync(int selectionLimit = 1, params MediaFileType[] types)
            => PickAsync(selectionLimit, null, types);

        public static async Task<MediaPickResult> PickAsync(int selectionLimit, object presentationSourceBounds, params MediaFileType[] types)
        {
            ExeptionHelper.CheckSupport();
            if (!(types?.Length > 0))
                types = new MediaFileType[] { MediaFileType.Image, MediaFileType.Video };

            if (selectionLimit < 0)
                selectionLimit = 1;

            return new MediaPickResult(await PlatformPickAsync(selectionLimit, presentationSourceBounds, types));
        }

        /// <summary>Saves a media file with metadata </summary>
        /// <param name="type">Type of media file to save.</param>
        /// <param name="fileStream">The stream to output the file to.</param>
        /// <param name="fileName">The name of the saved file including the extension.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public static Task SaveAsync(MediaFileType type, Stream fileStream, string fileName)
        {
            ExeptionHelper.CheckSupport();
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));
            CheckFileName(fileName);

            return PlatformSaveAsync(type, fileStream, fileName);
        }

        /// <summary>Saves a media file with metadata </summary>
        /// <param name="type">Type of media file to save.</param>
        /// <param name="data">A byte array to save to the file.</param>
        /// <param name="fileName">The name of the saved file including the extension.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public static Task SaveAsync(MediaFileType type, byte[] data, string fileName)
        {
            ExeptionHelper.CheckSupport();
            if (!(data?.Length > 0))
                throw new ArgumentNullException(nameof(data));
            CheckFileName(fileName);

            return PlatformSaveAsync(type, data, fileName);
        }

        /// <summary>Saves a media file with metadata </summary>
        /// <param name="type">Type of media file to save.</param>
        /// <param name="filePath">Full path to a local file.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public static Task SaveAsync(MediaFileType type, string filePath)
        {
            ExeptionHelper.CheckSupport();
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new ArgumentException(nameof(filePath));

            return PlatformSaveAsync(type, filePath);
        }

        static void CheckFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(nameof(fileName));
        }
    }
}