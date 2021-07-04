using System;
using System.Linq;
using System.Threading.Tasks;
using NativeMedia;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Internals;

namespace Sample.Views
{
    public partial class PopupPickPage
    {
        [Preserve]
        public PopupPickPage()
        {
            InitializeComponent();
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                _ = Task.Run(async () =>
                  {
                      await Task.Delay(1500);
                      await PopupNavigation.Instance.PopAllAsync();
                  });

                var ss = await MediaGallery.PickAsync();

                if (ss?.Files?.Any() ?? false)
                    foreach (var file in ss.Files)
                        file.Dispose();

                await DisplayAlert(string.Empty, "Successfully", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert(string.Empty, ex.Message, "Ok");
            }
        }
    }
}
