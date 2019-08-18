using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private readonly IFriendDataService _friendDataService;
        private readonly IEventAggregator _eventAgregator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fds"></param>
        /// <param name="ea"></param>
        public FriendDetailViewModel(IFriendDataService fds, IEventAggregator ea)
        {
            _friendDataService = fds;
            _eventAgregator = ea;
            _eventAgregator.GetEvent<OpenFriendDetailViewEvent>().Subscribe(OnOpenFriendDetailViewEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        private async void OnOpenFriendDetailViewEvent(int AFriendId)
        {
            await LoadAsync(AFriendId);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        /// <returns></returns>
        public async Task LoadAsync(int AFriendId)
        {
            Friend = await _friendDataService.GetByIdAsync(AFriendId);
        }
    }
}
