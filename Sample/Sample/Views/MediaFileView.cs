using NativeMedia;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Sample.Views
{
    public class MediaFileView : ContentView
    {
        public static readonly BindableProperty FileProperty =
            BindableProperty.Create(
                nameof(File),
                typeof(IMediaFile),
                typeof(MediaFileView),
                null,
                BindingMode.OneWay,
                propertyChanged: (b, o, n) => ((MediaFileView)b).OnChanged());

        public IMediaFile File
        {
            get => (IMediaFile)GetValue(FileProperty);
            set => SetValue(FileProperty, value);
        }

        public static readonly BindableProperty FilePathProperty =
            BindableProperty.Create(
                nameof(FilePath),
                typeof(string),
                typeof(MediaFileView),
                null,
                BindingMode.OneWay,
                propertyChanged: (b, o, n) => ((MediaFileView)b).OnChanged());

        public string FilePath
        {
            get => (string)GetValue(FilePathProperty);
            set => SetValue(FilePathProperty, value);
        }

        private void OnChanged()
        {
            var path = FilePath;
            var file = File;

            if (file?.Type == null || string.IsNullOrWhiteSpace(path))
            {
                Content = null;
                return;
            }

            Content = file.Type switch
            {
                MediaFileType.Image => new Image { Source = path },
                MediaFileType.Video => new MediaElement { Source = path },
                _ => null
            };
        }
    }
}
