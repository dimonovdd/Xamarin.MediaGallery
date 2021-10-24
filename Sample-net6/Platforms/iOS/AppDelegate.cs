using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Sample_net6
{
	[Register("AppDelegate")]
	public class AppDelegate : MauiUIApplicationDelegate
	{
		protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
	}
}