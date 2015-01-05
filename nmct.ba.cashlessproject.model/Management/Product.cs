using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model.Management
{
    public class Product
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _productNaam;
        public string ProductNaam
        {
            get { return _productNaam; }
            set { _productNaam = value; }
        }

        private double _prijs;
        public double Prijs
        {
            get { return _prijs; }
            set { _prijs = value; }
        }

        private bool _isActief;
        public bool IsActief
        {
            get { return _isActief; }
            set { _isActief = value; }
        }
    }
}
