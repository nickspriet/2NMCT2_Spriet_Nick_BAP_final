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
    public class BeheerMedewerkersVM : ObservableObject, IPage
    {
        //naam pagina
        public string Name
        {
            get { return "Beheer medewerkers"; }
        }

        //constructor
        public BeheerMedewerkersVM()
        {
            if (LoginVM.token != null) GetMedewerkers();
        }

        #region "Ophalen Medewerkers"
        private ObservableCollection<Medewerker> _medewerkers;
        public ObservableCollection<Medewerker> Medewerkers
        {
            get { return _medewerkers; }
            set { _medewerkers = value; OnPropertyChanged("Medewerkers"); }
        }

        private Medewerker _selectedMedewerker;
        public Medewerker SelectedMedewerker
        {
            get { return _selectedMedewerker; }
            set { _selectedMedewerker = value; OnPropertyChanged("SelectedMedewerker"); OnPropertyChanged("btnAnnuleerIsEnabled"); }
        }

        private async void GetMedewerkers()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(LoginVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Medewerker");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Medewerkers = JsonConvert.DeserializeObject<ObservableCollection<Medewerker>>(json);

                    SearchList = Medewerkers;
                }
            }
        }
        #endregion



        #region "SearchList Medewerkers"
        private ObservableCollection<Medewerker> _searchList;
        public ObservableCollection<Medewerker> SearchList
        {
            get { return _searchList; }
            set { _searchList = value; OnPropertyChanged("SearchList"); }
        }

        private string _medewerkernaam;
        public string Medewerkernaam
        {
            get { return _medewerkernaam; }
            set
            {
                _medewerkernaam = value;
                OnPropertyChanged("Medewerkernaam");
                SearchMedewerker(_medewerkernaam);
            }
        }

        private void SearchMedewerker(string naam)
        {
            SearchList = new ObservableCollection<Medewerker>();

            foreach (Medewerker medewerker in Medewerkers)
            {
                if (!string.IsNullOrEmpty(naam))
                {
                    string str = medewerker.MedewerkerNaam.ToLower();
                    if (str.Contains(naam.ToLower())) SearchList.Add(medewerker);
                }
            }

            if (naam.Length > 2 && SearchList.Count > 0) SearchList = SearchList;
            else if (naam.Length < 3) SearchList = Medewerkers;
            else SearchList = null;
            OnPropertyChanged("NotFoundVisible");
        }

        public Visibility NotFoundVisible
        {
            get
            {
                if (Medewerkers == null)
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


        #region "Medewerker Toevoegen"
        public ICommand AddMedewerkerCommand
        {
            get { return new RelayCommand(AddMedewerker); }
        }

        private void AddMedewerker()
        {
            Medewerker nieuweMedewerker = new Medewerker();
            Medewerkers.Add(nieuweMedewerker);
            SelectedMedewerker = nieuweMedewerker;
        }



        #endregion"

        #region "Medewerker Opslaan"
        public ICommand SaveMedewerkerCommand
        {
            get { return new RelayCommand(SaveMedewerker); }
        }

        private async void SaveMedewerker()
        {
            string input = JsonConvert.SerializeObject(SelectedMedewerker);

            if (SelectedMedewerker.ID == 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(LoginVM.token.AccessToken);
                    HttpResponseMessage response = await client.PostAsync("http://localhost:50726/api/Medewerker", new StringContent(input, UTF8Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        string output = await response.Content.ReadAsStringAsync();
                        SelectedMedewerker.ID = Int32.Parse(output);
                    }
                    else
                    {
                        Console.WriteLine("error bij post (nieuwe medewerker)");
                    }
                }
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(LoginVM.token.AccessToken);
                    HttpResponseMessage response = await client.PutAsync("http://localhost:50726/api/Medewerker", new StringContent(input, Encoding.UTF8, "application/json"));
                    if (!response.IsSuccessStatusCode) Console.WriteLine("error bij put (update medewerker)");
                }
            }
        }
        #endregion


        #region "Medewerker Verwijderen"
        public ICommand DeleteMedewerkerCommand
        {
            get { return new RelayCommand(DeleteMedewerker); }
        }

        private async void DeleteMedewerker()
        {
            if (SelectedMedewerker.ID != 0)
            {
                MessageBoxResult result = MessageBox.Show(App.Current.MainWindow, "Bent u zeker dat u deze medewerker wilt verwijderen?", "Medeweker verwijderen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    string input = JsonConvert.SerializeObject(SelectedMedewerker);

                    using (HttpClient client = new HttpClient())
                    {
                        client.SetBearerToken(LoginVM.token.AccessToken);
                        HttpResponseMessage response = await client.DeleteAsync("http://localhost:50726/api/Medewerker/" + SelectedMedewerker.ID);
                        if (!response.IsSuccessStatusCode) Console.WriteLine("error bij deleten medewerker");
                    }
                }
            }
        }
        #endregion


        public ICommand AnnuleerMedewerkerCommand
        {
            get { return new RelayCommand(AnnuleerMedewerker); }
        }

        private void AnnuleerMedewerker()
        {
            if (SelectedMedewerker.ID == 0) Medewerkers.Remove(SelectedMedewerker);
            SelectedMedewerker = null;         
            OnPropertyChanged("SelectedMedewerker");
        }


        public Boolean buttonIsEnabled
        {
            get
            {
                if (SelectedMedewerker != null) return true;
                else return false;
            }
        }
    }
}
