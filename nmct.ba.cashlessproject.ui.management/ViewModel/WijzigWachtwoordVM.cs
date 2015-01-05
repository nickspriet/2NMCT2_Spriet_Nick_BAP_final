using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.model.IT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.ui.management.ViewModel
{
    public class WijzigWachtwoordVM : ObservableObject, IPage
    {
        public static TokenResponse token = null;

        //naam pagina
        public string Name
        {
            get { return "Wijzig wachtwoord"; }
        }

        //login manager (header)
        private string _loginManager;
        public string LoginManager
        {
            get
            {
                LoginVM loginvm = Application.Current.Windows[0].DataContext as LoginVM;
                _loginManager = loginvm.UserName;
                return _loginManager;
            }
            set { _loginManager = value; OnPropertyChanged("LoginManager"); }
        }


        #region "Properties Wijzig Wachtwoord"
        private string _oudWachtwoord;
        public string OudWachtwoord
        {
            get { return _oudWachtwoord; }
            set { _oudWachtwoord = value; OnPropertyChanged("OudWachtwoord"); }
        }

        private string _nieuwWachtwoord;
        public string NieuwWachtwoord
        {
            get { return _nieuwWachtwoord; }
            set { _nieuwWachtwoord = value; OnPropertyChanged("NieuwWachtwoord"); }
        }

        private string _nieuwWachtwoordBevestigen;
        public string NieuwWachtwoordBevestigen
        {
            get { return _nieuwWachtwoordBevestigen; }
            set { _nieuwWachtwoordBevestigen = value; OnPropertyChanged("NieuwWachtwoordBevestigen"); }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }
        #endregion


        #region "Wijzig Wachtwoord"
        public ICommand WijzigWachtwoordCommand
        {
            get { return new RelayCommand<object>(WijzigWachtwoord); }
        }

        private void WijzigWachtwoord(object arrParameter)
        {
            object[] arrWachtwoorden = (object[])arrParameter;

            PasswordBox oudPasswordBox = arrWachtwoorden[0] as PasswordBox;
            OudWachtwoord = oudPasswordBox.Password;

            PasswordBox nieuwPasswordBox = arrWachtwoorden[1] as PasswordBox;
            NieuwWachtwoord = nieuwPasswordBox.Password;

            PasswordBox nieuwBevestigenPasswordBox = arrWachtwoorden[2] as PasswordBox;
            NieuwWachtwoordBevestigen = nieuwBevestigenPasswordBox.Password;

            //nieuwe token aanmaken
            token = GetToken();

            if (!token.IsError) //OFWEL if (token == LoginVM.token)
            {
                if (NieuwWachtwoord == NieuwWachtwoordBevestigen)
                {
                    //token leegmaken en opvullen met nieuwe token
                    LoginVM.token = null;
                    LoginVM.token = token;
                    VeranderWachtwoord();

                }
                else
                {
                    Error = "Wachtwoorden komen niet overeen";
                }
            }
            else
            {
                Error = "Oud wachtwoord is niet correct";
            }
        }

        private async void VeranderWachtwoord()
        {
            Organisatie oNieuw = new Organisatie();
            oNieuw.GebruikersNaam = LoginManager;
            oNieuw.Wachtwoord = NieuwWachtwoord;

            string input = JsonConvert.SerializeObject(oNieuw);

            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(LoginVM.token.AccessToken);
                HttpResponseMessage response = await client.PutAsync("http://localhost:50726/api/Organisatie", new StringContent(input, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode) Console.WriteLine("error bij update");
            }
        }

        private TokenResponse GetToken()
        {
            OAuth2Client client = new OAuth2Client(new Uri("http://localhost:50726/token"));
            return client.RequestResourceOwnerPasswordAsync(LoginManager, OudWachtwoord).Result;
        }
        #endregion
    }
}
