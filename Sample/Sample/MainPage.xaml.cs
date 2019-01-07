using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void ScannerViewButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ScannerPage());
        }

        private async void DependencyButton_Clicked(object sender, EventArgs e)
        {
            var result = await DependencyService.Get<IDeviceService>().ScanAsync();
            if(!string.IsNullOrEmpty(result))
            {
                await DisplayAlert(result, null, "OK");
            }
        }
    }
}
