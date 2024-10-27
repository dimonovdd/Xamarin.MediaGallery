using Microsoft.Extensions.Logging;

namespace Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder().UseMauiApp<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}