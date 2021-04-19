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

                pickerRef = new PHPickerViewController(config)
                {
                    Delegate = new PHPickerDelegate(tcs)
                };
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

                pickerRef = new UIImagePickerController
                {
                    SourceType = sourceType,
                    AllowsEditing = false,
                    AllowsImageEditing = false,
                    Delegate = new PhotoPickerDelegate(tcs),
                    MediaTypes = isVideo && isImage
                        ? new string[] { UTType.Movie, UTType.Image }
                        : new string[] { isVideo ? UTType.Movie : UTType.Image }
                };
            }

            if (pickerRef.PresentationController != null)
                pickerRef.PresentationController.Delegate = new PresentatControllerDelegate(tcs);

            if (DeviceInfo.Idiom == DeviceIdiom.Tablet && pickerRef.PopoverPresentationController != null && vc.View != null)
                pickerRef.PopoverPresentationController.SourceRect = vc.View.Bounds;

            await vc.PresentViewControllerAsync(pickerRef, true);

            var result = await tcs.Task;

            pickerRef?.Dispose();
            pickerRef = null;

            return result;
        }

        static IMediaFile ConvertPickerResults(NSDictionary info)
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

        static IEnumerable<IMediaFile> ConvertPickerResults(PHPickerResult[] results)
        {
            foreach (var res in results)
                if(res?.ItemProvider != null)
                    yield return new PHPickerFile(res.ItemProvider);
        }

        class PhotoPickerDelegate : UIImagePickerControllerDelegate
        {
            TaskCompletionSource<IEnumerable<IMediaFile>> tcs;

            internal PhotoPickerDelegate(TaskCompletionSource<IEnumerable<IMediaFile>> tcs)
                => this.tcs = tcs;

            public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
            {
                picker.DismissViewController(true, null);
                var result = ConvertPickerResults(info);
                tcs.TrySetResult(result == null ? null : new IMediaFile[] { result });
            }

            public override void Canceled(UIImagePickerController picker)
                => tcs?.TrySetResult(null);
        }

        class PHPickerDelegate : PHPickerViewControllerDelegate
        {
            TaskCompletionSource<IEnumerable<IMediaFile>> tcs;

            internal PHPickerDelegate(TaskCompletionSource<IEnumerable<IMediaFile>> tcs)
                => this.tcs = tcs;

            public override void DidFinishPicking(PHPickerViewController picker, PHPickerResult[] results)
            {
                picker.DismissViewController(true, null);
                tcs?.TrySetResult(results == null ? null : ConvertPickerResults(results));
            }
        }

        class PresentatControllerDelegate : UIAdaptivePresentationControllerDelegate
        {
            TaskCompletionSource<IEnumerable<IMediaFile>> tcs;

            internal PresentatControllerDelegate(TaskCompletionSource<IEnumerable<IMediaFile>> tcs)
                => this.tcs = tcs;

            public override void DidDismiss(UIPresentationController presentationController) =>
                tcs?.TrySetResult(null);
        }
    }
}
