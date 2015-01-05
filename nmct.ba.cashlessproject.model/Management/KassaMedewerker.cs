using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model.Management
{
    public class KassaMedewerker
    {
        private Kassa _kassa;
        public Kassa Kassa
        {
            get { return _kassa; }
            set { _kassa = value; }
        }

        private Medewerker _medewerker;
        public Medewerker Medewerker
        {
            get { return _medewerker; }
            set { _medewerker = value; }
        }

        private DateTime _van;
        public DateTime Van
        {
            get { return _van; }
            set { _van = value; }
        }

        private DateTime _tot;
        public DateTime Tot
        {
            get { return _tot; }
            set { _tot = value; }
        }
    }
}
