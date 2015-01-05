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
    public class VerkoopDA
    {
        public static List<Verkoop> GetVerkoop(IEnumerable<Claim> claims)
        {
            List<Verkoop> lijstVerkoop = new List<Verkoop>();

            string sql = "SELECT * FROM Verkoop v INNER JOIN Klanten kl ON v.KlantID = kl.ID INNER JOIN Producten p ON v.ProductID = p.ID INNER JOIN Kassas k ON v.KassaID = k.ID ORDER BY v.Timestamp";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql);

            while (reader.Read())
            {
                lijstVerkoop.Add(CreateVerkoop(reader));
            }
            reader.Close();

            return lijstVerkoop;
        }

        private static Verkoop CreateVerkoop(IDataRecord record)
        {
            int klantID = Int32.Parse(record["KlantID"].ToString());
            int kassaID = Int32.Parse(record["KassaID"].ToString());
            string kassaNaam = record["KassaNaam"].ToString();
            int productID = Int32.Parse(record["ProductID"].ToString());
            string productNaam = record["ProductNaam"].ToString();
            double prijs = double.Parse(record["Prijs"].ToString());


            return new Verkoop()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                Timestamp = ConvertUnixTimeStampToDateTime(Int32.Parse(record["Timestamp"].ToString())),
                Klant = new Klant() { ID = klantID },
                Kassa = new Kassa() { ID = kassaID, KassaNaam = kassaNaam },
                Product = new Product() { ID = productID, ProductNaam = productNaam, Prijs = prijs },
                AantalProducten = Int32.Parse(record["AantalProducten"].ToString()),
                TotaalPrijs = double.Parse(record["TotaalPrijs"].ToString())
            };
        }


        public static int InsertVerkoop(Verkoop v, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Verkoop (Timestamp, KlantID, KassaID, ProductID, AantalProducten, TotaalPrijs) VALUES (@Timestamp, @KlantID, @KassaID, @ProductID, @AantalProducten, @TotaalPrijs)";

            DbParameter parTimestamp = Database.AddParameter("AdminDB", "@Timestamp", DateTimeToUnixTimeStamp(v.Timestamp));
            DbParameter parKlantID = Database.AddParameter("AdminDB", "@KlantID", v.Klant.ID);
            DbParameter parKassaID = Database.AddParameter("AdminDB", "@KassaID", v.Kassa.ID);
            DbParameter parProductID = Database.AddParameter("AdminDB", "@ProductID", v.Product.ID);
            DbParameter parAantalProducten = Database.AddParameter("AdminDB", "@AantalProducten", v.AantalProducten);
            DbParameter parTotaalPrijs = Database.AddParameter("AdminDB", "@TotaalPrijs", v.TotaalPrijs);


            return Database.InsertData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parTimestamp, parKlantID, parKassaID, parProductID, parAantalProducten, parTotaalPrijs);
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