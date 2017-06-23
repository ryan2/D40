namespace D40.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<D40.Models.D40DBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "D40.Models.D40DBContext";
        }

        protected override void Seed(D40.Models.D40DBContext context)
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
            var names = new List<D40.Models.Name>
            {
                new D40.Models.Name {Last_Name = "Travolta",First_Name = "John", Active = true, Office = "HP" },
                new D40.Models.Name {Last_Name = "Hague",First_Name = "Mike", Active = true, Office = "OPPS" },
                new D40.Models.Name {Last_Name = "Fox",First_Name = "Tom", Active = true, Office = "HSP" },
                new D40.Models.Name {Last_Name = "Ball",First_Name = "Joe", Active = false, Office = "DALTCP" },
            };
            names.ForEach(s => context.Names.AddOrUpdate(p => p.Last_Name,s));
            context.SaveChanges();
            var Assets = new List<D40.Models.D40>
            {
                new D40.Models.D40 {Category = "Computer", Asset_Tag = "1", Model = "E3000", Refresh_Date = DateTime.Parse("2017-02-01"), NameID = names.Single(i=>i.Last_Name =="Travolta").ID},
                new D40.Models.D40 {Category = "Computer", Asset_Tag = "2", Model = "E3000", Refresh_Date = DateTime.Parse("2017-07-01"), NameID = names.Single(i=>i.Last_Name =="Travolta").ID},
                new D40.Models.D40 {Category = "Computer", Asset_Tag = "3", Model = "E3000", Refresh_Date = DateTime.Parse("2017-04-01"), NameID = names.Single(i=>i.Last_Name =="Hague").ID},
                new D40.Models.D40 {Category = "Computer", Asset_Tag = "4", Model = "E3000", Refresh_Date = DateTime.Parse("2017-05-03"), NameID = names.Single(i=>i.Last_Name =="Fox").ID},
                new D40.Models.D40 {Category = "Computer", Asset_Tag = "5", Model = "E3000", Refresh_Date = DateTime.Parse("2017-05-02"), NameID = names.Single(i=>i.Last_Name =="Fox").ID}
            };
            Assets.ForEach(s => context.D40.AddOrUpdate(p => p.Asset_Tag));
            context.SaveChanges();
            var Tickets = new List<D40.Models.Ticket>
            {
                new D40.Models.Ticket {Ticket_Num = "1", Description="new computer",Open_Date = DateTime.Parse("2012-5-5"),Closed_Date = DateTime.Parse("2012-5-5"), NameID=names.Single(i=>i.Last_Name=="Travolta").ID},
                new D40.Models.Ticket {Ticket_Num = "2", Description="Refresh",Open_Date = DateTime.Parse("2012-5-5"),Closed_Date = DateTime.Parse("2012-5-5"), NameID=names.Single(i=>i.Last_Name=="Hague").ID},
                new D40.Models.Ticket {Ticket_Num = "3", Description="Crashed",Open_Date = DateTime.Parse("2012-5-5"),Closed_Date = DateTime.Parse("2012-5-5"), NameID=names.Single(i=>i.Last_Name=="Travolta").ID},
                new D40.Models.Ticket {Ticket_Num = "4", Description="Return",Open_Date = DateTime.Parse("2012-5-5"),Closed_Date = DateTime.Parse("2012-5-5"), NameID=names.Single(i=>i.Last_Name=="Fox").ID},
            };
            Tickets.ForEach(s => context.Tickets.AddOrUpdate(p => p.Ticket_Num,s));
            context.SaveChanges();

            var Softwares = new List<D40.Models.Software>
            {
                new D40.Models.Software {title = "Adobe", license="1", num = 5, Users = new List<D40.Models.SoftwareName>()},
                new D40.Models.Software {title = "Microsoft", license="2", num = 5, Users = new List<D40.Models.SoftwareName>()},
                new D40.Models.Software {title = "Word", license="3", num = 5, Users = new List<D40.Models.SoftwareName>()},
                new D40.Models.Software {title = "Excel", license="4", num = 5, Users = new List<D40.Models.SoftwareName>()},
            };
            var SoftwareNames = new List<D40.Models.SoftwareName>
            {
                new Models.SoftwareName {NameID = names.Single(s=>s.Last_Name=="Hague").ID, SoftwareID=Softwares.Single(s=>s.title=="Adobe").ID },
                new Models.SoftwareName {NameID = names.Single(s=>s.Last_Name=="Hague").ID, SoftwareID=Softwares.Single(s=>s.title=="Word").ID },
                new Models.SoftwareName {NameID = names.Single(s=>s.Last_Name=="Hague").ID, SoftwareID=Softwares.Single(s=>s.title=="Excel").ID },
                new Models.SoftwareName {NameID = names.Single(s=>s.Last_Name=="Travolta").ID, SoftwareID=Softwares.Single(s=>s.title=="Microsoft").ID },
                new Models.SoftwareName {NameID = names.Single(s=>s.Last_Name=="Hague").ID, SoftwareID=Softwares.Single(s=>s.title=="Microsoft").ID },
                new Models.SoftwareName {NameID = names.Single(s=>s.Last_Name=="Travolta").ID, SoftwareID=Softwares.Single(s=>s.title=="Adobe").ID },
                new Models.SoftwareName {NameID = names.Single(s=>s.Last_Name=="Fox").ID, SoftwareID=Softwares.Single(s=>s.title=="Adobe").ID },
                new Models.SoftwareName {NameID = names.Single(s=>s.Last_Name=="Fox").ID, SoftwareID=Softwares.Single(s=>s.title=="Word").ID },
                new Models.SoftwareName {NameID = names.Single(s=>s.Last_Name=="Fox").ID, SoftwareID=Softwares.Single(s=>s.title=="Excel").ID },
            };
            SoftwareNames.ForEach(s => context.SoftwareNames.Add(s));


        }
    }
}
