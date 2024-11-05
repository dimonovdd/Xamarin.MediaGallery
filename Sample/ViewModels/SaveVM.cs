using System.Windows.Input;
using NativeMedia;
using Sample.Helpers;

namespace Sample.ViewModels;

public class SaveVM : BaseVM
{
    public SaveVM()
    {
        SavePngCommand = new Command(() => Save(MediaFileType.Image, EmbeddedMedia.BaboonPng));
        SaveJpgCommand = new Command(() => Save(MediaFileType.Image, EmbeddedMedia.LomonosovJpg));
        SaveGifCommand = new Command(() => Save(MediaFileType.Image, EmbeddedMedia.NewtonsCradleGif));
        SaveVideoCommand = new Command(() => Save(MediaFileType.Video, EmbeddedMedia.EarthMp4));

        PngSource = EmbeddedResourceProvider.GetImageSource(EmbeddedMedia.BaboonPng);
        JpgSource = EmbeddedResourceProvider.GetImageSource(EmbeddedMedia.LomonosovJpg);
        GifSource = EmbeddedResourceProvider.GetImageSource(EmbeddedMedia.NewtonsCradleGif);
    }

    public bool FromStream { get; set; } = true;

    public bool FromByteArray { get; set; }

    public bool FromCacheDirectory { get; set; }

    public ImageSource? PngSource { get; }

    public ImageSource? JpgSource { get; }

    public ImageSource? GifSource { get; }

    public ICommand SavePngCommand { get; }

    public ICommand SaveJpgCommand { get; }

    public ICommand SaveGifCommand { get; }

    public ICommand SaveVideoCommand { get; }


    async void Save(MediaFileType type, string name)
    {
        var status = await CheckAndRequestAsync<SaveMediaPermission>(
            "The application needs permission to save media files",
            "To grant access save media files, go to settings");

        if (!status)
        {
            await DisplayAlertAsync("The application did not get the necessary permission to save media files");
            return;
        }

        try
        {
            await using var stream = EmbeddedResourceProvider.Load(name);

            if (stream is null)
                throw new NullReferenceException("EmbeddedResource not found");

            if (FromStream)
            {
                await MediaGallery.SaveAsync(type, stream, name);
            }
            else if (FromByteArray)
            {
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

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