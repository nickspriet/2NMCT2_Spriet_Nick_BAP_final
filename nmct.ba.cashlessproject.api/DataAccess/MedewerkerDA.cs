using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace nmct.ba.cashlessproject.api.DataAccess
{
    public class MedewerkerDA
    {
        public static List<Medewerker> GetMedewerkers(IEnumerable<Claim> claims)
        {
            List<Medewerker> lijstMedewerkers = new List<Medewerker>();

            string sql = "SELECT ID, MedewerkerNaam, MedewerkerVoornaam, Straat, Nummer, Stad, Postcode, Email, Telefoon FROM Medewerkers ORDER BY MedewerkerNaam";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql);

            while (reader.Read())
            {
                lijstMedewerkers.Add(CreateMedewerker(reader));
            }
            reader.Close();

            return lijstMedewerkers;
        }

        private static Medewerker CreateMedewerker(IDataRecord record)
        {
            return new Medewerker()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                MedewerkerNaam = record["MedewerkerNaam"].ToString(),
                MedewerkerVoornaam = record["MedewerkerVoornaam"].ToString(),
                Straat = record["Straat"].ToString(),
                Nummer = record["Nummer"].ToString(),
                Stad = record["Stad"].ToString(),
                Postcode = record["Postcode"].ToString(),
                Email = record["Email"].ToString(),
                Telefoon = record["Telefoon"].ToString()
            };
        }


        public static int InsertMedewerker(Medewerker m, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Medewerkers (MedewerkerNaam, MedewerkerVoonaam, Straat, Nummer, Stad, Postcode, Email, Telefoon) VALUES (@MedewerkerNaam, @MedewerkerVoonaam, @Straat, @Nummer, @Stad, @Postcode, @Email, @Telefoon)";

            DbParameter parMedewerkerNaam = Database.AddParameter("AdminDB", "@MedewerkerNaam", m.MedewerkerNaam);
            DbParameter parMedewerkerVoonaam = Database.AddParameter("AdminDB", "@MedewerkerVoonaam", m.MedewerkerVoornaam);
            DbParameter parStraat = Database.AddParameter("AdminDB", "@Straat", m.Straat);
            DbParameter parNummer = Database.AddParameter("AdminDB", "@Nummer", m.Nummer);
            DbParameter parStad = Database.AddParameter("AdminDB", "@Stad", m.Stad);
            DbParameter parPostcode = Database.AddParameter("AdminDB", "@Postcode", m.Postcode);
            DbParameter parEmail = Database.AddParameter("AdminDB", "@Email", m.Email);
            DbParameter parTelefoon = Database.AddParameter("AdminDB", "@Telefoon", m.Telefoon);

            return Database.InsertData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parMedewerkerNaam, parMedewerkerVoonaam, parStraat, parNummer, parStad, parPostcode, parEmail, parTelefoon);
        }


        public static void UpdateMedewerker(Medewerker m, IEnumerable<Claim> claims)
        {
            string sql = "UPDATE Medewerkers SET MedewerkerNaam=@MedewerkerNaam, MedewerkerVoornaam=@MedewerkerVoonaam, Straat=@Straat, Nummer=@Nummer, Stad=@Stad, Postcode=@Postcode, Email=@Email, Telefoon=@Telefoon WHERE ID=@ID";

            DbParameter parMedewerkerNaam = Database.AddParameter("AdminDB", "@MedewerkerNaam", m.MedewerkerNaam);
            DbParameter parMedewerkerVoonaam = Database.AddParameter("AdminDB", "@MedewerkerVoonaam", m.MedewerkerVoornaam);
            DbParameter parStraat = Database.AddParameter("AdminDB", "@Straat", m.Straat);
            DbParameter parNummer = Database.AddParameter("AdminDB", "@Nummer", m.Nummer);
            DbParameter parStad = Database.AddParameter("AdminDB", "@Stad", m.Stad);
            DbParameter parPostcode = Database.AddParameter("AdminDB", "@Postcode", m.Postcode);
            DbParameter parEmail = Database.AddParameter("AdminDB", "@Email", m.Email);
            DbParameter parTelefoon = Database.AddParameter("AdminDB", "@Telefoon", m.Telefoon);

            Database.ModifyData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parMedewerkerNaam, parMedewerkerVoonaam, parStraat, parNummer, parStad, parPostcode, parEmail, parTelefoon);

        }


        public static void DeleteMedewerker(int id, IEnumerable<Claim> claims)
        {
            string sql = "DELETE FROM Medewerkers WHERE ID=@ID";

            DbParameter parID = Database.AddParameter("AdminDB", "@ID", id);

            Database.ModifyData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parID);
        }

       
    }
}