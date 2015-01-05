using GalaSoft.MvvmLight.Command;
using nmct.ba.cashlessproject.ui.management.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.ui.management.ViewModel
{
    public class LoginVM: ObservableObject, IPage
    {
        public static TokenResponse token = null;

        //naam pagina
        public string Name
        {
            get { return "Login"; }
        }

        //constructor
        public LoginVM()
        {
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }


        #region "Properties Login"
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged("UserName"); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged("Password"); }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }
        #endregion


        public ICommand LoginCommand
        {
            get { return new RelayCommand<object>(Login); }
        }

        private void Login(object txtWachtwoord)
        {
            PasswordBox passwordBox = txtWachtwoord as PasswordBox;
            Password = passwordBox.Password;

            LoginVM.token = GetToken();

            if (!LoginVM.token.IsError)
            {
                //MainWindow.xaml tonen
                var window = new MainWindow();
                window.Show();
                
                 //huidige window (Login.xaml) verbergen
                App.Current.MainWindow.Hide();
                
                //maak MainWindow het huidige window
                App.Current.MainWindow = window;
            }
            else
            {
                Error = "Gebruikersnaam en/of wachtwoord kloppen niet";
            }
            
        }

        private TokenResponse GetToken()
        {
            OAuth2Client client = new OAuth2Client(new Uri("http://localhost:50726/token"));
            return client.RequestResourceOwnerPasswordAsync(UserName, Password).Result;
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
