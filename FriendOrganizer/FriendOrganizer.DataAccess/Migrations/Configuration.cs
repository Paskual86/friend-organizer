namespace FriendOrganizer.DataAccess.Migrations
{
    using FriendOrganizer.Model;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDbContext context)
        {
            context.Friends.AddOrUpdate(
                f => f.FirstName,
                new Friend { FirstName = "Pablo", LastName = "Andrada", Email = "pablo.andrada@nyp.com" },
                new Friend { FirstName = "Noelia", LastName = "Lopez", Email = "none.lopez@nyp.com" },
                new Friend { FirstName = "Noah", LastName = "Andrada", Email = "noah.andrada@nyp.com" },
                new Friend { FirstName = "Pilar", LastName = "Andrada", Email = "pilar.andrada@nyp.com" });

            context.ProgrammingLanguages.AddOrUpdate(
                f => f.Name,
                new ProgrammingLanguage { Id = 1, Name = "C#" },
                new ProgrammingLanguage { Id = 2, Name = "Typescript" },
                new ProgrammingLanguage { Id = 3, Name = "F#" },
                new ProgrammingLanguage { Id = 4, Name = "Swift" },
                new ProgrammingLanguage { Id = 5, Name = "Java" });

            context.SaveChanges();

            context.FriendPhoneNumbers.AddOrUpdate(pn => pn.Number,
                new FriendPhoneNumber { Number = "+54 91134974854", FriendId = context.Friends.FirstOrDefault().Id });
        }
    }
}
