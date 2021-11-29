using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.OS;
using Microsoft.Maui;

namespace Sample.Maui
{
	[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
	public class MainActivity : MauiAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
        {
			base.OnCreate(savedInstanceState);
			Microsoft.Maui.Essentials.Platform.Init(this, savedInstanceState);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
		{
			if (NativeMedia.Platform.CheckCanProcessResult(requestCode, resultCode, intent))
				NativeMedia.Platform.OnActivityResult(requestCode, resultCode, intent);

			base.OnActivityResult(requestCode, resultCode, intent);
		}

		
	}
}