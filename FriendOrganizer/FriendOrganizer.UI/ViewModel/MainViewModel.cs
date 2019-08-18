using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IFriendDataService _friendDataService;
        private Friend _selectedFriend;

        public MainViewModel(IFriendDataService fds)
        {
            Friends = new ObservableCollection<Friend>();
            _friendDataService = fds;
        }

        public async Task LoadAsync()
        {
            var friends = await _friendDataService.GetAllAsync();
            Friends.Clear();

            foreach (var friend in friends)
            {
                Friends.Add(friend);
            }
        }
        public ObservableCollection<Friend> Friends { get; set; }

        public Friend SelectedFriend
        {
            get { return _selectedFriend; }
            set
            {
                _selectedFriend = value;
                OnPropertyChanged();
            }
        }
    }
}
