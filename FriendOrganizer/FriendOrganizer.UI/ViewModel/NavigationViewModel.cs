using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        public ObservableCollection<LookupItem> Friends { get; }

        private readonly ILookupDataService _lookupDataService;
        private readonly IEventAggregator _eventAgregator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ALookupDataService"></param>
        public NavigationViewModel(ILookupDataService ALookupDataService, IEventAggregator ea)
        {
            Friends = new ObservableCollection<LookupItem>();
            _lookupDataService = ALookupDataService;
            _eventAgregator = ea;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task LoadAsync()
        {
            var lookup = await _lookupDataService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in lookup) Friends.Add(item);
        }

        private LookupItem _selectedFriend;

        public LookupItem SelectedFriend
        {
            get { return _selectedFriend; }
            set { _selectedFriend = value;
                OnPropertyChanged();
                if (_selectedFriend != null)
                {
                    _eventAgregator.GetEvent<OpenFriendDetailViewEvent>().Publish(_selectedFriend.Id);
                }
            }
        }


    }
}
