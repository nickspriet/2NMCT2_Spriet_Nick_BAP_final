using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model.IT
{
    public class KassaIT
    {
        //private fields
        private int _id;
        private string _kassaNaam;
        private string _toestel;
        private DateTime _aankoopDatum;
        private DateTime _vervalDatum;
        private int _selectedOrganisatieID;


        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required(ErrorMessage="Gelieve een geldige kassanaam in te vullen")]
        [DisplayName("Kassanaam")]
        public string KassaNaam
        {
            get { return _kassaNaam; }
            set { _kassaNaam = value; }
        }

        [Required(ErrorMessage = "Gelieve een geldig toestel in te vullen")]
        public string Toestel
        {
            get { return _toestel; }
            set { _toestel = value; }
        }

        [Required(ErrorMessage = "Gelieve een correcte datum in te vullen")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Aankoopdatum")]
        public DateTime AankoopDatum
        {
            get { return _aankoopDatum; }
            set { _aankoopDatum = value; }
        }


        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Vervaldatum")]
        public DateTime VervalDatum
        {
            get { return _vervalDatum; }
            set { _vervalDatum = value; }
        }


        [DisplayName("Organisatie")]
        public int SelectedOrganisatieID
        {
            get { return _selectedOrganisatieID; }
            set { _selectedOrganisatieID = value; }
        }
    }
}
