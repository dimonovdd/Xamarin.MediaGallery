using System.Text;
using System.Windows.Input;
using MetadataExtractor;
using NativeMedia;
using Sample.Helpers;

namespace Sample.ViewModels;

public class MediaFileInfoVM : BaseVM
{
    bool isFirstAppiring = true;

    public MediaFileInfoVM(IMediaFile file)
    {
        File = file;
        ShareCommand = new Command(async () => await ShareAsync());
    }

    public IMediaFile File { get; }

    public string? Metadata { get; private set; }

    public bool IsBusy { get; private set; }

    public ICommand ShareCommand { get; }
    
    string? Path { get; set; }

    public override void OnAppearing()
    {
        base.OnAppearing();

        if (!isFirstAppiring)
            return;

        isFirstAppiring = false;
        Task.Run(async () =>
        {
            IsBusy = true;
            try
            {
                await using var stream = await File.OpenReadAsync();

                var name = (string.IsNullOrWhiteSpace(File.NameWithoutExtension)
                    ? Guid.NewGuid().ToString()
                    : File.NameWithoutExtension)
                    + $".{File.Extension}";

                Path = await FilesHelper.SaveToCacheAsync(stream, name);
                stream.Position = 0;
                await ReadMeta(stream);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync(ex.Message);
            }
            IsBusy = false;
        });      
    }

    Task ShareAsync()
    {
        if (string.IsNullOrWhiteSpace(Path) || !System.IO.File.Exists(Path))
            return DisplayAlertAsync("Couldn't share the file");
        return Share.RequestAsync(new ShareFileRequest(new ShareFile(Path)));
    }

    async Task ReadMeta(Stream stream)
    {
        var meta = new StringBuilder();

        try
        {
            var directories = ImageMetadataReader.ReadMetadata(stream);

            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                    meta.AppendLine(tag.ToString());

                foreach (var error in directory.Errors)
                    meta.AppendLine("ERROR: " + error);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(ex.Message);
        }
        Metadata = meta.ToString();
    }
}
