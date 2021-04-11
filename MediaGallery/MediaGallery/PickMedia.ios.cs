using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MobileCoreServices;
using Photos;
using PhotosUI;
using UIKit;
using Xamarin.Essentials;

namespace Xamarin.MediaGallery
{
    public static partial class MediaGallery
    {
        static UIViewController pickerRef;

        static async Task<IEnumerable<IMediaFile>> PlatformPickAsync(int selectionLimit, params MediaFileType[] types)
        {
            var vc = GetCurrentUIViewController();

            var isVideo = types.Contains(MediaFileType.Video);
            var isImage = types.Contains(MediaFileType.Image);

            var tcs = new TaskCompletionSource<IEnumerable<IMediaFile>>();

            if (HasOSVersion(14))
            {
                var config = new PHPickerConfiguration();
                config.SelectionLimit = selectionLimit;

                if (!(isVideo && isImage))
                    config.Filter = isVideo
                        ? PHPickerFilter.VideosFilter
                        : PHPickerFilter.ImagesFilter;

                var picker = new PHPickerViewController(config);
                picker.Delegate = new PPD
                {
                    CompletedHandler = res =>
                        tcs.TrySetResult(PickerResultsToMediaFile(res))
                };

                pickerRef = picker;
            }
            else
            {
                var sourceType = UIImagePickerControllerSourceType.PhotoLibrary;

                if (!UIImagePickerController.IsSourceTypeAvailable(sourceType))
                    throw new FeatureNotSupportedException();

                var availableTypes = UIImagePickerController.AvailableMediaTypes(sourceType);
                isVideo = isVideo && availableTypes.Contains(UTType.Movie);
                isImage = isImage && availableTypes.Contains(UTType.Image);
                if (!(isVideo || isImage))
                    throw new FeatureNotSupportedException();

                var picker = new UIImagePickerController();
                picker.SourceType = sourceType;
                picker.MediaTypes = isVideo && isImage
                    ? new string[] { UTType.Movie, UTType.Image }
                    : new string[] { isVideo ? UTType.Movie : UTType.Image };
                picker.AllowsEditing = false;
                picker.AllowsImageEditing = false;

                picker.Delegate = new PhotoPickerDelegate
                {
                    CompletedHandler = res =>
                    {
                        var result = DictionaryToMediaFile(res);
                        tcs.TrySetResult(result == null ? null : new IMediaFile[] { result });
                    }
                };

                pickerRef = picker;
            }

            if (DeviceInfo.Idiom == DeviceIdiom.Tablet && pickerRef.PopoverPresentationController != null && vc.View != null)
                pickerRef.PopoverPresentationController.SourceRect = vc.View.Bounds;

            await vc.PresentViewControllerAsync(pickerRef, true);

            var result = await tcs.Task;

            await vc.DismissViewControllerAsync(true);

            pickerRef?.Dispose();
            pickerRef = null;

            return result;
        }

        static IMediaFile DictionaryToMediaFile(NSDictionary info)
        {
            if (info == null)
                return null;

            PHAsset phAsset = null;
            NSUrl assetUrl = null;

            if (HasOSVersion(11))
            {
                assetUrl = info[UIImagePickerController.ImageUrl] as NSUrl;

                // Try the MediaURL sometimes used for videos
                if (assetUrl == null)
                    assetUrl = info[UIImagePickerController.MediaURL] as NSUrl;

                if (assetUrl != null)
                {
                    if (!assetUrl.Scheme.Equals("assets-library", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var doc = new UIDocument(assetUrl);
                        var fullPath = doc.FileUrl?.Path;

                        return MediaFile.Create(
                            doc.LocalizedName ?? Path.GetFileNameWithoutExtension(fullPath),
                            () => Task.FromResult<Stream>(File.OpenRead(fullPath)),
                            null,
                            assetUrl.PathExtension);
                    }

                    phAsset = info.ValueForKey(UIImagePickerController.PHAsset) as PHAsset;
                }
            }

            if (phAsset == null)
            {
                assetUrl = info[UIImagePickerController.ReferenceUrl] as NSUrl;

                if (assetUrl != null)
                    phAsset = PHAsset.FetchAssets(new NSUrl[] { assetUrl }, null)?.LastObject as PHAsset;
            }

            if (phAsset == null || assetUrl == null)
            {
                var img = info.ValueForKey(UIImagePickerController.OriginalImage) as UIImage;

                if (img != null)
                    return MediaFile.Create(
                        Guid.NewGuid().ToString(),
                        () => Task.FromResult(img.AsJPEG().AsStream()),
                        UTType.JPEG);
            }

            if (phAsset == null || assetUrl == null)
                return null;

            string originalFilename;

            if (HasOSVersion(9))
                originalFilename = PHAssetResource.GetAssetResources(phAsset).FirstOrDefault()?.OriginalFilename;
            else
                originalFilename = phAsset.ValueForKey(new NSString("filename")) as NSString;

            return MediaFile.Create(
                originalFilename,
                () =>
                {
                    var tcsStream = new TaskCompletionSource<Stream>();

                    PHImageManager.DefaultManager.RequestImageData(phAsset, null, new PHImageDataHandler((data, str, orientation, dict) =>
                        tcsStream.TrySetResult(data.AsStream())));

                    return tcsStream.Task;
                },
                null,
                assetUrl.PathExtension);
        }

        static IEnumerable<IMediaFile> PickerResultsToMediaFile(PHPickerResult[] results)
        {
            if (results != null)
                foreach (var res in results)
                {
                    var item = res.ItemProvider;
                    var identifier = item.RegisteredTypeIdentifiers?.FirstOrDefault();

                    yield return MediaFile.Create(
                        item.SuggestedName,
                        async () =>
                        {
                            var data = await item.LoadDataRepresentationAsync(identifier);
                            var stream = data.AsStream();
                            return stream;
                        },
                        identifier);
                }
        }

        class PhotoPickerDelegate : UIImagePickerControllerDelegate
        {
            public Action<NSDictionary> CompletedHandler { get; set; }

            public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info) =>
                CompletedHandler?.Invoke(info);

            public override void Canceled(UIImagePickerController picker) =>
                CompletedHandler?.Invoke(null);
        }

        class PPD : PHPickerViewControllerDelegate
        {
            public Action<PHPickerResult[]> CompletedHandler { get; set; }

            public override void DidFinishPicking(PHPickerViewController picker, PHPickerResult[] results) =>
                CompletedHandler?.Invoke(results);
        }

        static UIViewController GetCurrentUIViewController()
            => Platform.GetCurrentUIViewController()
            ?? throw new InvalidOperationException("Could not find current view controller.");
    }
}
