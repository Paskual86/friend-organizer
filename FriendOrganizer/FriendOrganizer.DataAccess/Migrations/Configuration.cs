namespace FriendOrganizer.DataAccess.Migrations
{
    using FriendOrganizer.Model;
    using System.Data.Entity.Migrations;

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
                new Friend { Id = 1, FirstName = "Pablo", LastName = "Andrada", Email = "pablo.andrada@nyp.com" },
                new Friend { Id = 1, FirstName = "Noelia", LastName = "Lopez", Email = "none.lopez@nyp.com" },
                new Friend { Id = 1, FirstName = "Noah", LastName = "Andrada", Email = "noah.andrada@nyp.com" },
                new Friend { Id = 1, FirstName = "Pilar", LastName = "Andrada", Email = "pilar.andrada@nyp.com" });
        }
    }
}
