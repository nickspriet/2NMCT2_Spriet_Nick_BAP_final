namespace nmct.ssa.cashlessproject.webapp.it.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using nmct.ssa.cashlessproject.webapp.it.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<nmct.ssa.cashlessproject.webapp.it.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        //De 'Seed' methode wordt aangeroepen als we een migratie uitvoeren
        protected override void Seed(nmct.ssa.cashlessproject.webapp.it.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //


            string roleAdmin = "Administrator";
            IdentityResult roleResult;

            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!RoleManager.RoleExists(roleAdmin)) roleResult = RoleManager.Create(new IdentityRole(roleAdmin));
        
            if (!context.Users.Any(u => u.Email.Equals("nick.spriet@student.howest.be")))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                var user = new ApplicationUser()
                {
                    Naam = "Spriet",
                    Voornaam = "Nick",
                    Email = "nick.spriet@student.howest.be",
                    UserName = "nick.spriet@student.howest.be",
                    Adres = "Dunantlaan 35",
                    Stad = "Ieper",
                    Postcode = "8900"
                };

                manager.Create(user, "-W8w00rd123");
                manager.AddToRole(user.Id, roleAdmin);
            }
        }
    }
}
