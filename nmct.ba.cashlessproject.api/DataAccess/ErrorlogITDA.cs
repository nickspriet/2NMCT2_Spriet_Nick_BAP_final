using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace nmct.ba.cashlessproject.api.DataAccess
{
    public class ErrorlogITDA
    {
        public static int InsertErrorlogs(Errorlog e, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO ErrorlogsIT (KassaID, Timestamp, Message, Stacktrace) VALUES (@KassaID, @Timestamp, @Message, @Stacktrace)";

            DbParameter parKassaID = Database.AddParameter("AdminDB", "@KassaID", e.Kassa.ID);
            DbParameter parTimestamp = Database.AddParameter("AdminDB", "@Timestamp", DateTimeToUnixTimeStamp(e.Timestamp));
            DbParameter parMessage = Database.AddParameter("AdminDB", "@Message", e.Message);
            DbParameter parStacktrace = Database.AddParameter("AdminDB", "@Stacktrace", e.Stacktrace);

            return Database.InsertData(Database.GetConnection("AdminDB"), sql, parKassaID, parTimestamp, parMessage, parStacktrace);
        }

        private static int DateTimeToUnixTimeStamp(DateTime unixDatum)
        {
            Int32 unixTimestamp = (Int32)(unixDatum.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
            return unixTimestamp;
        }
    }
}