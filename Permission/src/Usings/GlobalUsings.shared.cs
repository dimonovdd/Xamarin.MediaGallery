#if NET6_0 || NET6_0_ANDROID || NET6_0_IOS
global using EssentialsEx = Microsoft.Maui.ApplicationModel;
global using Microsoft.Maui.ApplicationModel;
#else
global using EssentialsEx = Xamarin.Essentials;
global using Xamarin.Essentials;
#endif