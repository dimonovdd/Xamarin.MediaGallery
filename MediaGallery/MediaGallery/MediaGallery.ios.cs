using UIKit;

namespace Xamarin.MediaGallery
{
    public static partial class MediaGallery
    {
        internal static bool HasOSVersion(int major) =>
            UIDevice.CurrentDevice.CheckSystemVersion(major, 0);
    }
}