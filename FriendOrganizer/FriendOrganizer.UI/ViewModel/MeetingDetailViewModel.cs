using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private IMeetingRepository _meetingRepository;
        private MeetingWrapper _meeting;
        private IMessageDialogService _messageDialogService;

        public MeetingDetailViewModel(IEventAggregator eventAggregator
            , IMeetingRepository meetingRepository
            , IMessageDialogService messageDialogService) : base(eventAggregator)
        {
            _meetingRepository = meetingRepository;
            _messageDialogService = messageDialogService;
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

        public override async Task LoadAsync(int? id)
        {
            var meeting = id.HasValue
                ? await _meetingRepository.GetByIdAsync(id.Value)
                : CreateNewMeeting();
            InitializeMeeting(meeting);
        }

        protected override void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog($"Do you really want to delete", "Question");
            if (result == MessageDialogResult.OK) 
            {
                _meetingRepository.Remove(Meeting.Model);
                _meetingRepository.SaveAsync();
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
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            
            // Little trick to raise the validation
            if (Meeting.Id == 0) 
            {
                Meeting.Title = "";
            }
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
