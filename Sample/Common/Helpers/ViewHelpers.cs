
namespace Sample.Helpers;

public static class ViewHelpers
{
    public static Rectangle GetAbsoluteBounds(this View element, int offsetY = 0)
    {
        Element looper = element;

        var absoluteX = element.X + element.Margin.Top;
        var absoluteY = element.Y + element.Margin.Left;

        // TODO: add logic to handle titles, headers, or other non-view bars

        while (looper.Parent != null)
        {
            looper = looper.Parent;
            if (looper is View v)
            {
                absoluteX += v.X + v.Margin.Top;
                absoluteY += v.Y + v.Margin.Left;
            }
        }

        return new Rectangle((int)absoluteX, (int)absoluteY + offsetY, (int)element.Width, (int)element.Height);
    }
}
