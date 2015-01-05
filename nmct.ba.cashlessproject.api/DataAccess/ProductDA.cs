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
    public class ProductDA
    {
        public static List<Product> GetProducten(IEnumerable<Claim> claims)
        {
            List<Product> lijstProducten = new List<Product>();
 
            string sql = "SELECT ID, ProductNaam, Prijs, IsActief FROM Producten ORDER BY ProductNaam";
            DbDataReader reader = Database.GetData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql);

            while (reader.Read())
            {
                lijstProducten.Add(CreateProduct(reader));
            }
            reader.Close();
            
            return lijstProducten;
        }

        private static Product CreateProduct(IDataRecord record)
        {
            return new Product()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                ProductNaam = record["ProductNaam"].ToString(),
                Prijs = double.Parse(record["Prijs"].ToString()),
                IsActief = bool.Parse(record["IsActief"].ToString())
            };
        }


        public static int InsertProduct(Product p, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Producten (ProductNaam, Prijs, IsActief) VALUES (@ProductNaam, @Prijs, @IsActief)";

            DbParameter parProductNaam = Database.AddParameter("AdminDB", "@ProductNaam", p.ProductNaam);
            DbParameter parPrijs = Database.AddParameter("AdminDB", "@Prijs", p.Prijs);
            DbParameter parIsActief = Database.AddParameter("AdminDB", "@IsActief", p.IsActief);

            return Database.InsertData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parProductNaam, parPrijs, parIsActief);
        }


        public static void UpdateProduct(Product p, IEnumerable<Claim> claims)
        {
            string sql = "UPDATE Producten SET ProductNaam=@ProductNaam, Prijs=@Prijs, IsActief=@IsActief WHERE ID=@ID";

            DbParameter parID = Database.AddParameter("AdminDB", "@ID", p.ID);
            DbParameter parProductNaam = Database.AddParameter("AdminDB", "@ProductNaam", p.ProductNaam);
            DbParameter parPrijs = Database.AddParameter("AdminDB", "@Prijs", p.Prijs);
            DbParameter parIsActief = Database.AddParameter("AdminDB", "@IsActief", p.IsActief);

            Database.ModifyData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parID, parProductNaam, parPrijs, parIsActief);
        }


        //public static void DeleteProduct(int id, IEnumerable<Claim> claims)
        //{
        //    string sql = "DELETE FROM Producten WHERE ID=@ID";

        //    DbParameter parID = Database.AddParameter("AdminDB", "@ID", id);

        //    Database.ModifyData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parID);
        //}


        public static void UpdateActiefToInactief(int id, IEnumerable<Claim> claims)
        {
            string sql = "UPDATE Producten SET IsActief=@IsActief WHERE ID=@ID";

            DbParameter parID = Database.AddParameter("AdminDB", "@ID", id);
            DbParameter parIsActief = Database.AddParameter("AdminDB", "@IsActief", 0);

            Database.ModifyData(Database.GetConnection(Database.CreateConnectionStringFromClaims(claims)), sql, parID, parIsActief);
        }
    }
}