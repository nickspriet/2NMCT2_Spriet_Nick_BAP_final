using GalaSoft.MvvmLight.CommandWpf;
using nmct.ba.cashlessproject.ui.management.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace nmct.ba.cashlessproject.ui.management.ViewModel
{
    public class ApplicationVM : ObservableObject
    {
        //constructor
        public ApplicationVM()
        {
            Pages.Add(new BeheerProductenVM());
            Pages.Add(new BeheerMedewerkersVM());
            Pages.Add(new BeheerKassasVM());
            Pages.Add(new BeheerKlantenVM());
            Pages.Add(new StatistiekenVM());
            Pages.Add(new WijzigWachtwoordVM());
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

        private void Logout()
        {
            //token wissen
            LoginVM.token = null;

            //Login.xaml tonen
            Login login = new Login();
            login.Show();

            //MainWindow.xaml verbergen
            App.Current.MainWindow.Hide();

            //maak Login.xaml het huidige window
            App.Current.MainWindow = login;
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
