using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Media;
using System.Net;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Windows.Interop;
using Google.Cloud.Speech.V1;
using NAudio.Wave;
using System.Timers;

namespace EFT_Price_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;

        //Modifiers:
        private const uint MOD_NONE = 0x0000; //(none)
        private const uint MOD_ALT = 0x0001; //ALT
        private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT
        private const uint MOD_WIN = 0x0008; //WINDOWS
        //CAPS LOCK:
        private const uint VK_CAPITAL = 0x14;
        //F1
        private const uint VK_F1 = 0x70;

        public MainWindow()
        {
            InitializeComponent();
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/solid-solstice-316204-d35b67c50206.json"));
        }

        private IntPtr _windowHandle;
        private HwndSource _source;
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_SHIFT, VK_F1); //SHIFT + F1
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_F1)
                            {
                                record();   
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            base.OnClosed(e);
        }


        private void record()
        {

            // Sound player
            SoundPlayer soundPlayer1 = new SoundPlayer();
            soundPlayer1.SoundLocation = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/beep-start.wav");
            SoundPlayer soundPlaer2 = new SoundPlayer();
            soundPlaer2.SoundLocation = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/beep-end.wav");


            string outputFilePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/audio.wav");

            var waveSource = new WaveIn();
            waveSource.WaveFormat = new WaveFormat(16000, 1);
         
            // Redefine the audio writer instance with the given configuration
            WaveFileWriter RecordedAudioWriter = new WaveFileWriter(outputFilePath, waveSource.WaveFormat);

            waveSource.DataAvailable += (s, a) =>
            {
                RecordedAudioWriter.Write(a.Buffer, 0, a.BytesRecorded);
            };

            // When the Capturer Stops, dispose instances of the capturer and writer
            waveSource.RecordingStopped += (s, a) =>
            {
                RecordedAudioWriter.Dispose();
                RecordedAudioWriter = null;
                waveSource.Dispose();
            };

            var recordTimer = new Timer(2500);
            recordTimer.Elapsed += (s, e) => {
                Debug.WriteLine("STOP RECORDING");
                soundPlaer2.PlaySync();
                waveSource.StopRecording();
                recordTimer.Stop();
                recordTimer.Dispose();

                
                var speech = SpeechClient.Create();
                var config = new RecognitionConfig
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                    SampleRateHertz = 16000,
                    LanguageCode = LanguageCodes.English.UnitedStates
                };
                var audio = RecognitionAudio.FromFile(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/audio.wav"));

                var response = speech.Recognize(config, audio);

                if (response.Results.Count > 0)
                {
                    getItemDetails(response.Results[0].Alternatives[0].Transcript);
                }

            };
            recordTimer.AutoReset = false;

            recordTimer.Start();

            // Start audio recording !
            Debug.WriteLine("START RECORDING");
            soundPlayer1.PlaySync();
            waveSource.StartRecording();
        }

        private void getItemDetails(string itemName)
        {
            string html = string.Empty;
            string url = @"http://127.0.0.1:8090/item-details?itemName=" + itemName;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        html = reader.ReadToEnd();

                        dynamic itemDetails = JsonConvert.DeserializeObject(html);
                        string tier = itemDetails.tier;
                        tblockItemName.Text = itemDetails.name;
                        tblockItemPrice.Text = "₽ " + String.Format("{0:n0}", itemDetails.price);
                        tblockItemPricePerSlot.Text = "₽ " + String.Format("{0:n0}", itemDetails.pricePerSlot);

                        tblockItemTier.Text = tier;
                        if (tier == "S") tblockItemTier.Foreground = Brushes.Green;
                        else if (tier == "A") tblockItemTier.Foreground = Brushes.YellowGreen;
                        else if (tier == "B") tblockItemTier.Foreground = Brushes.Yellow;
                        else if (tier == "C") tblockItemTier.Foreground = Brushes.Gray;
                        else if (tier == "D") tblockItemTier.Foreground = Brushes.Orange;
                        else if (tier == "F") tblockItemTier.Foreground = Brushes.Red;
                        else tblockItemTier.Foreground = tblockItemPrice.Foreground;
                    }
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse eRes = (HttpWebResponse)e.Response;
                        if (eRes.StatusCode == HttpStatusCode.BadRequest)
                        {
                            tblockItemName.Text = "Item Not Found";
                            tblockItemPrice.Text = "N/A";
                            tblockItemPricePerSlot.Text = "N/A";
                            tblockItemTier.Text = "N/A";
                        }
                        else if (eRes.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            tblockItemName.Text = "Something Went Wrong, Please Try Again";
                            tblockItemPrice.Text = "N/A";
                            tblockItemPricePerSlot.Text = "N/A";
                            tblockItemTier.Text = "N/A";
                        }

                    }
                }
            });
        }
    }
}
