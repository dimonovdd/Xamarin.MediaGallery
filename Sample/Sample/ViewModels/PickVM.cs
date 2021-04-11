using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Sample.Helpers;
using Sample.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.MediaGallery;

namespace Sample.ViewModels
{
    public class PickVM : BaseVM
    {
        public PickVM()
        {
            PickAnyCommand = new Command(async () => await Pick());
            PickImageCommand = new Command(async () => await Pick(MediaFileType.Image));
            PickVideoCommand = new Command(async () => await Pick(MediaFileType.Video));
        }

        public int SelectionLimit { get; set; } = 3;

        public IEnumerable<CacheItem> SelectedItems { get; set; }

        public ICommand PickAnyCommand { get; }

        public ICommand PickImageCommand { get; }

        public ICommand PickVideoCommand { get; }


        async Task Pick(params MediaFileType[] types)
        {
            var results = await MediaGallery.PickAsync(SelectionLimit, types);

            if (results?.Files == null)
                return;

            var items = new List<CacheItem>();
            foreach(var res in results.Files)
            {
                if (res.Type == null)
                    return;

                using var stream = await res.OpenReadAsync();
                var path = await FilesHelper.SaveToCacheAsync(
                    stream,
                    res.FileName);
                items.Add(new CacheItem(path, (MediaFileType)res.Type));
            }

            MainThread.BeginInvokeOnMainThread(()=> SelectedItems = items);
        }
    }
}
