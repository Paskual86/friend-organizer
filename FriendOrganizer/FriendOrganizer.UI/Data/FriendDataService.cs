using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        private readonly Func<FriendOrganizerDbContext> _contextCreator;

        public FriendDataService(Func<FriendOrganizerDbContext> cc)
        {
            _contextCreator = cc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Friend>> GetAllAsync()
        {
            using (var ctx = _contextCreator())
            {
                /*var r = await ctx.Friends.AsNoTracking().ToListAsync();
                await Task.Delay(5000);
                return r;*/

                return await ctx.Friends.AsNoTracking().ToListAsync();
            }

        }
    }
}
