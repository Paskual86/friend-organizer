﻿using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private readonly IFriendDataService _friendDataService;
        private readonly IEventAggregator _eventAgregator;
        private FriendWrapper _friend;

        public FriendWrapper Friend
        {
            get { return _friend; }
            set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
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
            // Geenrate Event
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        /// <returns></returns>
        public async Task LoadAsync(int AFriendId)
        {
            var friend = await _friendDataService.GetByIdAsync(AFriendId);
            Friend = new FriendWrapper(friend);
            Friend.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool OnSaveCanExecute()
        {
            return Friend!=null && !Friend.HasErrors;
        }

        /// <summary>
        /// 
        /// </summary>
        private async void OnSaveExecute()
        {
            await _friendDataService.SaveAsync(Friend.Model);
            _eventAgregator.GetEvent<AfterFriendSaveEvent>().Publish(new AfterFriendSaveEventArgs { Id = Friend.Id, DisplayMember = $"{Friend.FirstName} {Friend.LastName}" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        private async void OnOpenFriendDetailViewEvent(int AFriendId)
        {
            await LoadAsync(AFriendId);
        }


        
    }
}
