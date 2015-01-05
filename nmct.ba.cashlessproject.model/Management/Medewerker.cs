using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model.Management
{
    public class Medewerker
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _medewerkerNaam;
        public string MedewerkerNaam
        {
            get { return _medewerkerNaam; }
            set { _medewerkerNaam = value; }
        }

        private string _medewerkerVoornaam;
        public string MedewerkerVoornaam
        {
            get { return _medewerkerVoornaam; }
            set { _medewerkerVoornaam = value; }
        }

        private string _straat;
        public string Straat
        {
            get { return _straat; }
            set { _straat = value; }
        }

        private string _nummer;
        public string Nummer
        {
            get { return _nummer; }
            set { _nummer = value; }
        }

        private string _stad;
        public string Stad
        {
            get { return _stad; }
            set { _stad = value; }
        }

        private string _postcode;
        public string Postcode
        {
            get { return _postcode; }
            set { _postcode = value; }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private string _telefoon;
        public string Telefoon
        {
            get { return _telefoon; }
            set { _telefoon = value; }
        }
    }
}
