using System;
using CoreGraphics;

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
        new CGRect((nfloat)rect.X, (nfloat)rect.Y, (nfloat)rect.Width, (nfloat)rect.Height);
#else
        rect.ToPlatformRectangle();
#endif
    public static Rectangle ToRect(this object val) =>
        val is Rectangle rect ? rect : EmptyRectangle;

}
