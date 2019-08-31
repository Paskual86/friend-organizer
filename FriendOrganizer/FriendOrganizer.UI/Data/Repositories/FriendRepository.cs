using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly FriendOrganizerDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cc"></param>
        public FriendRepository(FriendOrganizerDbContext cc)
        {
            _context = cc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Friend> GetByIdAsync(int AFriendId)
        {
            return await _context.Friends.SingleAsync(f => f.Id == AFriendId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AValue"></param>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
