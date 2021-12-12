using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using MobileCoreServices;
using Photos;
using PhotosUI;
using UIKit;

namespace NativeMedia
{
    public static partial class MediaGallery
    {
        static UIViewController pickerRef;
        static UIViewController cameraRef;

        static async Task<IEnumerable<IMediaFile>> PlatformPickAsync(MediaPickRequest request, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                var isVideo = request.Types.Contains(MediaFileType.Video);
                var isImage = request.Types.Contains(MediaFileType.Image);

                var tcs = new TaskCompletionSource<IEnumerable<IMediaFile>>(TaskCreationOptions.RunContinuationsAsynchronously);

                CancelTaskIfRequested(token, tcs);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    CancelTaskIfRequested(token, tcs);

                    if (Platform.HasOSVersion(14))
                    {
                        var config = new PHPickerConfiguration();
                        config.SelectionLimit = request.SelectionLimit;

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

                    CancelTaskIfRequested(token, tcs);
                    var vc = Platform.GetCurrentUIViewController();

                    if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
                    {
                        var rect = request.PresentationSourceBounds.ToRect();
                        pickerRef.ModalPresentationStyle = rect != UsingsHelper.EmptyRectangle
                            ? UIModalPresentationStyle.Popover
                            : UIModalPresentationStyle.PageSheet;

                        if (pickerRef.PopoverPresentationController != null)
                        {
                            pickerRef.PopoverPresentationController.SourceView = vc.View;
                            pickerRef.PopoverPresentationController.SourceRect = rect.AsCGRect();
                        }
                    }

                    ConfigureController(pickerRef, tcs);

                    CancelTaskIfRequested(token, tcs);

                    vc.PresentViewController(pickerRef, true, () => PresentViewControllerHandler(pickerRef, token, tcs));
                });

                CancelTaskIfRequested(token, tcs, false);
                return await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                pickerRef?.Dispose();
                pickerRef = null;
            }
        }

        static bool PlatformCheckCapturePhotoSupport()
            => UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera);

        static async Task<IMediaFile> PlatformCapturePhotoAsync(CancellationToken token)
        {
            var tcs = new TaskCompletionSource<IEnumerable<IMediaFile>>(TaskCreationOptions.RunContinuationsAsynchronously);
            CancelTaskIfRequested(token, tcs, false);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CancelTaskIfRequested(token, tcs, false);

                cameraRef = new UIImagePickerController
                {
                    SourceType = UIImagePickerControllerSourceType.Camera,
                    AllowsEditing = false,
                    AllowsImageEditing = false,
                    Delegate = new PhotoPickerDelegate(tcs),
                    CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo
                };

                CancelTaskIfRequested(token, tcs, false);
                var vc = Platform.GetCurrentUIViewController();

                ConfigureController(cameraRef, tcs);


                CancelTaskIfRequested(token, tcs, false);
                vc.PresentViewController(cameraRef, true, () => PresentViewControllerHandler(cameraRef, token, tcs));
            });

            var res = await tcs.Task.ConfigureAwait(false);
            return res?.FirstOrDefault();
        }

        static void ConfigureController(UIViewController controller, TaskCompletionSource<IEnumerable<IMediaFile>> tcs)
        {
            if (controller.PresentationController != null)
                controller.PresentationController.Delegate = new PresentatControllerDelegate(tcs);
        }

        static void CancelTaskIfRequested(CancellationToken token, TaskCompletionSource<IEnumerable<IMediaFile>> tcs, bool needThrow = true)
        {
            if (!token.IsCancellationRequested)
                return;
            tcs?.TrySetCanceled(token);
            if (needThrow)
                token.ThrowIfCancellationRequested();
        }

        static void PresentViewControllerHandler(UIViewController controller, CancellationToken token, TaskCompletionSource<IEnumerable<IMediaFile>> tcs)
        {
            if (!token.CanBeCanceled)
                return;

            token.Register(
                ()=> MainThread.BeginInvokeOnMainThread(
                    ()=> controller?.DismissViewController(true,
                        ()=> tcs?.TrySetCanceled(token))));
        }

        class PHPickerDelegate : PHPickerViewControllerDelegate
        {
            readonly TaskCompletionSource<IEnumerable<IMediaFile>> tcs;

            internal PHPickerDelegate(TaskCompletionSource<IEnumerable<IMediaFile>> tcs)
                => this.tcs = tcs;

            public override void DidFinishPicking(PHPickerViewController picker, PHPickerResult[] results)
            {
                picker.DismissViewController(true, null);
                tcs?.TrySetResult(results?.Length > 0 ? ConvertPickerResults(results) : null);
            }

            static IEnumerable<IMediaFile> ConvertPickerResults(PHPickerResult[] results)
                => results
                .Select(res => res.ItemProvider)
                .Where(provider => provider != null && provider.RegisteredTypeIdentifiers?.Length > 0)
                .Select(provider => new PHPickerFile(provider))
                .ToArray();
        }

        class PhotoPickerDelegate : UIImagePickerControllerDelegate
        {
            readonly TaskCompletionSource<IEnumerable<IMediaFile>> tcs;

            internal PhotoPickerDelegate(TaskCompletionSource<IEnumerable<IMediaFile>> tcs)
                => this.tcs = tcs;

            public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
            {
                picker.DismissViewController(true, null);
                var result = ConvertPickerResults(info);
                tcs.TrySetResult(result == null ? null : new IMediaFile[] { result });
            }

            public override void Canceled(UIImagePickerController picker)
            {
                picker.DismissViewController(true, null);
                tcs?.TrySetResult(null);
            }

            IMediaFile ConvertPickerResults(NSDictionary info)
            {
                if (info == null)
                    return null;

                var assetUrl = (info.ValueForKey(UIImagePickerController.ImageUrl)
                    ?? info.ValueForKey(UIImagePickerController.MediaURL)) as NSUrl;

                var path = assetUrl?.Path;

                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                    return new UIDocumentFile(assetUrl, GetOriginalName(info));

                assetUrl?.Dispose();
                var img = info.ValueForKey(UIImagePickerController.OriginalImage) as UIImage;
                var meta = info.ValueForKey(UIImagePickerController.MediaMetadata) as NSDictionary;

                if (img != null && meta != null)
                    return new PhotoFile(img, meta, GetNewImageName());

                return null;
            }

            string GetOriginalName(NSDictionary info)
            {
                if (PHPhotoLibrary.AuthorizationStatus != PHAuthorizationStatus.Authorized
                    || !info.ContainsKey(UIImagePickerController.PHAsset))
                    return null;

                using var asset = info.ValueForKey(UIImagePickerController.PHAsset) as PHAsset;

                return asset != null
                    ? PHAssetResource.GetAssetResources(asset)?.FirstOrDefault()?.OriginalFilename
                    : null;
            }
        }

        class PresentatControllerDelegate : UIAdaptivePresentationControllerDelegate
        {
            readonly TaskCompletionSource<IEnumerable<IMediaFile>> tcs;

            internal PresentatControllerDelegate(TaskCompletionSource<IEnumerable<IMediaFile>> tcs)
                => this.tcs = tcs;

            public override void DidDismiss(UIPresentationController presentationController)
                => tcs?.TrySetResult(null);

            protected override void Dispose(bool disposing)
            {
                tcs?.TrySetResult(null);
                base.Dispose(disposing);
            }
        }
    }
}
