using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using NativeMedia;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Sample.ViewModels
{
    
    public class PickVM : BaseVM
    {
        private int delayMilliseconds = 5000;

        public PickVM()
        {
            PickAnyCommand = new Command(async () => await Pick());
            PickImageCommand = new Command(async () => await Pick(MediaFileType.Image));
            PickVideoCommand = new Command(async () => await Pick(MediaFileType.Video));
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

        public IEnumerable<IMediaFile> SelectedItems { get; set; }

        public ICommand PickAnyCommand { get; }

        public ICommand PickImageCommand { get; }

        public ICommand PickVideoCommand { get; }

        [Preserve]
        public ICommand OpenInfoCommand { get; }


        async Task Pick(params MediaFileType[] types)
        {
            try
            {
                if (SelectedItems?.Count() > 0)
                    if (SelectedItems?.Any() ?? false)
                        foreach (var item in SelectedItems)
                            item.Dispose();
                SelectedItems = null;

                var cts = new CancellationTokenSource(
                    TimeSpan.FromMilliseconds(DelayMilliseconds));

                var task = MediaGallery.PickAsync(SelectionLimit, cts.Token, types);

                var result = await task;

                SelectedItems = result?.Files?.ToArray();
    }
            catch(Exception ex)
            {
                await DisplayAlertAsync(ex.Message);
            }
        }
    }
}
