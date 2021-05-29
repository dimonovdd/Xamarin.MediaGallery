using System;
using System.IO;
using System.Threading.Tasks;

namespace NativeMedia
{
    /// <summary>Performs operations with media files.</summary>
    public static partial class MediaGallery
    {
        /// <summary>Opens media files Picker</summary>
        /// <returns>Media files selected by a user.</returns>
        /// <inheritdoc cref = "MediaPickRequest(int, MediaFileType[])" path="/param"/>
        public static Task<MediaPickResult> PickAsync(int selectionLimit = 1, params MediaFileType[] types)
            => PickAsync(new MediaPickRequest(selectionLimit, types));

        /// <param name="request">Media file request to pick.</param>
        /// <inheritdoc cref = "PickAsync(int, MediaFileType[])" path="//*[not(self::param)]"/>
        public static async Task<MediaPickResult> PickAsync(MediaPickRequest request)
        {
            ExeptionHelper.CheckSupport();
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new MediaPickResult(await PlatformPickAsync(request));
        }

        /// <summary>Saves a media file with metadata </summary>
        /// <param name="type">Type of media file to save.</param>
        /// <param name="fileStream">The stream to output the file to.</param>
        /// <param name="fileName">The name of the saved file including the extension.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public static async Task SaveAsync(MediaFileType type, Stream fileStream, string fileName)
        {
            await CheckPossibilitySave();
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));
            CheckFileName(fileName);

           await PlatformSaveAsync(type, fileStream, fileName);
        }

        /// <param name="data">A byte array to save to the file.</param>
        /// <inheritdoc cref = "SaveAsync(MediaFileType, Stream, string)" path=""/>
        public static async Task SaveAsync(MediaFileType type, byte[] data, string fileName)
        {
            await CheckPossibilitySave();
            if (!(data?.Length > 0))
                throw new ArgumentNullException(nameof(data));
            CheckFileName(fileName);

            await PlatformSaveAsync(type, data, fileName);
        }

        /// <param name="filePath">Full path to a local file.</param>
        /// <inheritdoc cref = "SaveAsync(MediaFileType, Stream, string)" path=""/>
        public static async Task SaveAsync(MediaFileType type, string filePath)
        {
            await CheckPossibilitySave();
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new ArgumentException(nameof(filePath));

            await PlatformSaveAsync(type, filePath);
        }

        static void CheckFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(nameof(fileName));
        }


        static async Task CheckPossibilitySave()
        {
            ExeptionHelper.CheckSupport();
            await SaveMediaPermission.EnsureGrantedAsync();
        }
    }
}