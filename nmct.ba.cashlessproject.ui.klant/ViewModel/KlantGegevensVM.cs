using AForge.Video;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ZXing;

namespace nmct.ba.cashlessproject.ui.klant.ViewModel
{
    public class KlantGegevensVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "Klantgegevens"; }
        }

        //constructor
        public KlantGegevensVM()
        {
            if (ApplicationVM.token != null)
            {
                GetKlanten();
                Huidig = StandbyVM.HuidigeKlant;
                ToonHuidigeKlantGegevens(Huidig);
                OnPropertyChanged("btnKaartOpladenIsEnabled");
                OnPropertyChanged("KaartOpladenVisibility");
            }
        }

        private ObservableCollection<Klant> _klanten;
        public ObservableCollection<Klant> Klanten
        {
            get { return _klanten; }
            set { _klanten = value; OnPropertyChanged("Klanten"); }
        }

        private Klant _huidig;
        public Klant Huidig
        {
            get { return _huidig; }
            set { _huidig = value; OnPropertyChanged("Huidig"); }
        }


        //alle klanten ophalen
        public async void GetKlanten()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/klant");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Klanten = JsonConvert.DeserializeObject<ObservableCollection<Klant>>(json);

                    OnPropertyChanged("KlantGegevensVisibility");
                    //Klant eersteKlant = Klanten.Where(k => k.ID == huidig.ID).FirstOrDefault();
                    //if (eersteKlant == null) SaveCustomer(huidig);    
                }
            }
        }

        #region "Properties KlantGegevens"
        //private string _customerName;
        //public string CustomerName
        //{
        //    get { return _customerName; }
        //    set { _customerName = value; OnPropertyChanged("CustomerName"); }
        //}

        //private string _customerFirstName;
        //public string CustomerFirstName
        //{
        //    get { return _customerFirstName; }
        //    set { _customerFirstName = value; OnPropertyChanged("CustomerFirstName"); }
        //}

        private string _klantNaam;
        public string KlantNaam
        {
            get { return _klantNaam; }
            set { _klantNaam = value; OnPropertyChanged("KlantNaam"); }
        }

        private string _cardNumber;
        public string CardNumber
        {
            get { return _cardNumber; }
            set { _cardNumber = value; OnPropertyChanged("CardNumber"); }
        }

        private string _geboortedatum;
        public string Geboortedatum
        {
            get { return _geboortedatum; }
            set { _geboortedatum = value; OnPropertyChanged("Geboortedatum"); }
        }

        //private string _street;
        //public string Street
        //{
        //    get { return _street; }
        //    set { _street = value; OnPropertyChanged("Street"); }
        //}

        //private string _number;
        //public string Number
        //{
        //    get { return _number; }
        //    set { _number = value; OnPropertyChanged("Number"); }
        //}

        //private string _city;
        //public string City
        //{
        //    get { return _city; }
        //    set { _city = value; OnPropertyChanged("City"); }
        //}

        //private string _zipCode;
        //public string ZipCode
        //{
        //    get { return _zipCode; }
        //    set { _zipCode = value; OnPropertyChanged("ZipCode"); }
        //}

        private string _adresLijn1;
        public string AdresLijn1
        {
            get { return _adresLijn1; }
            set { _adresLijn1 = value; OnPropertyChanged("AdresLijn1"); }
        }

        private string _adresLijn2;
        public string AdresLijn2
        {
            get { return _adresLijn2; }
            set { _adresLijn2 = value; OnPropertyChanged("AdresLijn2"); }
        }

        private string _nationaliteit;
        public string Nationaliteit
        {
            get { return _nationaliteit; }
            set { _nationaliteit = value; OnPropertyChanged("Nationaliteit"); }
        }

        private byte[] _picture;
        public byte[] Picture
        {
            get { return _picture; }
            set { _picture = value; OnPropertyChanged("Picture"); }
        }

        private double _balance;
        public double Balance
        {
            get { return _balance; }
            set { _balance = value; OnPropertyChanged("Balance"); }
        }
        #endregion


        private void ToonHuidigeKlantGegevens(Klant huidig)
        {
            //CustomerName = huidig.CustomerName;
            //CustomerFirstName = huidig.CustomerFirstName;
            KlantNaam = "Hallo " + huidig.KlantVoornaam + " " + huidig.KlantNaam;
            CardNumber = huidig.KaartNummer;
            Geboortedatum = huidig.Geboortedatum.ToShortDateString();
            //Street = huidig.Street;
            //Number = huidig.Number;
            AdresLijn1 = huidig.Straat + " " + huidig.Nummer;
            AdresLijn2 = huidig.Postcode + " " + huidig.Stad;
            Nationaliteit = huidig.Nationaliteit;
            Picture = huidig.PasFoto;
            Balance = huidig.Saldo;
        }


        //bestaande of nieuwe klant ?
        //  - bestaande klant: klant ziet button om kaart op te laden.
        //  - nieuwe klant: klant ziet button om zichzelf te registreren
        public Visibility KlantGegevensVisibility
        {
            get
            {
                if (Klanten != null)
                {
                    //Klant huidig = StandbyVM.HuidigeKlant;
                    Klant bestaandeKlant = Klanten.Where(k => k.RijksRegNummer == Huidig.RijksRegNummer).FirstOrDefault();

                    if (bestaandeKlant == null) return Visibility.Visible;
                    else return Visibility.Hidden;
                }
                else return Visibility.Visible;
            }
        }


        //nieuwe klant registreren
        public ICommand SaveCustomerCommand
        {
            get { return new RelayCommand<Klant>(SaveCustomer); }
        }

        private async void SaveCustomer(Klant nieuweKlant)
        {
            string input = JsonConvert.SerializeObject(nieuweKlant);

            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.PostAsync("http://localhost:50726/api/Klant", new StringContent(input, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    string output = await response.Content.ReadAsStringAsync();
                    nieuweKlant.ID = Int32.Parse(output);
                    GetKlanten();
                    OnPropertyChanged("KlantGegevensVisibility");
                }
                else
                {
                    Console.WriteLine("error");
                }
            }
        }


        //kaart opladen
        public ICommand KaartOpladenCommand
        {
            get { return new RelayCommand(KaartOpladen); }
        }


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


        private void KaartOpladen()
        {
            //toon nieuwe pagina
            camDevices = new CameraDevices();

            decodingThread = new Thread(DecodeBarcode);
            decodingThread.Start();
            LoadDevices();
            OnPropertyChanged("btnKaartOpladenIsEnabled");
            OnPropertyChanged("KaartOpladenVisibility");
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
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
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
                Thread.Sleep(3000);
            }
        }


        private string _barcodeFormat;
        public string BarcodeFormat
        {
            get { return _barcodeFormat; }
            set { _barcodeFormat = value; OnPropertyChanged("BarcodeFormat"); }
        }

        private ObservableCollection<string> _money = new ObservableCollection<string>();
        public ObservableCollection<string> Money
        {
            get { return _money; }
            set { _money = value; OnPropertyChanged("Money"); }
        }

        private byte[] _picSource;
        public byte[] PicSource
        {
            get { return _picSource; }
            set { _picSource = value; OnPropertyChanged("PicSource"); }
        }

        private void ShowResult(Result result)
        {
            currentResult = result;
            BarcodeFormat = result.BarcodeFormat.ToString();


            Money.Add(result.Text);
            Balance += Convert.ToDouble(result.Text);
        }

        public bool btnKaartOpladenIsEnabled
        {
            get
            {
                if (camDevices != null)
                {
                    if (camDevices.Current != null) return false;
                    else return true;
                }
                else return true;
            }
        }

        public Visibility KaartOpladenVisibility
        {
            get
            {
                if (btnKaartOpladenIsEnabled) return Visibility.Hidden;
                else return Visibility.Visible;
            }
        }


        //helper methodes 
        //static byte[] GetBytes(string str)
        //{
        //    byte[] array = Encoding.ASCII.GetBytes(str);
        //    return array;
        //}

        //static string GetString(byte[] bytes)
        //{
        //    char[] chars = new char[bytes.Length / sizeof(char)];
        //    System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        //    return new string(chars);
        //}

        public static void CloseCamera()
        {
            if (camDevices != null)
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
}
