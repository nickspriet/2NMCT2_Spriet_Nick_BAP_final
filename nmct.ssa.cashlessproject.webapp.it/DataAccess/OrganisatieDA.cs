using nmct.ba.cashlessproject.helper;
using nmct.ba.cashlessproject.model.IT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace nmct.ssa.cashlessproject.webapp.it.DataAccess
{
    public class OrganisatieDA
    {
        private static List<Organisatie> lijstOrganisaties;

        public static List<Organisatie> GetOrganisaties()
        {
            lijstOrganisaties = new List<Organisatie>();

            string sql = "SELECT ID, GebruikersNaam, Wachtwoord, DbNaam, DbGebruikersNaam, DbWachtwoord, OrganisatieNaam, Adres, Stad, Postcode, Email, Telefoon FROM Organisaties";

            try
            {
                DbDataReader reader = Database.GetData(Database.GetConnection("AdminDB"), sql);

                while (reader.Read())
                {
                    lijstOrganisaties.Add(CreateOrganisatie(reader));
                }
                reader.Close();

                return lijstOrganisaties;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static Organisatie CreateOrganisatie(IDataRecord record)
        {
            return new Organisatie()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                GebruikersNaam = Cryptography.Decrypt(record["GebruikersNaam"].ToString()),
                Wachtwoord = Cryptography.Decrypt(record["Wachtwoord"].ToString()),
                DbNaam = Cryptography.Decrypt(record["DbNaam"].ToString()),
                DbGebruikersNaam = Cryptography.Decrypt(record["DbGebruikersNaam"].ToString()),
                DbWachtwoord = Cryptography.Decrypt(record["DbWachtwoord"].ToString()),
                OrganisatieNaam = record["OrganisatieNaam"].ToString(),
                Adres = record["Adres"].ToString(),
                Stad = record["Stad"].ToString(),
                Postcode = record["Postcode"].ToString(),
                Email = record["Email"].ToString(),
                Telefoon = record["Telefoon"].ToString()
            };
        }



        public static Organisatie GetOrganisatieById(int id)
        {
            return lijstOrganisaties.Find(o => o.ID == id);
        }



        public static void AddOrganisatie(Organisatie o)
        {
            string sql = "INSERT INTO Organisaties (GebruikersNaam, Wachtwoord, DbNaam, DbGebruikersNaam, DbWachtwoord, OrganisatieNaam, Adres, Stad, Postcode, Email, Telefoon) VALUES (@GebruikersNaam, @Wachtwoord, @DbNaam, @DbGebruikersNaam, @DbWachtwoord, @OrganisatieNaam, @Adres, @Stad, @Postcode, @Email, @Telefoon)";

            DbParameter parGebruikersNaam = Database.AddParameter("AdminDB", "@GebruikersNaam", Cryptography.Encrypt(o.GebruikersNaam));
            DbParameter parWachtwoord = Database.AddParameter("AdminDB", "@Wachtwoord", Cryptography.Encrypt(o.Wachtwoord));
            DbParameter parDbNaam = Database.AddParameter("AdminDB", "@DbNaam", Cryptography.Encrypt(o.DbNaam));
            DbParameter parDbGebruikersNaam = Database.AddParameter("AdminDB", "@DbGebruikersNaam", Cryptography.Encrypt(o.DbGebruikersNaam));
            DbParameter parDbWachtwoord = Database.AddParameter("AdminDB", "@DbWachtwoord", Cryptography.Encrypt(o.DbWachtwoord));
            DbParameter parOrganisatieNaam = Database.AddParameter("AdminDB", "@OrganisatieNaam", o.OrganisatieNaam);
            DbParameter parAdres = Database.AddParameter("AdminDB", "@Adres", o.Adres);
            DbParameter parStad = Database.AddParameter("AdminDB", "@Stad", o.Stad);
            DbParameter parPostcode = Database.AddParameter("AdminDB", "@Postcode", o.Postcode);
            DbParameter parEmail = Database.AddParameter("AdminDB", "@Email", o.Email);
            DbParameter parTelefoon = Database.AddParameter("AdminDB", "@Telefoon", o.Telefoon);

            Database.InsertData(Database.GetConnection("AdminDB"), sql, parGebruikersNaam, parWachtwoord, parDbNaam, parDbGebruikersNaam, parDbWachtwoord, parOrganisatieNaam, parAdres, parStad, parPostcode, parEmail, parTelefoon);

            CreateDatabase(o);
        }



        #region "Database voor de organisatie aanmaken"
        private static void CreateDatabase(Organisatie o)
        {
            // create the actual database
            string create = File.ReadAllText(HostingEnvironment.MapPath(@"~/App_Data/create.txt")); //only for the web
            //string create = File.ReadAllText(@"..\..\Data\create.txt"); // only for desktop
            string sql = create.Replace("@@DbNaam", o.DbNaam).Replace("@@DbGebruikersNaam", o.DbGebruikersNaam).Replace("@@DbWachtwoord", o.DbWachtwoord);
            foreach (string commandText in RemoveGo(sql))
            {
                Database.ModifyData(Database.GetConnection("AdminDB"), commandText);
            }

            // create login, user and tables
            DbTransaction trans = null;
            try
            {
                trans = Database.BeginTransaction("AdminDB");

                string fill = File.ReadAllText(HostingEnvironment.MapPath(@"~/App_Data/fill.txt")); // only for the web
                //string fill = File.ReadAllText(@"..\..\Data\fill.txt"); // only for desktop
                string sql2 = fill.Replace("@@DbNaam", o.DbNaam).Replace("@@DbGebruikersNaam", o.DbGebruikersNaam).Replace("@@DbWachtwoord", o.DbWachtwoord);

                foreach (string commandText in RemoveGo(sql2))
                {
                    Database.ModifyData(trans, commandText);
                }

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                Console.WriteLine(ex.Message);
            }
        }

        private static string[] RemoveGo(string input)
        {
            //split the script on "GO" commands
            string[] splitter = new string[] { "\r\nGO\r\n" };
            string[] commandTexts = input.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            return commandTexts;
        }
        #endregion





        public static void UpdateOrganisatie(Organisatie o)
        {
            string sql = "UPDATE Organisaties SET GebruikersNaam=@GebruikersNaam , Wachtwoord=@Wachtwoord , DbNaam=@DbNaam , DbGebruikersNaam=@DbGebruikersNaam , DbWachtwoord=@DbWachtwoord , OrganisatieNaam=@OrganisatieNaam , Adres=@Adres , Stad=@Stad , Postcode=@Postcode , Email=@Email , Telefoon=@Telefoon WHERE ID=@ID ";

            DbParameter parID  = Database.AddParameter("AdminDB", "@ID ", o.ID);
            DbParameter parGebruikersNaam  = Database.AddParameter("AdminDB", "@GebruikersNaam ", Cryptography.Encrypt(o.GebruikersNaam));
            DbParameter parWachtwoord  = Database.AddParameter("AdminDB", "@Wachtwoord ", Cryptography.Encrypt(o.Wachtwoord));
            DbParameter parDbNaam  = Database.AddParameter("AdminDB", "@DbNaam ", Cryptography.Encrypt(o.DbNaam));
            DbParameter parDbGebruikersNaam  = Database.AddParameter("AdminDB", "@DbGebruikersNaam ", Cryptography.Encrypt(o.DbGebruikersNaam));
            DbParameter parDbWachtwoord  = Database.AddParameter("AdminDB", "@DbWachtwoord ", Cryptography.Encrypt(o.DbWachtwoord));
            DbParameter parOrganisatieNaam  = Database.AddParameter("AdminDB", "@OrganisatieNaam ", o.OrganisatieNaam);
            DbParameter parAdres  = Database.AddParameter("AdminDB", "@Adres", o.Adres);
            DbParameter parStad  = Database.AddParameter("AdminDB", "@Stad ", o.Stad);
            DbParameter parPostcode  = Database.AddParameter("AdminDB", "@Postcode ", o.Postcode);
            DbParameter parEmail = Database.AddParameter("AdminDB", "@Email ", o.Email);
            DbParameter parTelefoon  = Database.AddParameter("AdminDB", "@Telefoon ", o.Telefoon);

            Database.ModifyData(Database.GetConnection("AdminDB"), sql, parID, parGebruikersNaam , parWachtwoord , parDbNaam , parDbGebruikersNaam , parDbWachtwoord , parOrganisatieNaam , parAdres , parStad , parPostcode , parEmail , parTelefoon);
            
        }
    }
}