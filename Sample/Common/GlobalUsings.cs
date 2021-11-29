#if NET6_0_ANDROID || NET6_0_IOS
global using Microsoft.Maui.Essentials;
global using Microsoft.Maui.Controls;
global using Microsoft.Maui;
global using Rectangle = Microsoft.Maui.Graphics.Rectangle;
global using ViewRectangle = Microsoft.Maui.Graphics.Rectangle;
global using BasePermission = Microsoft.Maui.Essentials.Permissions.BasePermission;
#else
global using Xamarin.Essentials;
global using Xamarin.Forms;
global using Xamarin.Forms.Internals;
global using Rectangle = System.Drawing.Rectangle;
global using ViewRectangle = Xamarin.Forms.Rectangle;
global using BasePermission = Xamarin.Essentials.Permissions.BasePermission;
global using DeviceInfo = Xamarin.Essentials.DeviceInfo;
#endif