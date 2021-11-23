#if NET6_0 || NET6_0_ANDROID || NET6_0_IOS
global using EssentialsEx = Microsoft.Maui.Essentials;
global using Microsoft.Maui.Essentials;
#else
global using EssentialsEx = Xamarin.Essentials;
global using Xamarin.Essentials;
#endif