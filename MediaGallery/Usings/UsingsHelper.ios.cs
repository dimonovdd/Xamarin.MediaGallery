﻿using CoreGraphics;

static class UsingsHelper
{
    public static Rectangle EmptyRectangle =>
#if NET6_0_IOS
        Rectangle.Zero;
#else
        Rectangle.Empty;
#endif

    public static CGRect AsCGRect(this Rectangle rect) =>
#if NET6_0_IOS
        rect.AsCGRect();
#else
        rect.ToPlatformRectangle();
#endif

}
