using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        public ObservableCollection<NavigationItemViewModel> Friends { get; }

        private readonly ILookupDataService _lookupDataService;
        private readonly IEventAggregator _eventAgregator;

        private NavigationItemViewModel _selectedFriend;

        public NavigationItemViewModel SelectedFriend
        {
            get { return _selectedFriend; }
            set
            {
                _selectedFriend = value;
                OnPropertyChanged();
                if (_selectedFriend != null)
                {
                    _eventAgregator.GetEvent<OpenFriendDetailViewEvent>().Publish(_selectedFriend.Id);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ALookupDataService"></param>
        public NavigationViewModel(ILookupDataService ALookupDataService, IEventAggregator ea)
        {
            Friends = new ObservableCollection<NavigationItemViewModel>();
            _lookupDataService = ALookupDataService;
            _eventAgregator = ea;
            _eventAgregator.GetEvent<AfterFriendSaveEvent>().Subscribe(OnAfterFriendSaved);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void OnAfterFriendSaved(AfterFriendSaveEventArgs obj)
        {
            var lookItem = Friends.Single(l => l.Id == obj.Id);
            lookItem.DisplayMember = obj.DisplayMember;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task LoadAsync()
        {
            var lookup = await _lookupDataService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in lookup) Friends.Add(new NavigationItemViewModel(item.Id, item.DisplayMember, _eventAgregator));
        }
    }
}
