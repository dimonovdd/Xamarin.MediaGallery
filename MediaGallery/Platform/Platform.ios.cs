﻿using UIKit;

namespace NativeMedia
{
    public static partial class Platform
    {
        internal static bool HasOSVersion(int major) =>
            UIDevice.CurrentDevice.CheckSystemVersion(major, 0);
    }
}