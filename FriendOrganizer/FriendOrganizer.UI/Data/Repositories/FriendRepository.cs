﻿using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{

    public class FriendRepository : GenericRepository<Friend, FriendOrganizerDbContext>,  IFriendRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cc"></param>
        public FriendRepository(FriendOrganizerDbContext context)
            :base(context)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task<Friend> GetByIdAsync(int AFriendId)
        {
            return await Context.Friends
                .Include(f => f.PhoneNumbers)
                .SingleAsync(f => f.Id == AFriendId);
        }

        
        /// <summary>
        /// Removes the phone number.
        /// </summary>
        /// <param name="model">The model.</param>
        public void RemovePhoneNumber(FriendPhoneNumber model)
        {
            Context.FriendPhoneNumbers.Remove(model);
        }

        /// <summary>
        /// Determines whether [has meeting asynchronous] [the specified friend identifier].
        /// </summary>
        /// <param name="friendId">The friend identifier.</param>
        /// <returns></returns>
        public async Task<bool> HasMeetingAsync(int friendId) 
        {
            return await Context.Meetings.AsNoTracking()
                .Include(m => m.Friends)
                .AnyAsync(m => m.Friends.Any(f => f.Id == friendId));
        }

    }
}
