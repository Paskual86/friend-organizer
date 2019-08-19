using FriendOrganizer.Model;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data
{
    public interface IFriendDataService
    {
        Task<Friend> GetByIdAsync(int AFriendId);
        Task SaveAsync(Friend AValue);
    }
}