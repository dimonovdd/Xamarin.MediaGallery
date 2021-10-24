using System;
using UIKit;

namespace NativeMedia
{
    public static partial class Platform
    {
        static Func<UIViewController> getCurrentController;

        public static void Init(Func<UIViewController> getCurrentUIViewController)
            => getCurrentController = getCurrentUIViewController;

        internal static bool HasOSVersion(int major) =>
            UIDevice.CurrentDevice.CheckSystemVersion(major, 0);

        internal static UIViewController GetCurrentUIViewController()
          => getCurrentController?.Invoke()
            ?? EssentialsEx.Platform.GetCurrentUIViewController()
            ?? throw ExceptionHelper.ControllerNotFound;
    }
}
