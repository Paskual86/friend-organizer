namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPrgrammingLanguage1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Friend", "FavoriteLanguageId");
            RenameColumn(table: "dbo.Friend", name: "FavoriteLanguate_Id", newName: "FavoriteLanguageId");
            RenameIndex(table: "dbo.Friend", name: "IX_FavoriteLanguate_Id", newName: "IX_FavoriteLanguageId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Friend", name: "IX_FavoriteLanguageId", newName: "IX_FavoriteLanguate_Id");
            RenameColumn(table: "dbo.Friend", name: "FavoriteLanguageId", newName: "FavoriteLanguate_Id");
            AddColumn("dbo.Friend", "FavoriteLanguageId", c => c.Int());
        }
    }
}
