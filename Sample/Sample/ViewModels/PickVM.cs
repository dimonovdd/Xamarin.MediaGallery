using System;
using System.Collections.Generic;
using System.Linq;
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
        public PickVM()
        {
            PickAnyCommand = new Command(async () => await Pick(null));
            PickImageCommand = new Command<View>(async view => await Pick(view, MediaFileType.Image));
            PickVideoCommand = new Command<View>(async view => await Pick(view, MediaFileType.Video));
            OpenInfoCommand = new Command<IMediaFile>(async file => await NavigateAsync(new MediaFileInfoVM(file)));
        }

        public int SelectionLimit { get; set; } = 3;

        public IEnumerable<IMediaFile> SelectedItems { get; set; }

        public ICommand PickAnyCommand { get; }

        public ICommand PickImageCommand { get; }

        public ICommand PickVideoCommand { get; }

        [Preserve]
        public ICommand OpenInfoCommand { get; }


        async Task Pick(View view, params MediaFileType[] types)
        {
            try
            {
                if (SelectedItems?.Count() > 0)
                    foreach (var item in SelectedItems)
                        item.Dispose();

                var result = await MediaGallery.PickAsync(
                    new MediaPickRequest(SelectionLimit, types)
                    {
                        PresentationSourceBounds = view == null
                            ? System.Drawing.Rectangle.Empty
                            : view.GetAbsoluteBounds().ToSystemRectangle(40)
                    });

                SelectedItems = result?.Files?.ToArray();
            }
            catch(Exception ex)
            {
                await DisplayAlertAsync(ex.Message);
            }
        }
    }
}
