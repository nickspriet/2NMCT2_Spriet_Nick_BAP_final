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
    public class ErrorlogDA
    {
        public static List<Errorlog> GetErrorlogs(IEnumerable<Claim> claims)
        {
            List<Errorlog> lijstErrorlogs = new List<Errorlog>();

            string sql = "SELECT k.ID, k.KassaNaam, k.Toestel, e.Timestamp, e.Message, e.Stacktrace FROM Errorlogs e INNER JOIN Kassas k ON e.KassaID = k.ID";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql);

            while (reader.Read())
            {
                lijstErrorlogs.Add(CreateErrorlog(reader));
            }
            reader.Close();

            return lijstErrorlogs;
        }

        private static Errorlog CreateErrorlog(IDataRecord record)
        {
            int id = Int32.Parse(record["ID"].ToString());
            string kassaNaam = record["KassaNaam"].ToString();
            string toestel = record["Toestel"].ToString();

            return new Errorlog()
            {
                Kassa = new Kassa() { ID = id, KassaNaam = kassaNaam, Toestel = toestel },
                Timestamp = ConvertUnixTimeStampToDateTime(Int32.Parse(record["Timestamp"].ToString())),
                Message = record["Message"].ToString(),
                Stacktrace = record["Stacktrace"].ToString()
            };
        }


        public static int InsertErrorlogs(Errorlog e, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Errorlogs (KassaID, Timestamp, Message, Stacktrace) VALUES (@KassaID, @Timestamp, @Message, @Stacktrace)";

            DbParameter parKassaID = Database.AddParameter("AdminDB", "@KassaID", e.Kassa.ID);
            DbParameter parTimestamp = Database.AddParameter("AdminDB", "@Timestamp", DateTimeToUnixTimeStamp(e.Timestamp));
            DbParameter parMessage = Database.AddParameter("AdminDB", "@Message", e.Message);
            DbParameter parStacktrace = Database.AddParameter("AdminDB", "@Stacktrace", e.Stacktrace);

            return Database.InsertData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parKassaID, parTimestamp, parMessage, parStacktrace);
        }



        public static void DeleteErrorlogs(IEnumerable<Claim> claims)
        {
            string sql = "DELETE FROM Errorlogs WHERE 1 = 1";

            Database.ModifyData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql);
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