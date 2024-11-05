using System.Reflection;

namespace Sample.Helpers;

public static class EmbeddedMedia
{
    public const string BaboonPng = "baboon.png";
    public const string EarthMp4 = "earth.mp4";
    public const string LomonosovJpg = "lomonosov.jpg";
    public const string NewtonsCradleGif = "newtons_cradle.gif";
}

public static class EmbeddedResourceProvider
{
    static readonly Assembly Assembly = typeof(EmbeddedResourceProvider).GetTypeInfo().Assembly;
    static readonly string[] Resources = Assembly.GetManifestResourceNames();

    public static Stream? Load(string? name)
    {
        name = GetFullName(name);

        if (string.IsNullOrWhiteSpace(name))
            return null;

        return Assembly.GetManifestResourceStream(name);
    }

    public static ImageSource? GetImageSource(string? name)
    {
        name = GetFullName(name);

        if (string.IsNullOrWhiteSpace(name))
            return null;

        return ImageSource.FromResource(name, Assembly);
    }

    static string? GetFullName(string? name)
        => Resources.FirstOrDefault(n => n.EndsWith($".TestResources.{name}"));
}