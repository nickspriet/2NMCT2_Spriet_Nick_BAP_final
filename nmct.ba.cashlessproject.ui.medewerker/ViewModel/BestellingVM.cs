using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using System.Configuration;
using System.Globalization;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.ui.medewerker.ViewModel
{
    public class BestellingVM : ObservableObject, IPage
    {
        //naam pagina
        public string Name
        {
            get { return "Bestelling"; }
        }

        //constructor
        public BestellingVM()
        {
            if (ScanBadgeVM.token != null)
            {
                GetProducten();
                GetKlanten();
                GetKassas();
            }

            //begintijd opslaan
            if (ApplicationVM.HuidigeKassaMedewerker != null) ApplicationVM.HuidigeKassaMedewerker.Van = DateTime.Now;
        }


        //identiteitskaart scannen
        public ICommand ScanIdentityCardCommand
        {
            get { return new RelayCommand(ScanIdentityCard); }
        }

        private void ScanIdentityCard()
        {
            ReadData rd = new ReadData("beidpkcs11.dll");
            if (rd.GetSlotDescription() != "")
            {
                Error = "Bezig met scannen, even geduld...";

                //klantID inlezen
                rd = new ReadData("beidpkcs11.dll");
                RijksRegNummer = Convert.ToInt64(rd.GetData("national_number"));

                //update klant
                OnPropertyChanged("HuidigeKlant");
                OnPropertyChanged("HuidigeKlantNaam");
                OnPropertyChanged("IsKlantScannedEnabled");
                OnPropertyChanged("NieuwSaldoKlant");

                Error = "";
            }
            else
            {
                Error = "Geen identiteitskaart (scannen mislukt)";
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
                    ApplicationVM.HuidigeKassaMedewerker.Kassa = Kassas.First(k => k.ID.ToString() == ConfigurationManager.AppSettings["KassaID"]);
                }
            }
        }
        #endregion



        #region "Ophalen Klanten"
        private ObservableCollection<Klant> _klanten;
        public ObservableCollection<Klant> Klanten
        {
            get { return _klanten; }
            set { _klanten = value; OnPropertyChanged("Klanten"); }
        }

        private Klant _huidigeKlant = new Klant();
        public Klant HuidigeKlant
        {
            get
            {
                if (Klanten != null)
                {
                    _huidigeKlant = Klanten.FirstOrDefault(k => k.RijksRegNummer == RijksRegNummer);
                    if (_huidigeKlant == null) MessageBox.Show(App.Current.MainWindow, "Klant is nog niet geregistreerd.", "Registratie klant", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return _huidigeKlant;
            }
            set { _huidigeKlant = value; OnPropertyChanged("HuidigeKlant"); }
        }

        private long _rijksRegNummer;
        public long RijksRegNummer
        {
            get { return _rijksRegNummer; }
            set { _rijksRegNummer = value; OnPropertyChanged("RijksRegNummer"); }
        }

        private string _huidigeKlantNaam;
        public string HuidigeKlantNaam
        {
            get
            {
                if (HuidigeKlant.ID != 0) _huidigeKlantNaam = HuidigeKlant.KlantVoornaam + " " + HuidigeKlant.KlantNaam;
                return _huidigeKlantNaam;
            }
            set { _huidigeKlantNaam = value; OnPropertyChanged("HuidigeKlantNaam"); }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }

        private async void GetKlanten()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ScanBadgeVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Klant");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Klanten = JsonConvert.DeserializeObject<ObservableCollection<Klant>>(json);
                }
            }
        }
        #endregion



        #region "Ophalen Producten"
        private ObservableCollection<Product> _producten;
        public ObservableCollection<Product> Producten
        {
            get { return _producten; }
            set { _producten = value; OnPropertyChanged("Producten"); }
        }

        private async void GetProducten()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ScanBadgeVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Product");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Producten = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);

                    //lijst van achter naar voor overlopen !
                    for (int i = Producten.Count() - 1; i >= 0; i--)
                    {
                        if (Producten[i].IsActief == false) Producten.Remove(Producten[i]);
                    }
                }
            }
        }
        #endregion


        #region "Bestelling Opmaken"
        private ObservableCollection<Verkoop> _bestelling = new ObservableCollection<Verkoop>();
        public ObservableCollection<Verkoop> Bestelling
        {
            get { return _bestelling; }
            set { _bestelling = value; OnPropertyChanged("Bestelling"); }
        }

        public ICommand AddProductToOrderCommand
        {
            get { return new RelayCommand<Product>(AddProductToOrder); }
        }

        private void AddProductToOrder(Product clickedProduct)
        {
            this.ClickedProduct = clickedProduct;
            //is het product al aanwezig in de bestelling ?
            //  - NEE: voeg nieuw product toe aan de bestelling
            //  - JA: verhoog het aantal met 1
            Verkoop prod = Bestelling.FirstOrDefault(b => b.Product.ID == clickedProduct.ID);
            if (prod == null)
            {
                Verkoop v = new Verkoop() { Product = clickedProduct };
                Bestelling.Add(v);
            }
            else
            {
                Bestelling.First(b => b.Product.ID == clickedProduct.ID).AantalProducten += 1;
                Bestelling.First(b => b.Product.ID == clickedProduct.ID).TotaalPrijs = Bestelling.First(b => b.Product.ID == clickedProduct.ID).AantalProducten * clickedProduct.Prijs;
            }
            OnPropertyChanged("Totaal");
            OnPropertyChanged("NieuwSaldoKlant");
        }

        public ICommand PlusProductCommand
        {
            get { return new RelayCommand<Product>(PlusProduct); }
        }

        private void PlusProduct(Product clickedProduct)
        {
            Bestelling.First(b => b.Product.ID == clickedProduct.ID).AantalProducten += 1;
            Bestelling.First(b => b.Product.ID == clickedProduct.ID).TotaalPrijs = Bestelling.First(b => b.Product.ID == clickedProduct.ID).AantalProducten * clickedProduct.Prijs;
            OnPropertyChanged("Totaal");
            OnPropertyChanged("NieuwSaldoKlant");
        }

        public ICommand MinProductCommand
        {
            get { return new RelayCommand<Product>(MinProduct); }
        }

        private void MinProduct(Product clickedProduct)
        {
            if (Bestelling.First(b => b.Product.ID == clickedProduct.ID).AantalProducten - 1 == 0) Bestelling.Remove(Bestelling.First(b => b.Product.ID == clickedProduct.ID));
            else
            {
                Bestelling.First(b => b.Product.ID == clickedProduct.ID).AantalProducten -= 1;
                Bestelling.First(b => b.Product.ID == clickedProduct.ID).TotaalPrijs = Bestelling.First(b => b.Product.ID == clickedProduct.ID).AantalProducten * clickedProduct.Prijs;
            }
            OnPropertyChanged("Totaal");
            OnPropertyChanged("NieuwSaldoKlant");
        }

        #endregion



        #region "Properties Bestelling"
        //private int _id;
        //public int ID
        //{
        //    get { return _id; }
        //    set { _id = value; OnPropertyChanged("ID"); }
        //}

        private Product _clickedProduct;
        public Product ClickedProduct
        {
            get { return _clickedProduct; }
            set { _clickedProduct = value; OnPropertyChanged("ClickedProduct"); }
        }

        private double _totaal;
        public double Totaal
        {
            get
            {
                if (Bestelling != null)
                {
                    _totaal = 0;
                    foreach (Verkoop v in Bestelling)
                    {
                        _totaal += v.TotaalPrijs;
                    }
                }
                return _totaal;
            }
            set { _totaal = value; OnPropertyChanged("Totaal"); }
        }

        private double _nieuwSaldoKlant;
        public double NieuwSaldoKlant
        {
            get
            {
                if (HuidigeKlant.ID != 0) _nieuwSaldoKlant = HuidigeKlant.Saldo - Totaal;
                return _nieuwSaldoKlant;
            }
            set { _nieuwSaldoKlant = value; OnPropertyChanged("NieuwSaldoKlant"); }
        }


        public Boolean IsKlantScannedEnabled
        {
            get
            {
                if (HuidigeKlant.ID != 0) return true;
                else return false;
            }
        }

        //private double _totaalPrijsProduct;
        //public double TotaalPrijsProduct
        //{
        //    get { return _totaalPrijsProduct; }
        //    set { _totaalPrijsProduct = value; OnPropertyChanged("TotaalPrijsProduct"); }
        //}
        #endregion



        #region "Bestelling Afrekenen"
        public ICommand AfrekenenCommand
        {
            get { return new RelayCommand(Afrekenen); }
        }

        private void Afrekenen()
        {
            if (NieuwSaldoKlant > 0)
            {
                if (HuidigeKlant != null && ApplicationVM.HuidigeKassaMedewerker != null && Bestelling != null)
                {
                    //1: saldo updaten
                    //2: verkoop inserten in tabel 'Verkoop'
                    UpdateKlantSaldo(HuidigeKlant);
                    foreach (Verkoop v in Bestelling) AddVerkoop(v);
                }
            }
            else
            {
                MessageBox.Show(App.Current.MainWindow, "Saldo is ontoereikend.", "Saldo is ontoereikend", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //1: saldo updaten
        private async void UpdateKlantSaldo(Klant HuidigeKlant)
        {
            HuidigeKlant.Saldo = NieuwSaldoKlant;

            string input = JsonConvert.SerializeObject(HuidigeKlant);

            if (HuidigeKlant.ID != 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(ScanBadgeVM.token.AccessToken);
                    HttpResponseMessage response = await client.PutAsync("http://localhost:50726/api/Klant", new StringContent(input, Encoding.UTF8, "application/json"));

                    if (!response.IsSuccessStatusCode) Console.WriteLine("error bij put (update)");
                }
            }
        }

        //2: verkoop inserten in tabel 'Verkoop'
        private async void AddVerkoop(Verkoop verkoop)
        {
            verkoop.Timestamp = DateTime.Now;

            verkoop.Kassa = ApplicationVM.HuidigeKassaMedewerker.Kassa;
            verkoop.Klant = Klanten.First(k => k.ID == HuidigeKlant.ID);

            string input = JsonConvert.SerializeObject(verkoop);

            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ScanBadgeVM.token.AccessToken);
                HttpResponseMessage response = await client.PostAsync("http://localhost:50726/api/Verkoop", new StringContent(input, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    string output = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("error bij post");
                }
            }
        }
        #endregion
    }
}
