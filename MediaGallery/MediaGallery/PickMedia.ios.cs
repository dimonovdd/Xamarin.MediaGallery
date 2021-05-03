﻿using System;
using System.Collections.Generic;
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

        static async Task<IEnumerable<IMediaFile>> PlatformPickAsync(int selectionLimit, CancellationToken token, params MediaFileType[] types)
        {
            var vc = Platform.GetCurrentUIViewController();

            var isVideo = types.Contains(MediaFileType.Video);
            var isImage = types.Contains(MediaFileType.Image);

            var tcs = new TaskCompletionSource<IEnumerable<IMediaFile>>();

            if (Platform.HasOSVersion(14))
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

            if (token.IsCancellationRequested)
                Finish();
            else if (token.CanBeCanceled)
                token.Register(() =>
                {
                    MainThread.BeginInvokeOnMainThread(()
                        => pickerRef?.DismissViewController(true, null));
                    tcs.TrySetResult(null);
                });

            await vc.PresentViewControllerAsync(pickerRef, true);

            var result = await tcs.Task;
            Finish();

            return result;

            void Finish()
            {
                pickerRef?.Dispose();
                pickerRef = null;
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
            }
        }

        static IMediaFile ConvertPickerResults(NSDictionary info)
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

        static IEnumerable<IMediaFile> ConvertPickerResults(PHPickerResult[] results)
        {
            foreach (var res in results)
                if(res?.ItemProvider != null)
                    yield return new PHPickerFile(res.ItemProvider);
        }

        static string GetOriginalName(NSDictionary info)
        {
            if (PHPhotoLibrary.AuthorizationStatus != PHAuthorizationStatus.Authorized
                || !info.ContainsKey(UIImagePickerController.PHAsset))
                return null;

            using var asset = info.ValueForKey(UIImagePickerController.PHAsset) as PHAsset;

            return asset != null
                ? PHAssetResource.GetAssetResources(asset)?.FirstOrDefault()?.OriginalFilename
                : null;
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
        }

        class PHPickerDelegate : PHPickerViewControllerDelegate
        {
            readonly TaskCompletionSource<IEnumerable<IMediaFile>> tcs;

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
            readonly TaskCompletionSource<IEnumerable<IMediaFile>> tcs;

            internal PresentatControllerDelegate(TaskCompletionSource<IEnumerable<IMediaFile>> tcs)
                => this.tcs = tcs;

            public override void DidDismiss(UIPresentationController presentationController)
                => tcs?.TrySetResult(null);
        }
    }
}
