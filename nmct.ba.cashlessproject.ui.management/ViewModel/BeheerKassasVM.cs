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

namespace nmct.ba.cashlessproject.ui.management.ViewModel
{
    public class BeheerKassasVM : ObservableObject, IPage
    {
        //naam pagina
        public string Name
        {
            get { return "Beheer kassa's"; }
        }

        //constructor
        public BeheerKassasVM()
        {
            if (LoginVM.token != null)
            {
                GetKassas();
                GetKassaMedewerkers();
            }
        }

        #region "Ophalen Kassa's"
        private ObservableCollection<Kassa> _kassas;
        private ObservableCollection<Kassa> Kassas
        {
            get { return _kassas; }
            set { _kassas = value; OnPropertyChanged("Kassas"); }
        }

        private Kassa _selectedKassa;
        public Kassa SelectedKassa
        {
            get { return _selectedKassa; }
            set { _selectedKassa = value; OnPropertyChanged("SelectedKassa"); OnPropertyChanged("MedewerkersPerKassa"); }
        }

        private async void GetKassas()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(LoginVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Kassa");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Kassas = JsonConvert.DeserializeObject<ObservableCollection<Kassa>>(json);

                    SearchList = Kassas;
                }
            }
        }
        #endregion


        #region "SearchList Kassa's"
        private ObservableCollection<Kassa> _searchList;
        public ObservableCollection<Kassa> SearchList
        {
            get { return _searchList; }
            set { _searchList = value; OnPropertyChanged("SearchList"); }
        }

        private string _kassanaam;
        public string Kassanaam
        {
            get { return _kassanaam; }
            set
            {
                _kassanaam = value;
                OnPropertyChanged("Kassanaam");
                SearchKlant(_kassanaam);
            }
        }

        private void SearchKlant(string naam)
        {
            SearchList = new ObservableCollection<Kassa>();

            foreach (Kassa kassa in Kassas)
            {
                if (!string.IsNullOrEmpty(naam))
                {
                    string str = kassa.KassaNaam.ToLower();
                    if (str.Contains(naam.ToLower())) SearchList.Add(kassa);
                }
            }

            if (naam.Length > 2 && SearchList.Count > 0) SearchList = SearchList;
            else if (naam.Length < 3) SearchList = Kassas;
            else SearchList = null;
            OnPropertyChanged("NotFoundVisible");
        }

        public Visibility NotFoundVisible
        {
            get
            {
                if (Kassas == null)
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



        #region "Ophalen Bemande Kassa's"
        private ObservableCollection<KassaMedewerker> _kassaMedewerkers;
        public ObservableCollection<KassaMedewerker> KassaMedewerkers
        {
            get { return _kassaMedewerkers; }
            set { _kassaMedewerkers = value; OnPropertyChanged("KassaMedewerkers"); }
        }

        private ObservableCollection<KassaMedewerker> _medewerkersPerKassa;
        public ObservableCollection<KassaMedewerker> MedewerkersPerKassa
        {
            get
            {
                if (SelectedKassa != null)
                {
                    _medewerkersPerKassa = new ObservableCollection<KassaMedewerker>(KassaMedewerkers.Where(k => k.Kassa.ID == SelectedKassa.ID).ToList());
                    return _medewerkersPerKassa;
                }
                return null;
            }
            set { _medewerkersPerKassa = value; OnPropertyChanged("MedewerkersPerKassa"); }
        }

        private async void GetKassaMedewerkers()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(LoginVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/KassaMedewerker");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    KassaMedewerkers = JsonConvert.DeserializeObject<ObservableCollection<KassaMedewerker>>(json);
                }
            }
        }
        #endregion
    }
}
