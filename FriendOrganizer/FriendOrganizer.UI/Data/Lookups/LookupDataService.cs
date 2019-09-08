﻿using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Lookups
{
    /// <summary>
    /// 
    /// </summary>
    public class LookupDataService : IProgrammingLanguageLookupDataService, ILookupDataServiceFriend
    {
        private readonly Func<FriendOrganizerDbContext> _contextCreator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AContextCreator"></param>
        public LookupDataService(Func<FriendOrganizerDbContext> AContextCreator)
        {
            _contextCreator = AContextCreator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LookupItem>> GetFriendLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Friends.AsNoTracking().Select(f => new LookupItem { Id = f.Id, DisplayMember = f.FirstName + " " + f.LastName }).ToListAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LookupItem>> GetProgrammingLanguageLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.ProgrammingLanguages.AsNoTracking().Select(f => new LookupItem { Id = f.Id, DisplayMember = f.Name }).ToListAsync();
            }
        }
    }
}
