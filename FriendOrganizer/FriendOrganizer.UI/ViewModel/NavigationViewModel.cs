﻿using FriendOrganizer.UI.Data.Lookups;
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

        private readonly ILookupDataServiceFriend _lookupDataService;
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
        public NavigationViewModel(ILookupDataServiceFriend ALookupDataService, IEventAggregator ea)
        {
            Friends = new ObservableCollection<NavigationItemViewModel>();
            _lookupDataService = ALookupDataService;
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
                        var friend = Friends.SingleOrDefault(f => f.Id == args.Id);
                        if (friend != null)
                        {
                            Friends.Remove(friend);
                        }
                    }break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void OnAfterDetailSaved(AfterDetailSaveEventArgs obj)
        {
            switch (obj.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    {
                        var lookItem = Friends.SingleOrDefault(l => l.Id == obj.Id);
                        if (lookItem == null)
                        {
                            Friends.Add(new NavigationItemViewModel(obj.Id, 
                                        obj.DisplayMember, 
                                        nameof(FriendDetailViewModel), 
                                        _eventAgregator));
                        }
                        else
                        {
                            lookItem.DisplayMember = obj.DisplayMember;
                        }
                        break;
                    }
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
        }
    }
}
