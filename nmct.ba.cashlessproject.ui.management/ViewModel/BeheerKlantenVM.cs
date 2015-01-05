using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
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

namespace nmct.ba.cashlessproject.ui.management.ViewModel
{
    public class BeheerKlantenVM : ObservableObject, IPage
    {
        //naam pagina
        public string Name
        {
            get { return "Beheer klanten"; }
        }

        //constructor
        public BeheerKlantenVM()
        {
            if (LoginVM.token != null) GetKlanten();
        }


        #region "Ophalen Klanten"
        private ObservableCollection<Klant> _klanten;
        public ObservableCollection<Klant> Klanten
        {
            get { return _klanten; }
            set { _klanten = value; OnPropertyChanged("Klanten"); }
        }

        private Klant _selectedKlant;
        public Klant SelectedKlant
        {
            get { return _selectedKlant; }
            set { _selectedKlant = value; OnPropertyChanged("SelectedKlant"); }
        }

        private async void GetKlanten()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(LoginVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Klant");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Klanten = JsonConvert.DeserializeObject<ObservableCollection<Klant>>(json);
                    
                    SearchList = Klanten;
                }
            }
        }
        #endregion 


        #region "Wijzig Klant"
        public ICommand WijzigKlantCommand
        {
            get { return new RelayCommand(WijzigKlant); }
        }

        private async void WijzigKlant()
        {
            string input = JsonConvert.SerializeObject(SelectedKlant);

            if (SelectedKlant.ID != 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(LoginVM.token.AccessToken);
                    HttpResponseMessage response = await client.PutAsync("http://localhost:50726/api/Klant", new StringContent(input, Encoding.UTF8, "application/json"));
                    if (!response.IsSuccessStatusCode) Console.WriteLine("error bij update");
                }
            }
        }
        #endregion


        #region "Searchlist Klanten"
        private ObservableCollection<Klant> _searchList;
        public ObservableCollection<Klant> SearchList
        {
            get { return _searchList; }
            set { _searchList = value; OnPropertyChanged("SearchList"); }
        }

        private string _klantnaam;
        public string Klantnaam
        {
            get { return _klantnaam; }
            set
            {
                _klantnaam = value;
                OnPropertyChanged("Klantnaam");
                SearchKlant(_klantnaam);
            }
        }

        private void SearchKlant(string naam)
        {
            SearchList = new ObservableCollection<Klant>();

            foreach (Klant klant in Klanten)
            {
                if (!string.IsNullOrEmpty(naam))
                {
                    string str = klant.KlantVoornaam.ToLower();
                    if (str.Contains(naam.ToLower())) SearchList.Add(klant);
                }
            }

            if (naam.Length > 2 && SearchList.Count > 0) SearchList = SearchList;
            else if (naam.Length < 3) SearchList = Klanten;
            else SearchList = null;
            OnPropertyChanged("NotFoundVisible");
        }

        public Visibility NotFoundVisible
        {
            get
            {
                if (Klanten == null)
                {
                    return Visibility.Collapsed;
                }
                if (SearchList != null)
                {
                    if (SearchList.Count != 0) return Visibility.Collapsed;
                    else return Visibility.Visible;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        #endregion


        public Boolean buttonIsEnabled
        {
            get
            {
                if (SelectedKlant != null) return true;
                else return false;
            }
        }
    }
}
