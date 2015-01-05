using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model.IT
{
    public class Organisatie
    {
        //private fields
        private int _id;
        private string _gebruikersNaam;
        private string _wachtwoord;
        private string _dbNaam;
        private string _dbGebruikersNaam;
        private string _dbWachtwoord;
        private string _organisatieNaam;
        private string _adres;
        private string _stad;
        private string _postcode;
        private string _email;
        private string _telefoon;


        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required(ErrorMessage = "Gelieve een geldige gebruikersnaam in te vullen")]
        public string GebruikersNaam
        {
            get { return _gebruikersNaam; }
            set { _gebruikersNaam = value; }
        }

        [Required(ErrorMessage = "Gelieve een geldig wachtwoord in te vullen")]
        public string Wachtwoord
        {
            get { return _wachtwoord; }
            set { _wachtwoord = value; }
        }

        [Required(ErrorMessage = "Gelieve een geldige databasenaam in te vullen")]
        [DisplayName("Databasenaam")]
        public string DbNaam
        {
            get { return _dbNaam; }
            set { _dbNaam = value; }
        }

        [Required(ErrorMessage = "Gelieve een geldige database gebruikersnaam in te vullen")]
        [DisplayName("Database gebruikersnaam")]
        public string DbGebruikersNaam
        {
            get { return _dbGebruikersNaam; }
            set { _dbGebruikersNaam = value; }
        }


        [Required(ErrorMessage = "Gelieve een geldig database wachtwoord in te vullen")]
        [DisplayName("Database wachtwoord")]
        public string DbWachtwoord
        {
            get { return _dbWachtwoord; }
            set { _dbWachtwoord = value; }
        }

        [Required(ErrorMessage = "Gelieve een geldige organisatienaam in te vullen")]
        [DisplayName("Naam organisatie")]
        public string OrganisatieNaam
        {
            get { return _organisatieNaam; }
            set { _organisatieNaam = value; }
        }

        [Required(ErrorMessage = "Gelieve een geldig adres in te vullen")]
        public string Adres
        {
            get { return _adres; }
            set { _adres = value; }
        }

        [Required(ErrorMessage = "Gelieve een geldige stad in te vullen")]
        public string Stad
        {
            get { return _stad; }
            set { _stad = value; }
        }

        [Required(ErrorMessage = "Gelieve een geldige postcode in te vullen")]
        public string Postcode
        {
            get { return _postcode; }
            set { _postcode = value; }
        }

        [Required(ErrorMessage = "Gelieve een geldig e-mailadres in te vullen")]
        [DisplayName("E-mailadres")]
        [DataType(DataType.EmailAddress)]
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        [Required]
        [DisplayName("Telefoonnummer")]
        [DataType(DataType.PhoneNumber)]
        public string Telefoon
        {
            get { return _telefoon; }
            set { _telefoon = value; }
        }
    }
}
