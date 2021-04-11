using System;
using Sample.Models;
using Xamarin.Forms;
using Xamarin.MediaGallery;

namespace Sample.Views.Templates
{
    public class MediaTemplateSelector : DataTemplateSelector
    {

        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate VideoTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is CacheItem file)
            {
                return file.Type switch
                {
                    MediaFileType.Image => ImageTemplate,
                    _ => VideoTemplate
                };
            }

            throw new ArgumentOutOfRangeException(nameof(MediaTemplateSelector.OnSelectTemplate));
        }
    }
}
