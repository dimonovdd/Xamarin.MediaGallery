using System;

namespace NativeMedia;

static class ExceptionHelper
{
    internal static Exception NotSupportedOrImplementedException
        => new NotImplementedException("This functionality is not implemented in the portable version of this assembly. " +
                                       "You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

    internal static PermissionException PermissionException(PermissionStatus status)
        => new PermissionException($"{nameof(SaveMediaPermission)} was not granted: {status}");


    private static bool? isSupported;

    internal static void CheckSupport()
    {
        if (!isSupported.HasValue)
        {
            if(OperatingSystem.IsAndroid() && OperatingSystem.IsAndroidVersionAtLeast(21))
            {
                isSupported = true;
            }
            else if(OperatingSystem.IsOSPlatform("iOS") && OperatingSystem.IsOSPlatformVersionAtLeast("iOS",11))
            {
                isSupported = true;
            }
            else
            {
                isSupported = false;
            }
                
        }
        if (!isSupported.Value)
            throw NotSupportedOrImplementedException;
    }


    internal static Exception ActivityNotDetected
        => new NullReferenceException("The current Activity can not be detected. " +
                                      $"Ensure that you have called Xamarin.Essentials.Platform.Init in your Activity or Application class.");

    internal static Exception ControllerNotFound
        => new InvalidOperationException("Could not find current view controller.");
}