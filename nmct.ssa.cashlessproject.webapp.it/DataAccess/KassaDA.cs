using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.IT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace nmct.ssa.cashlessproject.webapp.it.DataAccess
{
    public class KassaDA
    {
        private static List<KassaIT> lijstKassas;

        public static List<KassaIT> GetKassas()
        {
            lijstKassas = new List<KassaIT>();

            string sql = "SELECT ID, KassaNaam, Toestel, AankoopDatum, VervalDatum FROM KassasIT WHERE VervalDatum >= " + DateTimeToUnixTimeStamp(DateTime.Now);
            DbDataReader reader = Database.GetData(Database.GetConnection("AdminDB"), sql);

            while (reader.Read())
            {
                lijstKassas.Add(CreateKassa(reader));
            }
            reader.Close();

            return lijstKassas;
        }

        private static KassaIT CreateKassa(IDataRecord record)
        {
            return new KassaIT()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                KassaNaam = record["KassaNaam"].ToString(),
                Toestel = record["Toestel"].ToString(),
                AankoopDatum = ConvertUnixTimeStampToDateTime(Int32.Parse(record["AankoopDatum"].ToString())),
                VervalDatum = ConvertUnixTimeStampToDateTime(Int32.Parse(record["VervalDatum"].ToString()))
            };
        }


        private static DateTime ConvertUnixTimeStampToDateTime(int unixTimestamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimestamp).ToLocalTime();
            return dtDateTime;
        }


        public static KassaIT GetKassaById(int id)
        {
            return GetKassas().Find(k => k.ID == id);
        }


        public static int AddKassa(KassaIT k)
        {
            string sql = "INSERT INTO KassasIT (KassaNaam, Toestel, AankoopDatum, VervalDatum) VALUES (@KassaNaam, @Toestel, @AankoopDatum, @VervalDatum)";

            if (k.VervalDatum <= new DateTime()) k.VervalDatum = k.AankoopDatum.AddYears(5);

            DbParameter parKassaNaam = Database.AddParameter("AdminDB", "@KassaNaam", k.KassaNaam);
            DbParameter parToestel = Database.AddParameter("AdminDB", "@Toestel", k.Toestel);
            DbParameter parAankoopDatum = Database.AddParameter("AdminDB", "@AankoopDatum", DateTimeToUnixTimeStamp(k.AankoopDatum));
            DbParameter parVervalDatum = Database.AddParameter("AdminDB", "@VervalDatum", DateTimeToUnixTimeStamp(k.VervalDatum));

            return Database.InsertData(Database.GetConnection("AdminDB"), sql, parKassaNaam, parToestel, parAankoopDatum, parVervalDatum);
        
        }


        public static int UpdateKassa(KassaIT k)
        {
            string sql = "UPDATE KassasIT SET KassaNaam=@KassaNaam, Toestel=@Toestel, AankoopDatum=@AankoopDatum, VervalDatum=@VervalDatum WHERE ID=@ID";

            if (k.VervalDatum == new DateTime()) k.VervalDatum = k.AankoopDatum.AddYears(5);

            DbParameter parID = Database.AddParameter("AdminDB", "@ID", k.ID);
            DbParameter parKassaNaam = Database.AddParameter("AdminDB", "@KassaNaam", k.KassaNaam);
            DbParameter parToestel = Database.AddParameter("AdminDB", "@Toestel", k.Toestel);
            DbParameter parAankoopDatum = Database.AddParameter("AdminDB", "@AankoopDatum", DateTimeToUnixTimeStamp(k.AankoopDatum));
            DbParameter parVervalDatum = Database.AddParameter("AdminDB", "@VervalDatum", DateTimeToUnixTimeStamp(k.VervalDatum));

            return Database.ModifyData(Database.GetConnection("AdminDB"), sql, parID, parKassaNaam, parToestel, parAankoopDatum, parVervalDatum);
        }


        private static int DateTimeToUnixTimeStamp(DateTime unixDatum)
        {
            Int32 unixTimestamp = (Int32)(unixDatum.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
            return unixTimestamp;
        }



        public static void DeleteKassa(KassaIT kassa)
        {
            string sql = "DELETE FROM KassasIT WHERE ID=@ID";

            DbParameter parID = Database.AddParameter("AdminDB", "@ID", kassa.ID);

            Database.ModifyData(Database.GetConnection("AdminDB"), sql, parID);
        }
    }
}