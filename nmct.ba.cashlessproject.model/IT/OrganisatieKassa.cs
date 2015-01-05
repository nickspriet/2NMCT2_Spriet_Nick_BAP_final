using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model.IT
{
    public class OrganisatieKassa
    {
        private Organisatie _organisatie;
        private KassaIT _kassaIT;
        private DateTime _vanDatum;
        private DateTime _totDatum;



        public Organisatie Organisatie
        {
            get { return _organisatie; }
            set { _organisatie = value; }
        }


        public KassaIT KassaIT
        {
            get { return _kassaIT; }
            set { _kassaIT = value; }
        }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime VanDatum
        {
            get { return _vanDatum; }
            set { _vanDatum = value; }
        }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TotDatum
        {
            get { return _totDatum; }
            set { _totDatum = value; }
        }
    }
}
