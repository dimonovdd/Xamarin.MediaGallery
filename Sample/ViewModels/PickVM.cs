using System.Windows.Input;
using NativeMedia;
using Sample.Helpers;

namespace Sample.ViewModels;

public class PickVM : BaseVM
{
    int delayMilliseconds = 5000000;

    public PickVM()
    {
        PickAnyCommand = new Command(() => Pick(null));
        PickImageCommand = new Command<View>(view => Pick(view, MediaFileType.Image));
        PickVideoCommand = new Command<View>(view => Pick(view, MediaFileType.Video));
        CapturePhotoCommand = new Command(async () => await CapturePhotoAsync());
        OpenInfoCommand = new Command<IMediaFile>(async file => await NavigateAsync(new MediaFileInfoVM(file)));
    }

    public int SelectionLimit { get; set; } = 3;

    public int DelayMilliseconds
    {
        get => delayMilliseconds;
        set => delayMilliseconds = value > 0 ? value : 1;
    }

    public string? OperationInfo { get; set; }

    public IEnumerable<IMediaFile>? SelectedItems { get; set; }

    public ICommand PickAnyCommand { get; }

    public ICommand PickImageCommand { get; }

    public ICommand PickVideoCommand { get; }

    public ICommand CapturePhotoCommand { get; }

    public ICommand OpenInfoCommand { get; }


    void Pick(View? view, params MediaFileType[] types) =>
        Task.Run(async () =>
        {
            CancellationTokenSource? cts = null;
            try
            {
                DisposeItems();

                cts = new CancellationTokenSource(
                    TimeSpan.FromMilliseconds(DelayMilliseconds));

                var result = await MediaGallery.PickAsync(
                    new MediaPickRequest(SelectionLimit, types)
                    {
                        Title = $"Select {SelectionLimit} photos",
                        PresentationSourceBounds = view?.GetAbsoluteBounds(40),
                    },
                    cts.Token);

                SelectedItems = result?.Files;

                SetInfo(SelectedItems);
            }
            catch (Exception ex)
            {
                OperationInfo = ex.Message;
            }
            finally
            {
                cts?.Dispose();
            }
        });

    async Task CapturePhotoAsync()
    {
        CancellationTokenSource? cts = null;
        try
        {
            DisposeItems();
            if (!MediaGallery.CheckCapturePhotoSupport())
            {
                OperationInfo = "Capture Photo Not Supported";
                return;
            }

            var status = await CheckAndRequestAsync<Permissions.Camera>(
                "The application needs permission to camera",
                "To grant access to camera, go to settings");

            if (!status)
            {
                await DisplayAlertAsync("The application did not get the necessary permission to camera");
                return;
            }

            cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(DelayMilliseconds));

            var file = await MediaGallery.CapturePhotoAsync(cts.Token);

            SelectedItems = file != null ? new[] { file } : null;
            SetInfo(SelectedItems);
        }
        catch (Exception ex)
        {
            OperationInfo = ex.Message;
        }
        finally
        {
            cts?.Dispose();
        }
    }

    void DisposeItems()
    {
        if (SelectedItems?.Any() ?? false)
            foreach (var item in SelectedItems)
                item?.Dispose();
        SelectedItems = null;
    }

    void SetInfo(IEnumerable<IMediaFile>? files)
        => OperationInfo = files?.Any() ?? false
            ? "Successfully"
            : "Media files not selected";
}