using System;
using System.IO;
using System.Threading.Tasks;

namespace Xamarin.MediaGallery
{
    public static partial class MediaGallery
    {
        public static async Task<MediaPickResult> PickAsync(int selectionLimit = 1, params MediaFileType[] types)
        {
            if(!(types?.Length > 0))
                types = new MediaFileType[] { MediaFileType.Image, MediaFileType.Video };

            if (selectionLimit < 0)
                selectionLimit = 1;

            return new MediaPickResult(await PlatformPickAsync(selectionLimit, types));
        }

        public static Task SaveAsync(MediaFileType type, Stream fileStream, string fileName)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));
            CheckFileName(fileName);

            return PlatformSaveAsync(type, fileStream, fileName);
        }

        public static Task SaveAsync(MediaFileType type, byte[] data, string fileName)
        {
            if (!(data?.Length > 0))
                throw new ArgumentNullException(nameof(data));
            CheckFileName(fileName);

            return PlatformSaveAsync(type, data, fileName);
        }

        public static Task SaveAsync(MediaFileType type, string filePath)
        {
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