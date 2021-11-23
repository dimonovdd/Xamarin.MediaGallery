using System;

namespace NativeMedia
{
    static class ExceptionHelper
    {
        internal static Exception NotSupportedOrImplementedException
            => new NotImplementedException("This functionality is not implemented in the portable version of this assembly. " +
                "You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
#if __MOBILE__
        internal static PermissionException PermissionException(PermissionStatus status)
            => new PermissionException($"{nameof(SaveMediaPermission)} was not granted: {status}");
#endif

        private static bool? isSupported;

        internal static void CheckSupport()
        {
            if (!isSupported.HasValue)
            {
                 isSupported
#if __DROID__
                 = Platform.HasSdkVersion(21);
#elif __IOS__
                 = Platform.HasOSVersion(11);
#else
                 = false;
#endif
			}
			if (!isSupported.Value)
                throw NotSupportedOrImplementedException;
        }

#if __DROID__
        internal static Exception ActivityNotDetected
            => new NullReferenceException("The current Activity can not be detected. " +
                $"Ensure that you have called Xamarin.Essentials.Platform.Init in your Activity or Application class.");
#endif

#if __IOS__
        internal static Exception ControllerNotFound
            => new InvalidOperationException("Could not find current view controller.");
#endif
    }
}
