using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace nmct.ba.cashlessproject.model.Management
{
    public class Klant
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private long _rijksRegNummer;
        public long RijksRegNummer
        {
            get { return _rijksRegNummer; }
            set { _rijksRegNummer = value; }
        }

        private string _klantNaam;
        public string KlantNaam
        {
            get { return _klantNaam; }
            set { _klantNaam = value; }
        }

        private string _klantVoornaam;
        public string KlantVoornaam
        {
            get { return _klantVoornaam; }
            set { _klantVoornaam = value; }
        }

        private string _kaartNummer;
        public string KaartNummer
        {
            get { return _kaartNummer; }
            set { _kaartNummer = value; }
        }

        private DateTime _geboortedatum;
        public DateTime Geboortedatum
        {
            get { return _geboortedatum; }
            set { _geboortedatum = value; }
        }

        private string _straat;
        public string Straat
        {
            get { return _straat; }
            set { _straat = value; }
        }

        private string _stad;
        public string Stad
        {
            get { return _stad; }
            set { _stad = value; }
        }

        private string _nummer;
        public string Nummer
        {
            get { return _nummer; }
            set { _nummer = value; }
        }

        private string _postcode;
        public string Postcode
        {
            get { return _postcode; }
            set { _postcode = value; }
        }

        private string _nationaliteit;
        public string Nationaliteit
        {
            get { return _nationaliteit; }
            set { _nationaliteit = value; }
        }

        private byte[] _pasFoto;
        public byte[] PasFoto
        {
            get { return _pasFoto; }
            set { _pasFoto = value; }
        }

        private double _saldo;
        public double Saldo
        {
            get { return _saldo; }
            set { _saldo = value; }
        }
    }
}
