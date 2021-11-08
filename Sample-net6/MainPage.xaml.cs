﻿using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using NativeMedia;
using System.Threading.Tasks;
using Microsoft.Maui;
using System;
using System.IO;
#if __IOS__
using Foundation;
#else
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Android.Net;
using Android.Graphics;
#endif


namespace Sample_net6
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnPickClicked(object sender, EventArgs e)
        {
            try
            {

                var res = await MediaGallery.PickAsync(3);
                FilesLabel.Text = res?.Files?.Any() ?? false
                    ? string.Join(Environment.NewLine, res.Files.Select(a => $"{a.NameWithoutExtension}.{a.Extension}"))
                    : string.Empty;
            }
            catch (Exception ex)
            {
                await DisplayAlert(ex.Message);
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var status = await PermissionHelper.CheckAndReques<SaveMediaPermission>(
                "The application needs permission to save media files",
                DisplayAlert);

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("The application did not get the necessary permission to save media files");
                return;
            }

            try
            {
#if __IOS__
                var res = NSBundle.MainBundle.PathForResource("dotnet_bot", ".png");
                await MediaGallery.SaveAsync(MediaFileType.Image, res);
#else
                using var bitmap = await BitmapFactory.DecodeResourceAsync(MauiApplication.Current.Resources, Resource.Drawable.dotnet_bot);
                using var stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);

               await MediaGallery.SaveAsync(MediaFileType.Image, stream.ToArray(), "dotnet_bot.png");

#endif
            }
            catch (Exception ex)
            {
                await DisplayAlert(ex.StackTrace);
            }
        }

        Task DisplayAlert(string message)
           => DisplayAlert(string.Empty, message, "ok");

    }
}
