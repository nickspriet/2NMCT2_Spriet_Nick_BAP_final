using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.ui.klant.ViewModel
{
    public class StandbyVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "Standby"; }
        }

        private static Klant _huidigeKlant;
        public static Klant HuidigeKlant
        {
            get { return _huidigeKlant; }
            set { _huidigeKlant = value; }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }

        public StandbyVM()
        {
            OnPropertyChanged("btnLogoutIsEnabled");
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

                //rijksregisternummer
                rd = new ReadData("beidpkcs11.dll");
                long regNummer = Convert.ToInt64(rd.GetData("national_number"));


                ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
                ApplicationVM.token = GetToken();

                //haal de klanten op
                if (!ApplicationVM.token.IsError) GetKlanten(regNummer, appvm);
            }
            else
            {
                Error = "Geen identiteitskaart";
            }
        }


        public static async void GetKlanten(long regNummer, ApplicationVM appvm)
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/klant");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Klanten = JsonConvert.DeserializeObject<ObservableCollection<Klant>>(json);

                    //zit klant al in database ?
                    //  - JA: stop de klant uit de database in Huidigeklant
                    //  - NEE: Lees de rest van identiteitskaart ook in
                    if (Klanten != null) HuidigeKlant = Klanten.FirstOrDefault(k => k.RijksRegNummer == regNummer);
                    if (HuidigeKlant == null) HuidigeKlant = ReadKlantID();

                    //stuur door naar volgende pagina
                    appvm.ChangePage(new KlantGegevensVM());
                }
            }
        }

        private static ObservableCollection<Klant> _klanten;
        public static ObservableCollection<Klant> Klanten
        {
            get { return _klanten; }
            set { _klanten = value; }
        }



        //token ophalen
        private TokenResponse GetToken()
        {
            OAuth2Client client = new OAuth2Client(new Uri("http://localhost:50726/token"));
            string login = ConfigurationManager.AppSettings["GebruikersNaam"];
            string wachtwoord = ConfigurationManager.AppSettings["Wachtwoord"];
            return client.RequestResourceOwnerPasswordAsync(login, wachtwoord).Result;
        }


        public static Klant ReadKlantID()
        {
            Klant k = new Klant();

            //rijksregisternummer
            ReadData rd = new ReadData("beidpkcs11.dll");
            k.RijksRegNummer = Convert.ToInt64(rd.GetData("national_number"));

            //familienaam
            rd = new ReadData("beidpkcs11.dll");
            k.KlantNaam = rd.GetData("surname");

            //voornaam
            rd = new ReadData("beidpkcs11.dll");
            string voornamen = rd.GetData("firstnames");
            k.KlantVoornaam = GetFirstName(voornamen);

            //kaartnummer
            rd = new ReadData("beidpkcs11.dll");
            k.KaartNummer = rd.GetData("card_number");

            //geboortedatum
            rd = new ReadData("beidpkcs11.dll");
            string datumString = rd.GetData("date_of_birth");
            k.Geboortedatum = ConvertStringToDateTime(datumString);

            //straat en nummer
            rd = new ReadData("beidpkcs11.dll");
            string adres = rd.GetData("address_street_and_number");
            k.Straat = GetStreet(adres);
            k.Nummer = GetNumber(adres);

            //stad
            rd = new ReadData("beidpkcs11.dll");
            k.Stad = rd.GetData("address_municipality");

            //postcode
            rd = new ReadData("beidpkcs11.dll");
            k.Postcode = rd.GetData("address_zip");

            //nationaliteit
            rd = new ReadData("beidpkcs11.dll");
            k.Nationaliteit = rd.GetData("nationality");

            //foto
            rd = new ReadData("beidpkcs11.dll");
            k.PasFoto = rd.GetPhotoFile();
            k.Saldo = 0;

            return k;
        }


        public static string GetStreet(string adres)
        {
            String[] stukken = adres.Split(new Char[] { ' ' });
            return stukken[0];
        }

        public static string GetNumber(string adres)
        {
            String[] stukken = adres.Split(new Char[] { ' ' });
            return stukken[1];
        }

        public static string GetFirstName(string voornamen)
        {
            int posSpatie = voornamen.IndexOf(" ");
            if (posSpatie == -1) return voornamen;
            else return voornamen.Substring(0, voornamen.Length - posSpatie - 1);
        }



        public static DateTime ConvertStringToDateTime(string datumString)
        {
            String[] arrDatum = datumString.Split(new Char[] { ' ' });

            //converteer de maand naar het juiste maandnummer
            arrDatum[1] = ConvertMonth(arrDatum[1]);

            //converteer naar integers
            int iDag, iMaand, iJaar;
            Int32.TryParse(arrDatum[0], out iDag);
            Int32.TryParse(arrDatum[1], out iMaand);
            if (arrDatum[2] == "") Int32.TryParse(arrDatum[3], out iJaar);
            else Int32.TryParse(arrDatum[2], out iJaar);


            return new DateTime(iJaar, iMaand, iDag);
        }


        public static string ConvertMonth(string maand)
        {
            switch (maand)
            {
                case "JAN": return "01";
                case "FEB": return "02";
                case "MAAR": return "03";
                case "APR": return "04";
                case "MEI": return "05";
                case "JUN": return "06";
                case "JUL": return "07";
                case "AUG": return "08";
                case "SEP": return "09";
                case "OKT": return "10";
                case "NOV": return "11";
                default: return "12";
            }
        }
    }
}
