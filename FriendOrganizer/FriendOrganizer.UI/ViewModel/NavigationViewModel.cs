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
        public ObservableCollection<NavigationItemViewModel> Meetings { get; }

        private readonly ILookupDataServiceFriend _lookupDataService;
        private readonly IMeetingLookupDataService _meetingLookupDataService;
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
                    _eventAgregator.GetEvent<OpenDetailViewEvent>().
                        Publish(new OpenDetailViewEventArgs()
                        {
                            Id = _selectedFriend.Id,
                            ViewModelName = nameof(FriendDetailViewModel)
                        });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ALookupDataService"></param>
        public NavigationViewModel(ILookupDataServiceFriend ALookupDataService
            , IMeetingLookupDataService meetingLookupDataService
            , IEventAggregator ea)
        {
            Friends = new ObservableCollection<NavigationItemViewModel>();
            Meetings = new ObservableCollection<NavigationItemViewModel>();

            _lookupDataService = ALookupDataService;
            _meetingLookupDataService = meetingLookupDataService;
            _eventAgregator = ea;

            _eventAgregator.GetEvent<AfterDetailSaveEvent>().Subscribe(OnAfterDetailSaved);
            _eventAgregator.GetEvent<AfterDetailDeleteEvent>().Subscribe(OnAfterDetailDeleted);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friendId"></param>
        private void OnAfterDetailDeleted(AfterDetailDeleteEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    {
                        OnAfterDetailDeleted(Friends, args);
                    }
                    break;
                case nameof(MeetingDetailViewModel):
                    {
                        OnAfterDetailDeleted(Friends, args);
                    }
                    break;
            }
        }

        /// <summary>
        /// Called when [after detail deleted].
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="args">The <see cref="AfterDetailDeleteEventArgs"/> instance containing the event data.</param>
        private void OnAfterDetailDeleted(ObservableCollection<NavigationItemViewModel> items, AfterDetailDeleteEventArgs args)
        {
            var item = items.SingleOrDefault(f => f.Id == args.Id);
            if (item != null)
            {
                Friends.Remove(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnAfterDetailSaved(AfterDetailSaveEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    {
                        OnAfterDetailSave(Friends, args);
                        break;
                    }
                case nameof(MeetingDetailViewModel):
                    {
                        OnAfterDetailSave(Meetings, args);
                        break;
                    }
            }
        }

        /// <summary>
        /// Called when [after detail save].
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="args">The <see cref="AfterDetailSaveEventArgs"/> instance containing the event data.</param>
        private void OnAfterDetailSave(ObservableCollection<NavigationItemViewModel> items, AfterDetailSaveEventArgs args)
        {
            var lookItem = items.SingleOrDefault(l => l.Id == args.Id);
            if (lookItem == null)
            {
                items.Add(new NavigationItemViewModel(args.Id,
                            args.DisplayMember,
                            args.ViewModelName,
                            _eventAgregator));
            }
            else
            {
                lookItem.DisplayMember = args.DisplayMember;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task LoadAsync()
        {
            var lookup = await _lookupDataService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in lookup) Friends.Add(new NavigationItemViewModel(item.Id, item.DisplayMember, 
                nameof(FriendDetailViewModel), 
                _eventAgregator));

            // Load the Meetings
            lookup = await _meetingLookupDataService.GetMeetingLookupAsync();
            Meetings.Clear();
            foreach (var item in lookup) Meetings.Add(new NavigationItemViewModel(item.Id, item.DisplayMember,
                nameof(MeetingDetailViewModel),
                _eventAgregator));
        }
    }
}
