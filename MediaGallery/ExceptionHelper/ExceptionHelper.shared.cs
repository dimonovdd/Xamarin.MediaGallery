namespace NativeMedia
{
    static class ExceptionHelper
    {
        private static Exception NotSupportedOrImplementedException
            => new NotImplementedException("This functionality is not implemented in the portable version of this assembly. " +
                "You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

        internal static PermissionException PermissionException(PermissionStatus status)
            => new($"{nameof(SaveMediaPermission)} was not granted: {status}");

        private static bool? isSupported;

        internal static void CheckSupport()
        {
            if (!isSupported.HasValue)
            {
                 isSupported
#if __ANDROID__
                 = Platform.HasSdkVersion(21);
#else
                 = Platform.HasOSVersion(11);
#endif
			}
			if (!isSupported.Value)
                throw NotSupportedOrImplementedException;
        }

#if __ANDROID__
        internal static Exception ActivityNotDetected
            => new NullReferenceException("The current Activity can not be detected. " +
                $"Ensure that you have called Xamarin.Essentials.Platform.Init in your Activity or Application class.");
#else
        internal static Exception ControllerNotFound
            => new InvalidOperationException("Could not find current view controller.");
#endif
    }
}
