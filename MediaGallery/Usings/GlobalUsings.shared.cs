#if NET6_0_ANDROID || NET6_0_IOS
global using EssentialsEx = Microsoft.Maui.Essentials;
global using Microsoft.Maui.Essentials;
global using Rectangle = Microsoft.Maui.Graphics.Rectangle;
#elif __MOBILE__
global using EssentialsEx = Xamarin.Essentials;
global using Xamarin.Essentials;
#endif
#if XAMARIN_IOS
global using Rectangle = System.Drawing.Rectangle;
#endif
#if MONOANDROID11_0 || NET6_0_ANDROID
global using MediaColumns = Android.Provider.MediaStore.IMediaColumns;
#elif MONOANDROID10_0
global using MediaColumns = Android.Provider.MediaStore.MediaColumns;
#endif