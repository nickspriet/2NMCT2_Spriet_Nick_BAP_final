using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.ui.medewerker.ViewModel
{
    public class ApplicationVM : ObservableObject
    {
        //constructor
        public ApplicationVM()
        {
            Pages.Add(new ScanBadgeVM());
            Pages.Add(new BestellingVM());
            CurrentPage = Pages[0];

            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }


        //de huidige usercontrol
        private IPage _currentPage;
        public IPage CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; OnPropertyChanged("CurrentPage"); }
        }


        //lijst van alle usercontrols van het project
        private List<IPage> _pages;
        public List<IPage> Pages
        {
            get
            {
                if (_pages == null) _pages = new List<IPage>();
                return _pages;
            }
        }


        //wissel van pagina
        public ICommand ChangePageCommand
        {
            get { return new RelayCommand<IPage>(ChangePage); }
        }

        public void ChangePage(IPage page)
        {
            CurrentPage = page;
        }


        //uitloggen
        public ICommand LogoutCommand
        {
            get { return new RelayCommand(Logout); }
        }

        public void Logout()
        {
            HuidigeKassaMedewerker.Tot = DateTime.Now;
            if (ScanBadgeVM.IngelogdeMedewerker != null) HuidigeKassaMedewerker.Medewerker = ScanBadgeVM.IngelogdeMedewerker;
            if (IngelogdeKassa != null) HuidigeKassaMedewerker.Kassa = IngelogdeKassa;
            InsertMedewerker();

            IngelogdeMedewerker = null;

            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
            ScanBadgeVM.token = null;

            appvm.ChangePage(new ScanBadgeVM());
        }


        //medewerker/kassa inserten in tabel 'KassaMedewerker'
        private async void InsertMedewerker()
        {
            string input = JsonConvert.SerializeObject(HuidigeKassaMedewerker);

            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ScanBadgeVM.token.AccessToken);
                HttpResponseMessage response = await client.PostAsync("http://localhost:50726/api/KassaMedewerker", new StringContent(input, Encoding.UTF8, "application/json"));
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

        private static KassaMedewerker _huidigeKassaMedewerker = new KassaMedewerker();
        public static KassaMedewerker HuidigeKassaMedewerker
        {
            get { return _huidigeKassaMedewerker; }
            set { _huidigeKassaMedewerker = value; }
        }


        //ingelogde medewerker (header)
        private string _ingelogdeMedewerker;
        public string IngelogdeMedewerker
        {
            get { return _ingelogdeMedewerker; }
            set { _ingelogdeMedewerker = value; OnPropertyChanged("IngelogdeMedewerker"); }
        }

        //ingelogde kassa (header)
        private Kassa _ingelogdeKassa;
        public Kassa IngelogdeKassa
        {
            get { return _ingelogdeKassa; }
            set { _ingelogdeKassa = value; OnPropertyChanged("IngelogdeKassa"); }
        }


        //Datum en tijd (footer)
        private static Timer _timer = new Timer(1000);

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DatumTijd = DateTime.Now.ToString();
        }

        private string _datumTijd;
        public string DatumTijd
        {
            get { return _datumTijd; }
            set { _datumTijd = value; OnPropertyChanged("DatumTijd"); }
        }
    }
}
