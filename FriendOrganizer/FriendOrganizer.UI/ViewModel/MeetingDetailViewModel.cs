using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private IMeetingRepository _meetingRepository;
        private MeetingWrapper _meeting;

        private Friend _selectedAvailableFriend;
        private Friend _selectedAddedFriend;
        private List<Friend> _allFriends;

        public ICommand AddFriendCommand { get; }
        public ICommand RemoveFriendCommand { get; }

        public ObservableCollection<Friend> AddedFriends { get; }
        public ObservableCollection<Friend> AvailableFriends { get; }

        /// <summary>
        /// Gets or sets the selected available friend.
        /// </summary>
        /// <value>
        /// The selected available friend.
        /// </value>
        public Friend SelectedAvailableFriend
        {
            get { return _selectedAvailableFriend; }
            set { 
                _selectedAvailableFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected added friend.
        /// </summary>
        /// <value>
        /// The selected added friend.
        /// </value>
        public Friend SelectedAddedFriend
        {
            get { return _selectedAddedFriend; }
            set { 
                _selectedAddedFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveFriendCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingDetailViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="meetingRepository">The meeting repository.</param>
        /// <param name="messageDialogService">The message dialog service.</param>
        public MeetingDetailViewModel(IEventAggregator eventAggregator
            , IMeetingRepository meetingRepository
            , IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            _meetingRepository = meetingRepository;
            EventAggregator.GetEvent<AfterDetailSaveEvent>().Subscribe(OnAfterDetailSaveEvent);
            EventAggregator.GetEvent<AfterDetailDeleteEvent>().Subscribe(OnAfterDetailDeleteEvent);
            AddedFriends = new ObservableCollection<Friend>();
            AvailableFriends = new ObservableCollection<Friend>();

            AddFriendCommand = new DelegateCommand(OnAddFriendExecute, OnAddFriendCanExecute);
            RemoveFriendCommand = new DelegateCommand(OnRemoveFriendExecute, OnRemoveFriendCanExecute);
        }

        /// <summary>
        /// Raises the <see cref="E:AfterDetailDeleteEvent" /> event.
        /// </summary>
        /// <param name="args">The <see cref="AfterDetailDeleteEventArgs"/> instance containing the event data.</param>
        private async void OnAfterDetailDeleteEvent(AfterDetailDeleteEventArgs args)
        {
            if (args.ViewModelName == nameof(FriendDetailViewModel))
            {
                await ReloadFriendAsync();
            }
        }

        /// <summary>
        /// Reloads the friend asynchronous.
        /// </summary>
        private async Task ReloadFriendAsync()
        {
            _allFriends = await _meetingRepository.GetAllFriendsAsync();
            SetupPickList();
        }

        /// <summary>
        /// Raises the <see cref="E:AfterDetailSaveEvent" /> event.
        /// </summary>
        /// <param name="args">The <see cref="FriendOrganizer.UI.Event.AfterDetailSaveEventArgs" /> instance containing the event data.</param>
        private async void OnAfterDetailSaveEvent(AfterDetailSaveEventArgs args)
        {
            if (args.ViewModelName == nameof(FriendDetailViewModel))
            {
                await _meetingRepository.ReloadFriendAsync(args.Id);
                await ReloadFriendAsync();
            }
        }

        /// <summary>
        /// Called when [remove friend can execute].
        /// </summary>
        /// <returns></returns>
        private bool OnRemoveFriendCanExecute()
        {
            return SelectedAddedFriend != null;
        }

        /// <summary>
        /// Called when [remove friend execute].
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void OnRemoveFriendExecute()
        {
            var friendToRemove = SelectedAddedFriend;

            Meeting.Model.Friends.Remove(friendToRemove);
            AddedFriends.Remove(friendToRemove);
            AvailableFriends.Add(friendToRemove);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Called when [add friend can execute].
        /// </summary>
        /// <returns></returns>
        private bool OnAddFriendCanExecute()
        {
            return SelectedAvailableFriend != null;
        }

        /// <summary>
        /// Called when [add friend execute].
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void OnAddFriendExecute()
        {
            var friendToAdd = SelectedAvailableFriend;

            Meeting.Model.Friends.Add(friendToAdd);
            AddedFriends.Add(friendToAdd);
            AvailableFriends.Remove(friendToAdd);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public MeetingWrapper Meeting
        {
            get { return _meeting; }
            private set
            {
                _meeting = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Loads the asynchronous.
        /// </summary>
        /// <param name="meetingId">The identifier.</param>
        public override async Task LoadAsync(int meetingId)
        {
            var meeting = meetingId>0
                ? await _meetingRepository.GetByIdAsync(meetingId)
                : CreateNewMeeting();
            Id = meetingId;
            InitializeMeeting(meeting);

            _allFriends = await _meetingRepository.GetAllFriendsAsync();

            SetupPickList();
        }

        /// <summary>
        /// Setups the pick list.
        /// </summary>
        private void SetupPickList()
        {
            var meetingFriendIds = Meeting.Model.Friends.Select(f => f.Id).ToList();
            var addedFriends = _allFriends.Where(f => meetingFriendIds.Contains(f.Id)).OrderBy(f => f.FirstName);
            var availableFriends = _allFriends.Except(addedFriends).OrderBy(f => f.FirstName);

            AddedFriends.Clear();
            AvailableFriends.Clear();

            foreach (var addedFriend in addedFriends) AddedFriends.Add(addedFriend);
            foreach (var af in availableFriends) AvailableFriends.Add(af);

        }

        /// <summary>
        /// Called when [delete execute].
        /// </summary>
        protected async override void OnDeleteExecute()
        {
            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you really want to delete", "Question");
            if (result == MessageDialogResult.OK) 
            {
                _meetingRepository.Remove(Meeting.Model);
                await _meetingRepository.SaveAsync();
                RaiseDetailDeletedEvent(Meeting.Id);
            }
        }

        /// <summary>
        /// Called when [save can execute].
        /// </summary>
        /// <returns></returns>
        protected override bool OnSaveCanExecute()
        {
            return Meeting != null && !Meeting.HasErrors && HasChanges;
        }

        /// <summary>
        /// Called when [save execute].
        /// </summary>
        protected override async void OnSaveExecute()
        {
            await _meetingRepository.SaveAsync();
            HasChanges = _meetingRepository.HasChanges();
            Id = Meeting.Id;
            RaiseDetailSaveEvent(Meeting.Id, Meeting.Title);
        }

        /// <summary>
        /// Initializes the meeting.
        /// </summary>
        /// <param name="meeting">The meeting.</param>
        private void InitializeMeeting(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting);

            Meeting.PropertyChanged += (s, e) => {
                if (!HasChanges) 
                {
                    HasChanges = _meetingRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Meeting.HasErrors)) 
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if (e.PropertyName == nameof(Meeting.Title)) SetTitle();
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            
            // Little trick to raise the validation
            if (Meeting.Id == 0) 
            {
                Meeting.Title = "";
            }
            SetTitle();
        }

        /// <summary>
        /// Sets the title.
        /// </summary>
        private void SetTitle()
        {
            Title = Meeting.Title;
        }

        /// <summary>
        /// Creates the new meeting.
        /// </summary>
        /// <returns></returns>
        private Meeting CreateNewMeeting()
        {
            var meeting = new Meeting
            {
                DateFrom = DateTime.Now.Date,
                DateTo = DateTime.Now.Date
            };
            _meetingRepository.Add(meeting);
            return meeting;
        }
    }
}
