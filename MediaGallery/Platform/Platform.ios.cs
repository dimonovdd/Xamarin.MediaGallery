namespace NativeMedia;

/// <summary>iOS-specific initialization for MediaGallery. Optional — only needed if the automatic UIViewController detection does not work in your app (e.g. custom navigation, popup pages).</summary>
public static class Platform
{
    static Func<UIViewController> getCurrentController;

    /// <summary>Provides a custom function to resolve the current UIViewController for presenting pickers and camera UI. Call in AppDelegate.FinishedLaunching if the default MAUI view controller detection does not work.</summary>
    /// <param name="getCurrentUIViewController">A function that returns the currently visible UIViewController.</param>
    public static void Init(Func<UIViewController> getCurrentUIViewController)
        => getCurrentController = getCurrentUIViewController;

    internal static bool HasOSVersion(int major) =>
        UIDevice.CurrentDevice.CheckSystemVersion(major, 0);

    internal static UIViewController GetCurrentUIViewController()
        => getCurrentController?.Invoke()
           ?? Microsoft.Maui.ApplicationModel.Platform.GetCurrentUIViewController()
           ?? throw ExceptionHelper.ControllerNotFound;
}