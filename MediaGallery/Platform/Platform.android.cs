using Android.App;
using Android.Content;
using Android.OS;
using AndroidUri = Android.Net.Uri;
using ContentFileProvider = AndroidX.Core.Content.FileProvider;

namespace NativeMedia
{
    /// <summary>Platform specific helpers.</summary>
    public static partial class Platform
    {
        internal static int pickRequestCode = 1111;
        internal static int cameraRequestCode = 1112;

        /// <summary>Initialize Xamarin.MediaGallery with Android's activity and bundle.</summary>
        /// <param name="activity">Activity to use for initialization.</param>
        /// <param name="bundle">Bundle of the activity.</param>
        public static void Init(Activity activity, Bundle bundle) { }

        /// <summary>This method should be used in the <see cref="Activity.OnActivityResult"/></summary>
        /// <param name="requestCode"></param> <param name="resultCode"></param> <param name="intent"></param>
        public static void OnActivityResult(int requestCode, Result resultCode, Intent intent)
            => MediaGallery.OnActivityResult(requestCode, resultCode, intent);

        /// <summary>This method should be used in the <see cref="Activity.OnActivityResult"/></summary>
        /// <param name="requestCode"></param> <param name="resultCode"></param> <param name="intent"></param>
        public static bool CheckCanProcessResult(int requestCode, Result resultCode, Intent intent)
            => MediaGallery.CheckCanProcessResult(requestCode, resultCode, intent);

        internal static Activity AppActivity
            => EssentialsEx.Platform.CurrentActivity
            ?? throw ExceptionHelper.ActivityNotDetected;

        internal static bool HasSdkVersion(int version)
            => (int)Build.VERSION.SdkInt >= version;

        internal static bool IsIntentSupported(Intent intent)
        {
            using var activity = intent.ResolveActivity(AppActivity.PackageManager);
            return activity != null;
        }
            
    }

    [ContentProvider(new[] { "${applicationId}" + Authority },Name = "nativeMedia.fileProvider", Exported = false, GrantUriPermissions = true)]
    [MetaData("android.support.FILE_PROVIDER_PATHS", Resource = "@xml/file_provider_paths")]
    public class MediaFileProvider : ContentFileProvider
    {
        internal const string Authority = ".mediaFileProvider";

        internal static AndroidUri GetUriForFile(Context context, Java.IO.File file)
            => GetUriForFile(context, context.PackageName + Authority, file);
    }
}
