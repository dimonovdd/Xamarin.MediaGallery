
namespace Sample.Helpers
{
    public static class ViewHelpers
    {
        public static ViewRectangle GetAbsoluteBounds(this View element)
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

            return new ViewRectangle(absoluteX, absoluteY, element.Width, element.Height);
        }

        public static Rectangle ToSystemRectangle(this ViewRectangle rect, int offsetY = 0) =>
            new((int)rect.X, (int)rect.Y + offsetY, (int)rect.Width, (int)rect.Height);
    }
}
