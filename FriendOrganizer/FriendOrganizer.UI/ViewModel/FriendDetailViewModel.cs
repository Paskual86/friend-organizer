using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IFriendDataService _friendDataService;
        public FriendDetailViewModel(IFriendDataService fds)
        {
            _friendDataService = fds;
        }


        private Friend _friend;

        public Friend Friend
        {
            get { return _friend; }
            set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync(int AFriendId)
        {
            Friend = await _friendDataService.GetByIdAsync(AFriendId);
        }
    }
}
