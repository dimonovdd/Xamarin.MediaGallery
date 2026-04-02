namespace NativeMedia;

/// <summary>Provides methods to pick, save, and capture media files (photos and videos) using native platform APIs.</summary>
public static partial class MediaGallery
{
    static readonly string CacheDir = "XamarinMediaGalleryCacheDir";

    /// <summary>Opens the native media picker to select files from the gallery. Does not require any permissions.</summary>
    /// <returns>A <see cref="MediaPickResult"/> containing the user-selected files, or <c>null</c> if cancelled.</returns>
    /// <inheritdoc cref="MediaPickRequest(int, MediaFileType[])" path="/param" />
    public static Task<MediaPickResult> PickAsync(int selectionLimit = 1, params MediaFileType[] types)
        => PickAsync(new MediaPickRequest(selectionLimit, types));

    /// <param name="request">Media file request to pick.</param>
    /// <inheritdoc cref="PickAsync(int, MediaFileType[])" path="//*[not(self::param)]" />
    public static async Task<MediaPickResult> PickAsync(MediaPickRequest request, CancellationToken token = default)
    {
        ExceptionHelper.CheckSupport();
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return new MediaPickResult(await PlatformPickAsync(request, token).ConfigureAwait(false));
    }

    /// <summary>Saves a media file to the device gallery. Requires <see cref="SaveMediaPermission"/> to be granted before calling.</summary>
    /// <param name="type">The type of media file being saved (image or video).</param>
    /// <param name="fileStream">A readable stream containing the file data.</param>
    /// <param name="fileName">The name of the file including extension (e.g. "photo.jpg"). On Android, the date/time will be appended to the name.</param>
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
    /// <inheritdoc cref="SaveAsync(MediaFileType, Stream, string)" path="" />
    public static async Task SaveAsync(MediaFileType type, byte[] data, string fileName)
    {
        await CheckPossibilitySave();
        if (!(data?.Length > 0))
            throw new ArgumentNullException(nameof(data));
        CheckFileName(fileName);

        await PlatformSaveAsync(type, data, fileName).ConfigureAwait(false);
    }

    /// <param name="filePath">Full path to a local file.</param>
    /// <inheritdoc cref="SaveAsync(MediaFileType, Stream, string)" path="" />
    public static async Task SaveAsync(MediaFileType type, string filePath)
    {
        await CheckPossibilitySave();
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            throw new ArgumentException(nameof(filePath));

        await PlatformSaveAsync(type, filePath).ConfigureAwait(false);
    }

    /// <summary>Checks whether the device has a camera and supports photo capture.</summary>
    /// <returns><c>true</c> if the device can capture photos; otherwise, <c>false</c>.</returns>
    public static bool CheckCapturePhotoSupport()
        => PlatformCheckCapturePhotoSupport();


    /// <summary>Opens the native camera app to capture a photo with EXIF metadata. Requires <see cref="Permissions.Camera"/> to be granted before calling.</summary>
    /// <returns>An <see cref="IMediaFile"/> containing the captured photo, or <c>null</c> if the user cancelled. The file name follows the format "IMG_yyyyMMdd_HHmmss".</returns>
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
        var status = await Permissions.CheckStatusAsync<SaveMediaPermission>();

        if (status != PermissionStatus.Granted)
            throw ExceptionHelper.PermissionException(status);
    }

    static async Task CheckPossibilityCamera()
    {
        ExceptionHelper.CheckSupport();

        if (!CheckCapturePhotoSupport())
            throw new FeatureNotSupportedException();


        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
            throw ExceptionHelper.PermissionException(status);
    }

    static void DeleteFile(string filePath)
    {
        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            File.Delete(filePath);
    }

    static string GetFilePath(string fileName)
    {
        fileName = fileName.Trim();
        var dirPath = Path.Combine(FileSystem.CacheDirectory, CacheDir);
        var filePath = Path.Combine(dirPath, fileName);

        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
        return filePath;
    }

    static string GetNewImageName(string imgName = null)
        => GetNewImageName(DateTime.Now, imgName);

    static string GetNewImageName(DateTime val, string imgName = null)
        => $"{imgName ?? "IMG"}_{val:yyyyMMdd_HHmmss}";
}