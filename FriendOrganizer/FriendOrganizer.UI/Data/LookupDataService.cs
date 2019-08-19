using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data
{
    public class LookupDataService : ILookupDataService
    {
        private readonly Func<FriendOrganizerDbContext> _contextCreator;

        public LookupDataService(Func<FriendOrganizerDbContext> AContextCreator)
        {
            _contextCreator = AContextCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetFriendLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Friends.AsNoTracking().Select(f => new LookupItem { Id = f.Id, DisplayMember = f.FirstName + " " + f.LastName }).ToListAsync();
            }
        }
    }
}
