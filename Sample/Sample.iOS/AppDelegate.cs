using System.Linq;
using Foundation;
using Rg.Plugins.Popup.Services;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Sample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();
            NativeMedia.Platform.Init(GetTopViewController);
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public UIViewController GetTopViewController()
        {
            if (!(PopupNavigation.Instance?.PopupStack?.Any() ?? false))
                return null;

            var vc = UIApplication.SharedApplication.Windows
                ?.FirstOrDefault(w => w.RootViewController != null && !(w.RootViewController is Rg.Plugins.Popup.IOS.Renderers.PopupPageRenderer))
                ?.RootViewController;

            if (vc is UINavigationController navController)
                vc = navController.ViewControllers.Last();
            
            return vc;
        }
    }
}
