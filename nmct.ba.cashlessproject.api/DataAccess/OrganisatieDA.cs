using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.IT;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace nmct.ba.cashlessproject.api.DataAccess
{
    public class OrganisatieDA
    {
        public static Organisatie CheckCredentials(string gebruikersnaam, string wachtwoord)
        {
            string sql = "SELECT ID, GebruikersNaam, Wachtwoord, DbNaam, DbGebruikersNaam, DbWachtwoord, OrganisatieNaam, Adres, Stad, Postcode, Email, Telefoon FROM Organisaties WHERE GebruikersNaam=@GebruikersNaam AND Wachtwoord=@Wachtwoord";
            DbParameter parGebruikersNaam = Database.AddParameter("AdminDB", "@GebruikersNaam", Cryptography.Encrypt(gebruikersnaam));
            DbParameter parWachtwoord = Database.AddParameter("AdminDB", "@Wachtwoord", Cryptography.Encrypt(wachtwoord));
            try
            {
                DbDataReader reader = Database.GetData(Database.GetConnection("AdminDB"), sql, parGebruikersNaam, parWachtwoord);
                reader.Read();
                return new Organisatie()
                {
                    ID = Int32.Parse(reader["ID"].ToString()),
                    GebruikersNaam = reader["GebruikersNaam"].ToString(),
                    Wachtwoord = reader["Wachtwoord"].ToString(),
                    DbNaam = reader["DbNaam"].ToString(),
                    DbGebruikersNaam = reader["DbGebruikersNaam"].ToString(),
                    DbWachtwoord = reader["DbWachtwoord"].ToString(),
                    OrganisatieNaam = reader["OrganisatieNaam"].ToString(),
                    Adres = reader["Adres"].ToString(),
                    Stad = reader["Stad"].ToString(),
                    Postcode = reader["Postcode"].ToString(),
                    Email = reader["Email"].ToString(),
                    Telefoon = reader["Telefoon"].ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static void UpdateOrganisatie(Organisatie o)
        {
            string sql = "UPDATE Organisaties SET Wachtwoord=@Wachtwoord WHERE GebruikersNaam=@GebruikersNaam";

            DbParameter parGebruikersNaam = Database.AddParameter("AdminDB", "@GebruikersNaam", Cryptography.Encrypt(o.GebruikersNaam));
            DbParameter parWachtwoord = Database.AddParameter("AdminDB", "@Wachtwoord", Cryptography.Encrypt(o.Wachtwoord));

            Database.ModifyData(Database.GetConnection("AdminDB"), sql, parGebruikersNaam, parWachtwoord);
            
        }
    }
}