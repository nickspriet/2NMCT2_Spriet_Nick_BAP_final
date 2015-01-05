using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model.Management
{
    public class Verkoop : ObservableObject
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }
        
        private Klant _klant;
        public Klant Klant
        {
            get { return _klant; }
            set { _klant = value; }
        }

        private Kassa _kassa;
        public Kassa Kassa
        {
            get { return _kassa; }
            set { _kassa = value; }
        }

        private Product _product;
        public Product Product
        {
            get { return _product; }
            set { _product = value; }
        }

        private int _aantalProducten = 1;
        public int AantalProducten
        {
            get { return _aantalProducten; }
            set { _aantalProducten = value; OnPropertyChanged("AantalProducten"); }
        }

        private double _totaalPrijs;
        public double TotaalPrijs
        {
            get 
            {
                if (Product != null) _totaalPrijs = AantalProducten * Product.Prijs;
                return _totaalPrijs;
            }
            set { _totaalPrijs = value; OnPropertyChanged("TotaalPrijs"); }
        }
        
        
        
        
    }
}
