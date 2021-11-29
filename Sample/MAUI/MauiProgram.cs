using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;

namespace Sample.Maui
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>();

			return builder.Build();
		}
	}
}