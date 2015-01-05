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
    public class KassaMedewerkerDA
    {

        public static List<KassaMedewerker> GetKassaMedewerkers(IEnumerable<Claim> claims)
        {
            List<KassaMedewerker> lijstKassaMedewerkers = new List<KassaMedewerker>();

            string sql = "SELECT km.KassaID, km.MedewerkerID, km.Van, km.Tot, m.MedewerkerNaam FROM Medewerkers m INNER JOIN Kassas_Medewerkers km ON m.ID = km.MedewerkerID INNER JOIN Kassas k ON  km.KassaID = k.ID";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql);

            while (reader.Read())
            {
                lijstKassaMedewerkers.Add(CreateKassaMedewerker(reader));
            }
            reader.Close();

            return lijstKassaMedewerkers;
        }

        private static KassaMedewerker CreateKassaMedewerker(IDataRecord record)
        {
            int kassaID = Int32.Parse(record["KassaID"].ToString());
            int medewerkerID = Int32.Parse(record["MedewerkerID"].ToString());
            string medewerkerNaam = record["MedewerkerNaam"].ToString();

            return new KassaMedewerker()
            {
                Kassa = new Kassa() { ID = kassaID },
                Medewerker = new Medewerker() { ID = medewerkerID, MedewerkerNaam = medewerkerNaam },
                Van = ConvertUnixTimeStampToDateTime(Int32.Parse(record["Van"].ToString())),
                Tot = ConvertUnixTimeStampToDateTime(Int32.Parse(record["Tot"].ToString()))
            };
        }

        public static int InsertKassaMedewerker(KassaMedewerker km, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Kassas_Medewerkers (KassaID, MedewerkerID, Van, Tot) VALUES (@KassaID, @MedewerkerID, @Van, @Tot)";

            DbParameter parKassaID = Database.AddParameter("AdminDB", "@KassaID", km.Kassa.ID);
            DbParameter parMedewerkerID = Database.AddParameter("AdminDB", "@MedewerkerID", km.Medewerker.ID);
            DbParameter parVan = Database.AddParameter("AdminDB", "@Van", DateTimeToUnixTimeStamp(km.Van));
            DbParameter parTot = Database.AddParameter("AdminDB", "@tot", DateTimeToUnixTimeStamp(km.Tot));

            return Database.InsertData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parKassaID, parMedewerkerID, parVan, parTot);
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