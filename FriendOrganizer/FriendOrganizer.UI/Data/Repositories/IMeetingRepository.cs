using FriendOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="FriendOrganizer.UI.Data.Repositories.IGenericRepository{FriendOrganizer.Model.Meeting}" />
    public interface IMeetingRepository : IGenericRepository<Meeting>
    {
        Task<List<Friend>> GetAllFriendsAsync();
    }
}