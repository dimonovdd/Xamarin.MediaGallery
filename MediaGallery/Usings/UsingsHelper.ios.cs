using System;
using CoreGraphics;

static class UsingsHelper
{
    public static Rect EmptyRectangle => Rect.Zero;

    public static CGRect AsCGRect(this Rect rect) =>
        new CGRect(rect.X, rect.Y, rect.Width, rect.Height);

    public static Rect ToRect(this object val) => val is Rect rect ? rect : EmptyRectangle;

}
