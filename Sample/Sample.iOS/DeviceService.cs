using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using Sample.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceService))]
namespace Sample.iOS
{
    public class DeviceService : IDeviceService
    {
        public async Task<string> ScanAsync()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner()
            {
                UseCustomOverlay = true
            };

            var options = new ZXing.Mobile.MobileBarcodeScanningOptions()
            {
                TryHarder = true,
                AutoRotate = false,
                UseFrontCameraIfAvailable = false,
                PossibleFormats = new List<ZXing.BarcodeFormat>()
                {
                    ZXing.BarcodeFormat.QR_CODE
                }
            };

            ScannerOverlayView customOverlay = new ScannerOverlayView();
            customOverlay.OnCancel += () =>
            {
                scanner?.Cancel();
            };
            customOverlay.OnResume += () =>
            {
                scanner?.ResumeAnalysis();
            };
            customOverlay.OnPause += () =>
            {
                scanner?.PauseAnalysis();
            };
            scanner.CustomOverlay = customOverlay;


            ZXing.Result scanResults = null;
            scanResults = await scanner.Scan(options);
            //customOverlay.Dispose();
            if (scanResults != null)
            {
                return scanResults.Text;
            }
            return string.Empty;
        }
    }
}