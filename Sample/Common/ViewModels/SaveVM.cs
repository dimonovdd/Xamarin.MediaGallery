using System;
using System.IO;
using System.Windows.Input;
using NativeMedia;
using Sample.Common.Helpers;
using Sample.Helpers;

namespace Sample.Common.ViewModels;

public class SaveVM : BaseVM
{
    public SaveVM()
    {
        SavevPngCommand = new Command(() => Save(MediaFileType.Image, EmbeddedMedia.baboonPng));
        SaveJpgCommand = new Command(() => Save(MediaFileType.Image, EmbeddedMedia.lomonosovJpg));
        SaveGifCommand = new Command(() => Save(MediaFileType.Image, EmbeddedMedia.newtonsCradleGif));
        SaveVideoCommand = new Command(() => Save(MediaFileType.Video, EmbeddedMedia.earthMp4));

        PngSource = EmbeddedResourceProvider.GetImageSource(EmbeddedMedia.baboonPng);
        JpgSource = EmbeddedResourceProvider.GetImageSource(EmbeddedMedia.lomonosovJpg);
        GifSource = EmbeddedResourceProvider.GetImageSource(EmbeddedMedia.newtonsCradleGif);
    }

    public bool FromStream { get; set; } = true;

    public bool FromByteArray { get; set; }

    public bool FromCacheDirectory { get; set; }

    public ImageSource PngSource { get; }

    public ImageSource JpgSource { get; }

    public ImageSource GifSource { get; }

    public ICommand SavevPngCommand { get; }

    public ICommand SaveJpgCommand { get; }

    public ICommand SaveGifCommand { get; }

    public ICommand SaveVideoCommand { get; }


    async void Save(MediaFileType type, string name)
    {
        var status = await PermissionHelper.CheckAndRequest<SaveMediaPermission>(
                "The application needs permission to save media files");

        if (!status)
        {
            await DisplayAlertAsync("The application did not get the necessary permission to save media files");
            return;
        }
        
        try
        {
            using var stream = EmbeddedResourceProvider.Load(name);

            if (FromStream)
            {
                await MediaGallery.SaveAsync(type, stream, name);
            }
            else if (FromByteArray)
            {
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                await MediaGallery.SaveAsync(type, memoryStream.ToArray(), name);
            }
            else if (FromCacheDirectory)
            {
                var filePath = await FilesHelper.SaveToCacheAsync(stream, name);

                await MediaGallery.SaveAsync(type, filePath);
            }

            await DisplayAlertAsync("Save Completed Successfully");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(ex.Message);
        }
    }
}
