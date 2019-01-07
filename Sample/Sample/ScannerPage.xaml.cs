using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sample
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScannerPage : ContentPage
	{
		public ScannerPage ()
		{
			InitializeComponent ();
            scanner.IsScanning = true;
            scanner.IsAnalyzing = true;
            scanner.Options = new ZXing.Mobile.MobileBarcodeScanningOptions()
            {
                DelayBetweenContinuousScans = 2000
            };
            scanner.OnScanResult += Scanner_OnScanResult;
        }

        private void Scanner_OnScanResult(ZXing.Result result)
        {
            Console.WriteLine($"scan-result......{result.Text}");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(() =>
            {
                var animation = new Animation(v => line.TranslationY = v, 0, 240);
                animation.Commit(this, "SimpleAnimation", 16, 2500, Easing.Linear, (v, c) => line.TranslationY = 240, () => true);
            });
        }
    }
}