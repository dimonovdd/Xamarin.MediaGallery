using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using NativeMedia;
using Sample.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Sample.ViewModels
{

    public class PickVM : BaseVM
    {
        private int delayMilliseconds = 5000;

        public PickVM()
        {
            PickAnyCommand = new Command(() => Pick(null));
            PickImageCommand = new Command<View>(view => Pick(view, MediaFileType.Image));
            PickVideoCommand = new Command<View>(view => Pick(view, MediaFileType.Video));
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

        public bool NeedUseCreateChooser { get; set; } = true;

        public IEnumerable<IMediaFile> SelectedItems { get; set; }

        public ICommand PickAnyCommand { get; }

        public ICommand PickImageCommand { get; }

        public ICommand PickVideoCommand { get; }

        [Preserve]
        public ICommand OpenInfoCommand { get; }


        void Pick(View view, params MediaFileType[] types)
        {
            Task.Run(async () =>
            {
                CancellationTokenSource cts = null;
                try
                {
                    if (SelectedItems?.Any() ?? false)
                        foreach (var item in SelectedItems)
                            item.Dispose();
                    SelectedItems = null;

                    cts = new CancellationTokenSource(
                        TimeSpan.FromMilliseconds(DelayMilliseconds));

                    Task<MediaPickResult> task = null;
                    try
                    {
                        task = MediaGallery.PickAsync(
                            new MediaPickRequest(SelectionLimit, types)
                            {
                                Title = $"Select {SelectionLimit} photos",
                                PresentationSourceBounds = view == null
                                    ? System.Drawing.Rectangle.Empty
                                    : view.GetAbsoluteBounds().ToSystemRectangle(40),
                                UseCreateChooser = NeedUseCreateChooser
                            },
                            cts.Token);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }


                    var result = await task;

                    SelectedItems = result?.Files;

                    OperationInfo = SelectedItems?.Any() ?? false
                        ? "Successfully"
                        : "Media files not selected";
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
    }
}
