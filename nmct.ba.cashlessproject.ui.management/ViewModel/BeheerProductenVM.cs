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
    public class BeheerProductenVM : ObservableObject, IPage
    {
        //naam pagina
        public string Name
        {
            get { return "Beheer producten"; }
        }

        //constructor
        public BeheerProductenVM()
        {
            if (LoginVM.token != null) GetProducten();
        }


        #region "Ophalen Producten"
        private ObservableCollection<Product> _producten;
        public ObservableCollection<Product> Producten
        {
            get { return _producten; }
            set { _producten = value; OnPropertyChanged("Producten"); }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set { _selectedProduct = value; OnPropertyChanged("SelectedProduct"); }
        }

        private async void GetProducten()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(LoginVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Product");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Producten = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);

                    SearchList = Producten;
                }
            }
        }
        #endregion


        #region "SearchList Producten"
        private ObservableCollection<Product> _searchList;
        public ObservableCollection<Product> SearchList
        {
            get { return _searchList; }
            set { _searchList = value; OnPropertyChanged("SearchList"); OnPropertyChanged("Producten"); }
        }

        private string _productnaam;
        public string Productnaam
        {
            get { return _productnaam; }
            set
            {
                _productnaam = value;
                OnPropertyChanged("Productnaam");
                SearchProduct(_productnaam);
            }
        }

        private void SearchProduct(string naam)
        {
            SearchList = new ObservableCollection<Product>();

            foreach (Product product in Producten)
            {
                if (!string.IsNullOrEmpty(naam))
                {
                    string str = product.ProductNaam.ToLower();
                    if (str.Contains(naam.ToLower())) SearchList.Add(product);
                }
            }

            if (naam.Length > 2 && SearchList.Count > 0) SearchList = SearchList;
            else if (naam.Length < 3) SearchList = Producten;
            else SearchList = null;
            OnPropertyChanged("NotFoundVisible");
        }

        public Visibility NotFoundVisible
        {
            get
            {
                if (Producten == null)
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

        #region "Product Toevoegen"
        public ICommand AddProductCommand
        {
            get { return new RelayCommand(AddProduct); }
        }

        private void AddProduct()
        {
            Product nieuwProduct = new Product();
            Producten.Add(nieuwProduct);
            SelectedProduct = nieuwProduct;
        }
        #endregion"

        #region "Product Opslaan"
        public ICommand SaveProductCommand
        {
            get { return new RelayCommand(SaveProduct); }
        }

        private async void SaveProduct()
        {
            string input = JsonConvert.SerializeObject(SelectedProduct);

            if (SelectedProduct.ID == 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(LoginVM.token.AccessToken);
                    HttpResponseMessage response = await client.PostAsync("http://localhost:50726/api/Product", new StringContent(input, UTF8Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        string output = await response.Content.ReadAsStringAsync();
                        SelectedProduct.ID = Int32.Parse(output);
                    }
                    else
                    {
                        Console.WriteLine("error bij post (nieuw product)");
                    }
                }
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(LoginVM.token.AccessToken);
                    HttpResponseMessage response = await client.PutAsync("http://localhost:50726/api/Product", new StringContent(input, Encoding.UTF8, "application/json"));
                    if (!response.IsSuccessStatusCode) Console.WriteLine("error bij put (update)");
                }
            }
        }
        #endregion


        #region "Product Verwijderen"
        public ICommand DeleteProductCommand
        {
            get { return new RelayCommand(DeleteProduct); }
        }

        private async void DeleteProduct()
        {
            if (SelectedProduct.ID != 0)
            {
                MessageBoxResult result = MessageBox.Show(App.Current.MainWindow, "Bent u zeker dat u dit product op inactief wilt plaatsen?", "Inactief product", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    //product staat al op inactief
                    if (!SelectedProduct.IsActief) MessageBox.Show(App.Current.MainWindow, "Het product staat al op inactief.", "Product al inactief", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        SelectedProduct.IsActief = false;
                        string input = JsonConvert.SerializeObject(SelectedProduct);

                        //het product staat op actief
                        using (HttpClient client = new HttpClient())
                        {
                            client.SetBearerToken(LoginVM.token.AccessToken);
                            HttpResponseMessage response = await client.PutAsync("http://localhost:50726/api/Product/", new StringContent(input, Encoding.UTF8, "application/json"));
                            if (!response.IsSuccessStatusCode) Console.WriteLine("error bij put (update van IsActief");
                            else OnPropertyChanged("SelectedProduct");
                        }
                    }
                }
            }
        }
        #endregion


        public Boolean buttonIsEnabled
        {
            get
            {
                if (SelectedProduct != null) return true;
                else return false;
            }
        }
    }
}
