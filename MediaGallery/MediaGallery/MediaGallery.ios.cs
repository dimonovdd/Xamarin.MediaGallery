using System;
using UIKit;
using Xamarin.Essentials;

namespace Xamarin.MediaGallery
{
    public static partial class MediaGallery
    {
        internal static bool HasOSVersion(int major) =>
            UIDevice.CurrentDevice.CheckSystemVersion(major, 0);

        static UIViewController GetCurrentUIViewController()
            => Platform.GetCurrentUIViewController()
            ?? throw new InvalidOperationException("Could not find current view controller.");
    }
}