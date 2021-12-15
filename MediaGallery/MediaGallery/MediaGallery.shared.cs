using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NativeMedia
{
    /// <summary>Performs operations with media files.</summary>
    public static partial class MediaGallery
    {
        static readonly string cacheDir = "XamarinMediaGalleryCacheDir";

        /// <summary>Opens media files Picker</summary>
        /// <returns>Media files selected by a user.</returns>
        /// <inheritdoc cref = "MediaPickRequest(int, MediaFileType[])" path="/param"/>
        public static Task<MediaPickResult> PickAsync(int selectionLimit = 1, params MediaFileType[] types)
            => PickAsync(new MediaPickRequest(selectionLimit, types), default);

        /// <param name="request">Media file request to pick.</param>
        /// <inheritdoc cref = "PickAsync(int, MediaFileType[])" path="//*[not(self::param)]"/>
        public static async Task<MediaPickResult> PickAsync(MediaPickRequest request, CancellationToken token = default)
        {
            ExceptionHelper.CheckSupport();
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new MediaPickResult(await PlatformPickAsync(request, token).ConfigureAwait(false));
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

           await PlatformSaveAsync(type, fileStream, fileName).ConfigureAwait(false);
        }

        /// <param name="data">A byte array to save to the file.</param>
        /// <inheritdoc cref = "SaveAsync(MediaFileType, Stream, string)" path=""/>
        public static async Task SaveAsync(MediaFileType type, byte[] data, string fileName)
        {
            await CheckPossibilitySave();
            if (!(data?.Length > 0))
                throw new ArgumentNullException(nameof(data));
            CheckFileName(fileName);

            await PlatformSaveAsync(type, data, fileName).ConfigureAwait(false);
        }

        /// <param name="filePath">Full path to a local file.</param>
        /// <inheritdoc cref = "SaveAsync(MediaFileType, Stream, string)" path=""/>
        public static async Task SaveAsync(MediaFileType type, string filePath)
        {
            await CheckPossibilitySave();
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new ArgumentException(nameof(filePath));

            await PlatformSaveAsync(type, filePath).ConfigureAwait(false);
        }

        /// <summary>Checks camera support on a device</summary>
        /// <returns></returns>
        public static bool CheckCapturePhotoSupport()
            => PlatformCheckCapturePhotoSupport();


        /// <summary>Opens camera apps and waits for result from an user</summary>
        /// <returns>Photo with metadata or null</returns>
        public static async Task<IMediaFile> CapturePhotoAsync(CancellationToken token = default)
        {
            await CheckPossibilityCamera();
            return await PlatformCapturePhotoAsync(token);
        }

        static void CheckFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(nameof(fileName));
        }
        
        static async Task CheckPossibilitySave()
        {
            ExceptionHelper.CheckSupport();
#if __MOBILE__
            var status = await Permissions.CheckStatusAsync<SaveMediaPermission>();

            if (status != PermissionStatus.Granted)
                throw ExceptionHelper.PermissionException(status);
#else
            await Task.CompletedTask;
#endif
        }

        static async Task CheckPossibilityCamera()
        {
            ExceptionHelper.CheckSupport();
#if __MOBILE__
            if (!CheckCapturePhotoSupport())
                throw new FeatureNotSupportedException();


            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
                throw ExceptionHelper.PermissionException(status);
#else
            await Task.CompletedTask;
#endif
        }

#if __MOBILE__
        static void DeleteFile(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                File.Delete(filePath);
        }

        static string GetFilePath(string fileName)
        {
            fileName = fileName.Trim();
            var dirPath = Path.Combine(FileSystem.CacheDirectory, cacheDir);
            var filePath = Path.Combine(dirPath, fileName);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            return filePath;
        }
#endif
        static string GetNewImageName(string imgName = null)
            => GetNewImageName(DateTime.Now, imgName);

        static string GetNewImageName(DateTime val, string imgName = null)
            => $"{imgName ?? "IMG"}_{val:yyyyMMdd_HHmmss}";
    }
}