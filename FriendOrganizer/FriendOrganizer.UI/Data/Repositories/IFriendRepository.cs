using FriendOrganizer.Model;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{


    public interface IFriendRepository : IGenericRepository<Friend>
    {
        /// <summary>
        /// Removes the phone number.
        /// </summary>
        /// <param name="model">The model.</param>
        void RemovePhoneNumber(FriendPhoneNumber model);
        /// <summary>
        /// Determines whether [has meeting asynchronous] [the specified friend identifier].
        /// </summary>
        /// <param name="friendId">The friend identifier.</param>
        /// <returns></returns>
        Task<bool> HasMeetingAsync(int friendId);
    }
}