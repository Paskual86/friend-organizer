using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDialogService _messageDialog;
        private readonly IFriendRepository _friendRepository;
        private FriendWrapper _friend;
        private bool _hasChanges;

        public FriendWrapper Friend
        {
            get { return _friend; }
            set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fds"></param>
        /// <param name="ea"></param>
        public FriendDetailViewModel(IFriendRepository fds, IEventAggregator ea, IMessageDialogService mds )
        {
            _friendRepository = fds;
            _eventAggregator = ea;
            _messageDialog = mds;
            // Geenrate Event
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
        }

        private async void OnDeleteExecute()
        {
            var result = _messageDialog.ShowOkCancelDialog($"Do you really want to delete the Friend {Friend.FirstName} {Friend.LastName}", "Question");
            if (result == MessageDialogResult.OK)
            {
                _friendRepository.Remove(Friend.Model);
                await _friendRepository.SaveAsync();
                _eventAggregator.GetEvent<AfterFriendDeleteEvent>().Publish(Friend.Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        /// <returns></returns>
        public async Task LoadAsync(int? AFriendId)
        {
            var friend = AFriendId.HasValue
                ? await _friendRepository.GetByIdAsync(AFriendId.Value)
                : CreateNewFriend();

            Friend = new FriendWrapper(friend);
            Friend.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _friendRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Friend.Id == 0)
            {
                // Little trick to execute the validation.
                Friend.FirstName = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async void OnSaveExecute()
        {
            await _friendRepository.SaveAsync();
            HasChanges = _friendRepository.HasChanges();
            _eventAggregator.GetEvent<AfterFriendSaveEvent>().Publish(new AfterFriendSaveEventArgs { Id = Friend.Id, DisplayMember = $"{Friend.FirstName} {Friend.LastName}" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool OnSaveCanExecute()
        {
            return Friend != null && !Friend.HasErrors && _hasChanges;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Friend CreateNewFriend()
        {
            var friend = new Friend();
            _friendRepository.Add(friend);            
            return friend;
        }

    }
}
