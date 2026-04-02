using Android.Content;
using Android.OS;
using AndroidUri = Android.Net.Uri;
using ContentFileProvider = AndroidX.Core.Content.FileProvider;
using File = Java.IO.File;

namespace NativeMedia;

/// <summary>Android-specific initialization and activity result handling for MediaGallery. Must be called from the main activity.</summary>
public static class Platform
{
    internal static int pickRequestCode = 1111;
    internal static int cameraRequestCode = 1112;

    internal static Activity AppActivity
        => Microsoft.Maui.ApplicationModel.Platform.CurrentActivity
           ?? throw ExceptionHelper.ActivityNotDetected;

    /// <summary>Initializes the MediaGallery plugin. Must be called in <see cref="Activity.OnCreate"/> of the main activity.</summary>
    /// <param name="activity">The current Android activity.</param>
    /// <param name="bundle">The saved instance state bundle from OnCreate.</param>
    public static void Init(Activity activity, Bundle bundle) { }

    /// <summary>Forwards activity results to MediaGallery for processing pick/capture operations. Must be called in <see cref="Activity.OnActivityResult"/> after <see cref="CheckCanProcessResult"/>.</summary>
    /// <param name="requestCode">The request code from OnActivityResult.</param>
    /// <param name="resultCode">The result code from OnActivityResult.</param>
    /// <param name="intent">The result intent data from OnActivityResult.</param>
    public static void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        => MediaGallery.OnActivityResult(requestCode, resultCode, intent);

    /// <summary>Checks whether the activity result belongs to a MediaGallery operation (pick or camera capture). Use this before calling <see cref="OnActivityResult"/>.</summary>
    /// <param name="requestCode">The request code from OnActivityResult.</param>
    /// <param name="resultCode">The result code from OnActivityResult.</param>
    /// <param name="intent">The result intent data from OnActivityResult.</param>
    /// <returns><c>true</c> if the result should be processed by <see cref="OnActivityResult"/>; otherwise, <c>false</c>.</returns>
    public static bool CheckCanProcessResult(int requestCode, Result resultCode, Intent intent)
        => MediaGallery.CheckCanProcessResult(requestCode, resultCode, intent);

    internal static bool HasSdkVersion(int version)
        => (int)Build.VERSION.SdkInt >= version;

    internal static bool IsIntentSupported(Intent intent)
    {
        using var activity = intent.ResolveActivity(AppActivity.PackageManager);
        return activity != null;
    }
}

[ContentProvider(["${applicationId}" + Authority], Name = "nativeMedia.fileProvider", Exported = false, GrantUriPermissions = true)]
[MetaData("android.support.FILE_PROVIDER_PATHS", Resource = "@xml/file_provider_paths")]
public class MediaFileProvider : ContentFileProvider
{
    internal const string Authority = ".mediaFileProvider";

    internal static AndroidUri GetUriForFile(Context context, File file)
        => GetUriForFile(context, context.PackageName + Authority, file);
}