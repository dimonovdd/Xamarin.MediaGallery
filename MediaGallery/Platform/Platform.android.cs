using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace Xamarin.MediaGallery
{
    /// <summary>Platform specific helpers.</summary>
    public static partial class Platform
    {
        static Activity currentActivity;

        public static int requestCode = 1111;

        /// <summary>Initialize Xamarin.MediaGallery with Android's activity and bundle.</summary>
        /// <param name="activity">Activity to use for initialization.</param>
        /// <param name="bundle">Bundle of the activity.</param>
        public static void Init(Activity activity, Bundle bundle)
            => currentActivity = activity;

        public static void OnActivityResult(int requestCode, Result resultCode, Intent intent)
            => MediaGallery.OnActivityResult(requestCode, resultCode, intent);

        public static bool CheckCanProcessResult(int requestCode, Result resultCode, Intent intent)
            => MediaGallery.CheckCanProcessResult(requestCode, resultCode, intent);

        internal static Activity AppActivity
            => currentActivity
            ?? throw new NullReferenceException("The current Activity can not be detected. " +
                "Ensure that you have called Init in your Activity or Application class.");
    }
}
