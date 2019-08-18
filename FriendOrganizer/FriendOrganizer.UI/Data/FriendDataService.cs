using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        private readonly Func<FriendOrganizerDbContext> _contextCreator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cc"></param>
        public FriendDataService(Func<FriendOrganizerDbContext> cc)
        {
            _contextCreator = cc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Friend> GetByIdAsync(int AFriendId)
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Friends.AsNoTracking().SingleAsync(f => f.Id == AFriendId);
            }

        }
    }
}
