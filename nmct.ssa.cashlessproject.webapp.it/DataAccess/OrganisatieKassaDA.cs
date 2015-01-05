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
    public class OrganisatieKassaDA
    {
        private static List<OrganisatieKassa> lijstOrganisatieKassas;

        public static List<OrganisatieKassa> GetOrganisatieKassas()
        {
            lijstOrganisatieKassas = new List<OrganisatieKassa>();

            string sql = "SELECT ok.OrganisatieID, ok.KassaID, k.KassaNaam, k.Toestel, k.AankoopDatum, k.VervalDatum, o.OrganisatieNaam FROM Organisaties_Kassas ok INNER JOIN Organisaties o ON ok.OrganisatieID = o.ID INNER JOIN KassasIT k ON ok.KassaID = k.ID WHERE k.VervalDatum >= " + DateTimeToUnixTimeStamp(DateTime.Now);

            DbDataReader reader = Database.GetData(Database.GetConnection("AdminDB"), sql);

            while (reader.Read())
            {
                lijstOrganisatieKassas.Add(CreateOrganisatieKassa(reader));
            }
            reader.Close();

            return lijstOrganisatieKassas;
        }

        private static OrganisatieKassa CreateOrganisatieKassa(IDataRecord record)
        {
            return new OrganisatieKassa()
            {
                Organisatie = new Organisatie()
                {
                    ID = Int32.Parse(record["OrganisatieID"].ToString()),
                    OrganisatieNaam = record["OrganisatieNaam"].ToString()
                },

                KassaIT = new KassaIT()
                {
                    ID = Int32.Parse(record["KassaID"].ToString()),
                    KassaNaam = record["KassaNaam"].ToString(),
                    Toestel = record["Toestel"].ToString(),
                    AankoopDatum = ConvertUnixTimeStampToDateTime(Int32.Parse(record["AankoopDatum"].ToString())),
                    VervalDatum = ConvertUnixTimeStampToDateTime(Int32.Parse(record["VervalDatum"].ToString()))
                }
            };
        }

        public static void AddOrganisatieKassa(KassaIT kassa)
        {
            string sql = "INSERT INTO Organisaties_Kassas (OrganisatieID, KassaID, VanDatum, TotDatum) VALUES (@OrganisatieID, @KassaID, @VanDatum, @TotDatum)";

            if (kassa.VervalDatum == null) kassa.VervalDatum = kassa.AankoopDatum.AddYears(5);

            DbParameter parOrganisatieID = Database.AddParameter("AdminDB", "@OrganisatieID", kassa.SelectedOrganisatieID);
            DbParameter parKassaID = Database.AddParameter("AdminDB", "@KassaID", kassa.ID);
            DbParameter parVanDatum = Database.AddParameter("AdminDB", "@VanDatum", DateTimeToUnixTimeStamp(kassa.AankoopDatum));
            DbParameter parTotDatum = Database.AddParameter("AdminDB", "@TotDatum", DateTimeToUnixTimeStamp(kassa.VervalDatum));


            Database.InsertData(Database.GetConnection("AdminDB"), sql, parOrganisatieID, parKassaID, parVanDatum, parTotDatum);
        }


        public static void UpdateOrganisatieKassa(KassaIT kassa)
        {
            string sql = "UPDATE Organisaties_Kassas SET OrganisatieID=@OrganisatieID, KassaID=@KassaID, VanDatum=@VanDatum, TotDatum=@TotDatum WHERE KassaID=@KassaID";

            if (kassa.VervalDatum == null) kassa.VervalDatum = kassa.AankoopDatum.AddYears(5);

            DbParameter parOrganisatieID = Database.AddParameter("AdminDB", "@OrganisatieID", kassa.SelectedOrganisatieID);
            DbParameter parKassaID = Database.AddParameter("AdminDB", "@KassaID", kassa.ID);
            DbParameter parVanDatum = Database.AddParameter("AdminDB", "@VanDatum", DateTimeToUnixTimeStamp(kassa.AankoopDatum));
            DbParameter parTotDatum = Database.AddParameter("AdminDB", "@TotDatum", DateTimeToUnixTimeStamp(kassa.VervalDatum));

            Database.ModifyData(Database.GetConnection("AdminDB"), sql, parOrganisatieID, parKassaID, parVanDatum, parTotDatum);
        }


        public static void DeleteOrganisatieKassa(KassaIT kassa)
        {
            string sql = "DELETE FROM Organisaties_Kassas WHERE KassaID=@KassaID";

            DbParameter parKassaID = Database.AddParameter("AdminDB", "@KassaID", kassa.ID);

            Database.ModifyData(Database.GetConnection("AdminDB"), sql, parKassaID);
        }



        private static int DateTimeToUnixTimeStamp(DateTime unixDatum)
        {
            Int32 unixTimestamp = (Int32)(unixDatum.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
            return unixTimestamp;
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