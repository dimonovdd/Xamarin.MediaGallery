#if __NET6__
global using Microsoft.Maui.ApplicationModel;
global using Microsoft.Maui.Controls;
global using Microsoft.Maui;
global using Rectangle = Microsoft.Maui.Graphics.Rect;
global using ViewRectangle = Microsoft.Maui.Graphics.Rect;
global using BasePermission = Microsoft.Maui.ApplicationModel.Permissions.BasePermission;
global using DeviceInfo = Microsoft.Maui.Devices.DeviceInfo;
global using DevicePlatform = Microsoft.Maui.Devices.DevicePlatform;
global using FileSystem = Microsoft.Maui.Storage.FileSystem;
global using Share = Microsoft.Maui.ApplicationModel.DataTransfer.Share;
global using ShareFile = Microsoft.Maui.ApplicationModel.DataTransfer.ShareFile;
global using ShareFileRequest = Microsoft.Maui.ApplicationModel.DataTransfer.ShareFileRequest;
#else
global using Xamarin.Essentials;
global using Xamarin.Forms;
global using Xamarin.Forms.Internals;
global using Rectangle = System.Drawing.Rectangle;
global using ViewRectangle = Xamarin.Forms.Rectangle;
global using BasePermission = Xamarin.Essentials.Permissions.BasePermission;
global using DeviceInfo = Xamarin.Essentials.DeviceInfo;
#endif