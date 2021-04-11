using System;
using Xamarin.MediaGallery;

namespace Sample.Models
{
    public class CacheItem
    {
        public CacheItem(string path, MediaFileType type)
        {
            Path = path;
            Type = type;
        }

        public string Path { get; }

        public MediaFileType Type { get; }
    }
}
