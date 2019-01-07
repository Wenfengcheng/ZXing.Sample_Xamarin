using Android.Animation;
using Android.App;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Plugin.CurrentActivity;
using Sample.Droid;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceService))]
namespace Sample.Droid
{
    public class DeviceService : IDeviceService
    {
        public async Task<string> ScanAsync()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner
            {
                UseCustomOverlay = true
            };

            var options = new ZXing.Mobile.MobileBarcodeScanningOptions()
            {
                TryHarder = true,
                AutoRotate = false,
                UseFrontCameraIfAvailable = false,
                CameraResolutionSelector = availableResolutions =>
                {
                    foreach (var ar in availableResolutions)
                    {
                        System.Diagnostics.Debug.WriteLine("Resolution: " + ar.Width + "x" + ar.Height);
                    }
                    return availableResolutions[availableResolutions.Count - 1];
                },
                PossibleFormats = new System.Collections.Generic.List<ZXing.BarcodeFormat>()
                {
                    ZXing.BarcodeFormat.QR_CODE
                }
            };

            View scanView = LayoutInflater.From(Application.Context).Inflate(Resource.Layout.ScanView, null);
            ImageView imgLine = scanView.FindViewById<ImageView>(Resource.Id.imgLine);
            ImageView imgClose = scanView.FindViewById<ImageView>(Resource.Id.imgClose);
            imgClose.Click += delegate
            {
                scanner.Cancel();
            };
            scanner.CustomOverlay = scanView;

            ObjectAnimator objectAnimator = ObjectAnimator.OfFloat(imgLine, "Y", 0, DpToPixels(240));
            objectAnimator.SetDuration(2500);
            objectAnimator.RepeatCount = -1;
            objectAnimator.SetInterpolator(new LinearInterpolator());
            objectAnimator.RepeatMode = ValueAnimatorRepeatMode.Restart;
            objectAnimator.Start();

            ZXing.Result scanResults = await scanner.Scan(CrossCurrentActivity.Current.Activity, options);
            if (scanResults != null)
            {
                return scanResults.Text;
            }
            return string.Empty;
        }

        private int DpToPixels(double dp)
        {
            return (int)(dp * Application.Context.Resources.DisplayMetrics.Density);
        }
    }
}