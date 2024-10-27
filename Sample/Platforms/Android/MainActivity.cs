using Android.App;
using Android.Content.PM;

namespace Sample;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize)]
public class MainActivity : MauiAppCompatActivity;
