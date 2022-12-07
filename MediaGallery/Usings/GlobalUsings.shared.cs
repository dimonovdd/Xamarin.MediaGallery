#if __NET6__
global using EssentialsEx = Microsoft.Maui.ApplicationModel;
global using Microsoft.Maui.ApplicationModel;
global using Rectangle = Microsoft.Maui.Graphics.Rect;
#elif __MOBILE__
global using EssentialsEx = Xamarin.Essentials;
global using Xamarin.Essentials;
#endif
#if __IOS__ && !__NET6__
global using Rectangle = System.Drawing.Rectangle;
#endif
#if MONOANDROID13_0 ||MONOANDROID12_0 || MONOANDROID11_0 || ANDROID30_0_OR_GREATER
global using MediaColumns = Android.Provider.MediaStore.IMediaColumns;
#elif MONOANDROID10_0
global using MediaColumns = Android.Provider.MediaStore.MediaColumns;
#endif