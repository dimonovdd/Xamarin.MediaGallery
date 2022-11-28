using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace Sample.Maui
{
    public partial class MediaFileInfoPage : BasePage
    {
        bool isDroid;

        public MediaFileInfoPage()
        {
            InitializeComponent();
            isDroid = DeviceInfo.Platform == DevicePlatform.Android;
        }

        protected override void OnAppearing()
        {
            MetaLabel.PropertyChanged += MetaLabel_PropertyChanged;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MetaLabel.PropertyChanged += MetaLabel_PropertyChanged;
            base.OnDisappearing();
        }

        //ugly hack to fix problem with cropping large text in metadata on Droid
        private void MetaLabel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (isDroid && e.PropertyName == nameof(Label.Text))
                Scroll.ForceLayout();
        }
    }
}
