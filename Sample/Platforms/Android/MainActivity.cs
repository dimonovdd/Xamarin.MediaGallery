using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Platform = NativeMedia.Platform;

namespace Sample;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? intent)
    {
        if (Platform.CheckCanProcessResult(requestCode, resultCode, intent))
            Platform.OnActivityResult(requestCode, resultCode, intent);

        base.OnActivityResult(requestCode, resultCode, intent);
    }
}