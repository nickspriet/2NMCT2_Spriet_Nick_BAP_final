using AForge.Video;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Thinktecture.IdentityModel.Client;
using ZXing;

namespace nmct.ba.cashlessproject.ui.medewerker.ViewModel
{
    public class ScanBadgeVM : ObservableObject, IPage
    {
        public static TokenResponse token = null;

        //naam pagina
        public string Name
        {
            get { return "Scan badge"; }
        }


        //constructor
        public ScanBadgeVM()
        {
            ScanBadgeVM.token = GetToken();

            if (ScanBadgeVM.token != null)
            {
                GetKassas(); 
                GetMedewerkers();
            }
        }

        #region "Ophalen Kassas"
        private ObservableCollection<Kassa> _kassas;
        public ObservableCollection<Kassa> Kassas
        {
            get { return _kassas; }
            set { _kassas = value; OnPropertyChanged("Kassas"); }
        }

        private async void GetKassas()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ScanBadgeVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Kassa");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Kassas = JsonConvert.DeserializeObject<ObservableCollection<Kassa>>(json);

                    ApplicationVM appvm = Application.Current.MainWindow.DataContext as ApplicationVM;
                    appvm.IngelogdeKassa = Kassas.FirstOrDefault(k => k.ID.ToString() == ConfigurationManager.AppSettings["KassaID"]);
                }
            }
        }
        #endregion

        #region "Ophalen Medewerkers"
        private ObservableCollection<Medewerker> _medewerkers;
        public ObservableCollection<Medewerker> Medewerkers
        {
            get { return _medewerkers; }
            set { _medewerkers = value; OnPropertyChanged("Medewerkers"); }
        }

        private static Medewerker _ingelogdeMedewerker;
        public static Medewerker IngelogdeMedewerker
        {
            get { return _ingelogdeMedewerker; }
            set { _ingelogdeMedewerker = value; }
        }

        private async void GetMedewerkers()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ScanBadgeVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Medewerker");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Medewerkers = JsonConvert.DeserializeObject<ObservableCollection<Medewerker>>(json);
                }
            }
        }
        #endregion




        private struct Device
        {
            public int Index;
            public string Name;
            public override string ToString()
            {
                return Name;
            }
        }

        private static CameraDevices camDevices;
        private Bitmap currentBitmapForDecoding;
        private static Thread decodingThread;
        private Result currentResult;

        public ICommand StartCameraScanCommand
        {
            get { return new RelayCommand(StartCameraScan); }
        }

        private void StartCameraScan()
        {
            camDevices = new CameraDevices();

            decodingThread = new Thread(DecodeBarcode);
            decodingThread.Start();
            LoadDevices();
        }

        private void LoadDevices()
        {
            //if (camDevices.Current != null)
            //{
            //    decodingThread.Abort();

            //    camDevices.Current.NewFrame -= Current_NewFrame;
            //    if (camDevices.Current.IsRunning)
            //    {
            //        camDevices.Current.SignalToStop();
            //    }
            //}
            ObservableCollection<Device> devices = new ObservableCollection<Device>();

            for (var index = 0; index < camDevices.Devices.Count; index++)
            {
                devices.Add(new Device { Index = index, Name = camDevices.Devices[index].Name });
            }

            camDevices.SelectCamera(((Device)(devices[0])).Index);
            camDevices.Current.NewFrame += Current_NewFrame;
            camDevices.Current.Start();

        }

        private void Current_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //if (IsDisposed) return;

            try
            {
                if (currentBitmapForDecoding == null)
                {
                    currentBitmapForDecoding = (Bitmap)eventArgs.Frame.Clone();
                }
                if (Application.Current != null) Application.Current.Dispatcher.Invoke(new Action<Bitmap>(ShowFrame), eventArgs.Frame.Clone());
            }
            catch (ObjectDisposedException)
            {
                // not sure, why....
            }
        }


        private void ShowFrame(Bitmap frame)
        {
            //if (PicSource.Width < frame.Width) PicSource.Width = frame.Width;
            //if (PicSource.Height < frame.Height) PicSource.Height = frame.Height;

            PicSource = BitMapToString(frame);
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        public byte[] BitMapToString(Bitmap bitmap)
        {
            // Convert the image to byte[]
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] imageBytes = stream.ToArray();

            return imageBytes;
        }

        private void DecodeBarcode()
        {
            var reader = new BarcodeReader();
            while (true)
            {
                if (currentBitmapForDecoding != null)
                {
                    var result = reader.Decode(currentBitmapForDecoding);
                    if (result != null)
                    {
                        if (Application.Current != null) Application.Current.Dispatcher.Invoke(new Action<Result>(ShowResult), result);
                    }
                    currentBitmapForDecoding.Dispose();
                    currentBitmapForDecoding = null;
                }
                Thread.Sleep(200);
            }
        }

        private byte[] _picSource;
        public byte[] PicSource
        {
            get { return _picSource; }
            set { _picSource = value; OnPropertyChanged("PicSource"); }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }


        private void ShowResult(Result result)
        {
            currentResult = result;

            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;

            if (!ScanBadgeVM.token.IsError && currentResult.BarcodeFormat.ToString() == "QR_CODE")
            {
                if (Medewerkers != null) IngelogdeMedewerker = Medewerkers.FirstOrDefault(m => m.ID.ToString() == currentResult.ToString());

                if (IngelogdeMedewerker != null)
                {
                    appvm.IngelogdeMedewerker = IngelogdeMedewerker.MedewerkerNaam;
                    ApplicationVM.HuidigeKassaMedewerker.Medewerker = IngelogdeMedewerker;

                    CloseCamera();
                    appvm.ChangePage(new BestellingVM());
                }
                else MessageBox.Show(App.Current.MainWindow, "Medewerker is nog niet geregistreerd, neem contact op met uw manager.", "Registratie medewerker", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private TokenResponse GetToken()
        {
            OAuth2Client client = new OAuth2Client(new Uri("http://localhost:50726/token"));
            string login = ConfigurationManager.AppSettings["GebruikersNaam"];
            string wachtwoord = ConfigurationManager.AppSettings["Wachtwoord"];
            return client.RequestResourceOwnerPasswordAsync(login, wachtwoord).Result;
        }

        public static void CloseCamera()
        {
            if (camDevices.Current != null)
            {
                decodingThread.Abort();

                //camDevices.Current.NewFrame -= Current_NewFrame;
                if (camDevices.Current.IsRunning)
                {
                    camDevices.Current.SignalToStop();
                }
                camDevices.Current = null;
            }
        }
    }
}
