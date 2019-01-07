using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace Sample
{
    public partial class MainPage : ContentPage
    {
        private ZXingScannerPage scanPage;
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Scan with default overlay
        /// </summary>
        private async void DefaultOverlayButton_Clicked(object sender, EventArgs e)
        {
            scanPage = new ZXingScannerPage();
            scanPage.OnScanResult += (result) => {
                scanPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                    DisplayAlert("Scanned Barcode", result.Text, "OK");
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        /// <summary>
        /// Scan continuously
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ScanContinuouslyButton_Clicked(object sender, EventArgs e)
        {
            scanPage = new ZXingScannerPage(new ZXing.Mobile.MobileBarcodeScanningOptions { DelayBetweenContinuousScans = 3000 });
            scanPage.OnScanResult += (result) => {
                scanPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(() => {
                    DisplayAlert("Scanned Barcode", result.Text, "OK");
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        /// <summary>
        /// Scan with custom overlay
        /// </summary>
        private async void CustomOverlayButton_Clicked(object sender, EventArgs e)
        {
            // Create our custom overlay
            var customOverlay = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            var torch = new Button
            {
                Text = "Toggle Torch"
            };
            torch.Clicked += delegate {
                scanPage.ToggleTorch();
            };
            customOverlay.Children.Add(torch);

            scanPage = new ZXingScannerPage(new ZXing.Mobile.MobileBarcodeScanningOptions { AutoRotate = true }, customOverlay: customOverlay);
            scanPage.OnScanResult += (result) => {
                scanPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    DisplayAlert("Scanned Barcode", result.Text, "OK");
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        /// <summary>
        /// Scan with custom page
        /// </summary>
        private async void CustomPageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ScannerPage());
        }

        /// <summary>
        /// Scan with platform custom overlay
        /// </summary>
        private async void DependencyButton_Clicked(object sender, EventArgs e)
        {
            var result = await DependencyService.Get<IDeviceService>().ScanAsync();
            if (!string.IsNullOrEmpty(result))
            {
                await DisplayAlert(result, null, "OK");
            }
        }

        /// <summary>
        /// Generate Barcode
        /// </summary>
        private async void GenerateBarcode_Clicked(object sender, EventArgs e)
        {
            var barcode = new ZXingBarcodeImageView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            barcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
            barcode.BarcodeOptions.Width = 300;
            barcode.BarcodeOptions.Height = 300;
            barcode.BarcodeOptions.Margin = 10;
            barcode.BarcodeValue = "Funky_Xamarin";

            var contentPage = new ContentPage()
            {
                Content = barcode
            };
            await Navigation.PushAsync(contentPage);
        }
        
    }
}
