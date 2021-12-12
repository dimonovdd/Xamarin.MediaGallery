using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using NativeMedia;
using Sample.Helpers;

namespace Sample.Common.ViewModels;

public class PickVM : BaseVM
{
    private int delayMilliseconds = 5000000;

    public PickVM()
    {
        PickAnyCommand = new Command(() => Pick(null));
        PickImageCommand = new Command<View>(view => Pick(view, MediaFileType.Image));
        PickVideoCommand = new Command<View>(view => Pick(view, MediaFileType.Video));
        CapturePhotoCommand = new Command(async () => await СapturePhotoAsync());
        OpenInfoCommand = new Command<IMediaFile>(async file => await NavigateAsync(new MediaFileInfoVM(file)));
    }

    public int SelectionLimit { get; set; } = 3;

    public int DelayMilliseconds
    {
        get => delayMilliseconds;
        set
        {
            delayMilliseconds = value > 0 ? value : 1;
        }
    }

    public string OperationInfo { get; set; }

    public bool UseCreateChooser { get; set; } = true;

    public IEnumerable<IMediaFile> SelectedItems { get; set; }

    public ICommand PickAnyCommand { get; }

    public ICommand PickImageCommand { get; }

    public ICommand PickVideoCommand { get; }

    public ICommand CapturePhotoCommand { get; }

    public ICommand OpenInfoCommand { get; }


    void Pick(View view, params MediaFileType[] types)
    {
        Task.Run(async () =>
        {
            CancellationTokenSource cts = null;
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
                        UseCreateChooser = UseCreateChooser
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
    }

    async Task СapturePhotoAsync()
    {
        CancellationTokenSource cts = null;
        try
        {
            DisposeItems();
            if (!MediaGallery.CheckCapturePhotoSupport())
            {
                OperationInfo = "Capture Photo Not Supported";
                return;
            }

            var status = await PermissionHelper.CheckAndRequest<Permissions.Camera>(
                    "The application needs permission to camera");

            if (!status)
            {
                await DisplayAlertAsync("The application did not get the necessary permission to camera");
                return;
            }

            cts = new CancellationTokenSource(
                    TimeSpan.FromMilliseconds(DelayMilliseconds));

            var file = await MediaGallery.CapturePhotoAsync(cts.Token);

            SelectedItems = file != null ?  new IMediaFile[] { file } : null;
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

    void SetInfo(IEnumerable<IMediaFile> files)
        => OperationInfo = files?.Any() ?? false
                    ? "Successfully"
                    : "Media files not selected";
}
