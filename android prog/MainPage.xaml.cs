using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading;
using System.IO;

namespace Manager_app
{
    public partial class MainPage : ContentPage
    {
        private Requests requests;
        private string state;
        private int timer;

        public MainPage()
        {
            InitializeComponent();

            requests = new Requests();
            Ip_Label.Text = requests.Ip;

            SetStatus("sync", true);

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    for (int timer = 10; timer >= 1; timer--)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            BUTTON_SYNC.Text = $"Sync (in {timer})";
                        });
                        Thread.Sleep(1000);
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        SetStatus("sync", true);
                    });
                }
            }).Start();
        }

        private async void OnButtonClick(object sender, EventArgs e) => SetStatus("on", false);

        private async void OffButtonClick(object sender, EventArgs e) => SetStatus("off", false);

        private async void SyncButtonClick(object sender, EventArgs e) => SetStatus("sync", true);


        private async void SetStatus(string endpoint, bool isSync)
        {
            L_Status.Text = "...";

            string r = await requests.SendRequestAsync(endpoint, isSync);
            string[] result = r.Split(' ');

            if (result[0] == "200")
            {
                L_Status.Text = result[1];
            }
            else if (result[0] == "102")
            {
                L_Status.Text = result[1];
            }
            else L_Status.Text = "SetStatus() error";
        }
    }
}
