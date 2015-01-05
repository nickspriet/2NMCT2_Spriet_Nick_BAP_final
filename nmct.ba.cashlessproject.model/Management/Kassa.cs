using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model.Management
{
    public class Kassa
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _kassaNaam;
        public string KassaNaam
        {
            get { return _kassaNaam; }
            set { _kassaNaam = value; }
        }

        private string _toestel;
        public string Toestel
        {
            get { return _toestel; }
            set { _toestel = value; }
        }
    }
}
