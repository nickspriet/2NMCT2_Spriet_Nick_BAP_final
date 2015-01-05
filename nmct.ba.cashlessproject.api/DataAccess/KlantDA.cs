using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace nmct.ba.cashlessproject.api.DataAccess
{
    public class KlantDA
    {
        public static List<Klant> GetKlanten(IEnumerable<Claim> claims)
        {
            List<Klant> lijstKlanten = new List<Klant>();

            string sql = "SELECT ID, RijksRegNummer, KlantNaam, KlantVoornaam, KaartNummer, Geboortedatum, Straat, Nummer, Stad, Postcode, Nationaliteit, PasFoto, Saldo FROM Klanten ORDER BY KlantVoornaam";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql);

            while (reader.Read())
            {
                lijstKlanten.Add(CreateKlant(reader));
            }
            reader.Close();

            return lijstKlanten;
        }

        //bij IDataRecord wordt elk record uit de reader 1x overlopen
        private static Klant CreateKlant(IDataRecord record)
        {
            byte[] pasfoto = null;
            if (!DBNull.Value.Equals(record["PasFoto"])) pasfoto = (byte[])record["PasFoto"];
            else pasfoto = new byte[0];

            return new Klant()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                RijksRegNummer = Int64.Parse(record["RijksRegNummer"].ToString()),
                KlantNaam = record["KlantNaam"].ToString(),
                KlantVoornaam = record["KlantVoornaam"].ToString(),
                KaartNummer = record["KaartNummer"].ToString(),
                Geboortedatum = ConvertUnixTimeStampToDateTime(Int32.Parse(record["Geboortedatum"].ToString())),
                Straat = record["Straat"].ToString(),
                Nummer = record["Nummer"].ToString(),
                Stad = record["Stad"].ToString(),
                Postcode = record["Postcode"].ToString(),
                Nationaliteit = record["Nationaliteit"].ToString(),
                PasFoto = pasfoto,
                Saldo = double.Parse(record["Saldo"].ToString())
            };
        }

        public static int InsertKlant(Klant k, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Klanten (RijksRegNummer, KlantNaam, KlantVoornaam, KaartNummer, Geboortedatum, Straat, Nummer, Stad, Postcode, Nationaliteit, PasFoto, Saldo) VALUES (@RijksRegNummer, @KlantNaam, @KlantVoornaam, @KaartNummer, @Geboortedatum, @Straat, @Nummer, @Stad, @Postcode, @Nationaliteit, @PasFoto, @Saldo)";

            DbParameter parRijksRegNummer = Database.AddParameter("AdminDB", "@RijksRegNummer", k.RijksRegNummer);
            DbParameter parKlantNaam = Database.AddParameter("AdminDB", "@KlantNaam", k.KlantNaam);
            DbParameter parKlantVoornaam = Database.AddParameter("AdminDB", "@KlantVoornaam", k.KlantVoornaam);
            DbParameter parKaartNummer = Database.AddParameter("AdminDB", "@KaartNummer", k.KaartNummer);
            DbParameter parGeboortedatum = Database.AddParameter("AdminDB", "@Geboortedatum", DateTimeToUnixTimeStamp(k.Geboortedatum));
            DbParameter parStraat = Database.AddParameter("AdminDB", "@Straat", k.Straat);
            DbParameter parNummer = Database.AddParameter("AdminDB", "@Nummer", k.Nummer);
            DbParameter parStad = Database.AddParameter("AdminDB", "@Stad", k.Stad);
            DbParameter parPostcode = Database.AddParameter("AdminDB", "@Postcode", k.Postcode);
            DbParameter parNationaliteit = Database.AddParameter("AdminDB", "@Nationaliteit", k.Nationaliteit);
            DbParameter parPasFoto = Database.AddParameter("AdminDB", "@PasFoto", k.PasFoto);
            DbParameter parSaldo = Database.AddParameter("AdminDB", "@Saldo", k.Saldo);

            if (k.PasFoto == null) parPasFoto.Value = new Byte[0];

            return Database.InsertData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parRijksRegNummer, parKlantNaam, parKlantVoornaam, parKaartNummer, parGeboortedatum, parStraat, parNummer, parStad, parPostcode, parNationaliteit, parPasFoto, parSaldo);
        }


        public static void UpdateKlant(Klant k, IEnumerable<Claim> claims)
        {
            //Rijksregisternummer kan niet gewijzigd worden
            string sql = "UPDATE Klanten SET KlantNaam=@KlantNaam, KlantVoornaam=@KlantVoornaam, KaartNummer=@KaartNummer, Geboortedatum=@Geboortedatum, Straat=@Straat, Nummer=@Nummer, Stad=@Stad, Postcode=@Postcode, Nationaliteit=@Nationaliteit, Saldo=@Saldo WHERE ID=@ID";

            DbParameter parID = Database.AddParameter("AdminDB", "@ID", k.ID);
            DbParameter parKlantNaam = Database.AddParameter("AdminDB", "@KlantNaam", k.KlantNaam);
            DbParameter parKlantVoornaam = Database.AddParameter("AdminDB", "@KlantVoornaam", k.KlantVoornaam);
            DbParameter parKaartNummer = Database.AddParameter("AdminDB", "@KaartNummer", k.KaartNummer);
            DbParameter parGeboortedatum = Database.AddParameter("AdminDB", "@Geboortedatum", DateTimeToUnixTimeStamp(k.Geboortedatum));
            DbParameter parStraat = Database.AddParameter("AdminDB", "@Straat", k.Straat);
            DbParameter parNummer = Database.AddParameter("AdminDB", "@Nummer", k.Nummer);
            DbParameter parStad = Database.AddParameter("AdminDB", "@Stad", k.Stad);
            DbParameter parPostcode = Database.AddParameter("AdminDB", "@Postcode", k.Postcode);
            DbParameter parNationaliteit = Database.AddParameter("AdminDB", "@Nationaliteit", k.Nationaliteit);
            DbParameter parPasFoto = Database.AddParameter("AdminDB", "@PasFoto", k.PasFoto);
            DbParameter parSaldo = Database.AddParameter("AdminDB", "@Saldo", k.Saldo);

            if (k.PasFoto == null) parPasFoto.Value = new Byte[0];

            Database.ModifyData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parID, parKlantNaam, parKlantVoornaam, parKaartNummer, parGeboortedatum, parStraat, parNummer, parStad, parPostcode, parNationaliteit, parPasFoto, parSaldo);
        }


        private static DateTime ConvertUnixTimeStampToDateTime(int unixTimestamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimestamp).ToLocalTime();
            return dtDateTime;
        }

        private static int DateTimeToUnixTimeStamp(DateTime unixDatum)
        {
            Int32 unixTimestamp = (Int32)(unixDatum.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
            return unixTimestamp;
        }
    }
}