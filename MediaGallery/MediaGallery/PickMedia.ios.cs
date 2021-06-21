using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using MobileCoreServices;
using Photos;
using PhotosUI;
using UIKit;
using Xamarin.Essentials;

namespace NativeMedia
{
    public static partial class MediaGallery
    {
        static UIViewController pickerRef;

        static async Task<IEnumerable<IMediaFile>> PlatformPickAsync(MediaPickRequest request, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                var isVideo = request.Types.Contains(MediaFileType.Video);
                var isImage = request.Types.Contains(MediaFileType.Image);

                var tcs = new TaskCompletionSource<IEnumerable<IMediaFile>>(TaskCreationOptions.RunContinuationsAsynchronously);

                CancelTaskIfRequested();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    CancelTaskIfRequested();

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

                    CancelTaskIfRequested();
                    var vc = Platform.GetCurrentUIViewController();

                    if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
                    {
                        pickerRef.ModalPresentationStyle
                            = request.PresentationSourceBounds != Rectangle.Empty
                            ? UIModalPresentationStyle.Popover
                            : UIModalPresentationStyle.PageSheet;

                        if (pickerRef.PopoverPresentationController != null)
                        {
                            pickerRef.PopoverPresentationController.SourceView = vc.View;
                            pickerRef.PopoverPresentationController.SourceRect
                            = request.PresentationSourceBounds.ToPlatformRectangle();
                        }
                    }

                    if (pickerRef.PresentationController != null)
                        pickerRef.PresentationController.Delegate = new PresentatControllerDelegate(tcs);

                    CancelTaskIfRequested();

                    vc.PresentViewController(pickerRef, true, () =>
                    {
                        if (!token.CanBeCanceled)
                            return;

                        token.Register(()
                            => MainThread.BeginInvokeOnMainThread(()
                                => pickerRef?.DismissViewController(true, ()
                                    => tcs?.TrySetCanceled(token))));
                    });
                });

                CancelTaskIfRequested(false);
                return await tcs.Task.ConfigureAwait(false);

                void CancelTaskIfRequested(bool needThrow = true)
                {
                    if (token.IsCancellationRequested)
                    {
                        tcs?.TrySetCanceled(token);
                        if (needThrow)
                            token.ThrowIfCancellationRequested();
                    }
                }
            }
            finally
            {
                pickerRef?.Dispose();
                pickerRef = null;
            }
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
                .Where(provider => provider != null)
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

                using var assetUrl = (info.ValueForKey(UIImagePickerController.ImageUrl)
                    ?? info.ValueForKey(UIImagePickerController.MediaURL)) as NSUrl;

                var path = assetUrl?.Path;

                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                    return null;

                return new UIDocumentFile(assetUrl, GetOriginalName(info));
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
        }
    }
}
