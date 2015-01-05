using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace nmct.ssa.cashlessproject.webapp.it.DataAccess
{
    public class ErrorlogDA
    {
        private static List<Errorlog> lijstErrorlogs;

        public static List<Errorlog> GetErrorlogs()
        {
            lijstErrorlogs = new List<Errorlog>();

            string sql = "SELECT k.ID, k.KassaNaam, e.Timestamp, e.Message, e.Stacktrace FROM ErrorlogsIT e INNER JOIN KassasIT k ON e.KassaID = k.ID";
            DbDataReader reader = Database.GetData(Database.GetConnection("AdminDB"), sql);

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

            return new Errorlog()
            {
                Kassa = new Kassa() { ID = id, KassaNaam = kassaNaam },
                Timestamp = ConvertUnixTimeStampToDateTime(Int32.Parse(record["Timestamp"].ToString())),
                Message = record["Message"].ToString(),
                Stacktrace = record["Stacktrace"].ToString()
            };
        }


        private static DateTime ConvertUnixTimeStampToDateTime(int unixTimestamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimestamp).ToLocalTime();
            return dtDateTime;
        }
    }
}