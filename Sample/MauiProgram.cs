using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkitMediaElement();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}