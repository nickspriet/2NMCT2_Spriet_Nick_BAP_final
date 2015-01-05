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
    public class KassaDA
    {

        public static List<Kassa> GetKassas(IEnumerable<Claim> claims)
        {
            List<Kassa> lijstKassas = new List<Kassa>();

            string sql = "SELECT ID, KassaNaam, Toestel FROM Kassas ORDER BY KassaNaam";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql);

            while (reader.Read())
            {
                lijstKassas.Add(CreateKassa(reader));
            }
            reader.Close();

            return lijstKassas;
        }

        private static Kassa CreateKassa(IDataRecord record)
        {
            return new Kassa()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                KassaNaam = record["KassaNaam"].ToString(),
                Toestel = record["Toestel"].ToString()
            };
        }
    }
}