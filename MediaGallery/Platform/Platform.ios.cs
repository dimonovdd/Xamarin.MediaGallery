﻿namespace NativeMedia;

public static class Platform
{
    static Func<UIViewController> getCurrentController;

    public static void Init(Func<UIViewController> getCurrentUIViewController)
        => getCurrentController = getCurrentUIViewController;

    internal static bool HasOSVersion(int major) =>
        UIDevice.CurrentDevice.CheckSystemVersion(major, 0);

    internal static UIViewController GetCurrentUIViewController()
        => getCurrentController?.Invoke()
           ?? Microsoft.Maui.ApplicationModel.Platform.GetCurrentUIViewController()
           ?? throw ExceptionHelper.ControllerNotFound;
}