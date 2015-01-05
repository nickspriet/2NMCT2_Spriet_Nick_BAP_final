using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.ui.klant.ViewModel
{
    public class ApplicationVM : ObservableObject
    {
        public static TokenResponse token = null;

        //constructor
        public ApplicationVM()
        {
            Pages.Add(new StandbyVM());
            Pages.Add(new KlantGegevensVM());
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
            KlantGegevensVM.CloseCamera();
            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
            ApplicationVM.token = null;

            appvm.ChangePage(new StandbyVM());
        }


        //toon huidige tijd onderaanlinks
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
