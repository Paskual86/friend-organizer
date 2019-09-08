using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDialogService _messageDialog;
        private readonly IProgrammingLanguageLookupDataService _lookupDataServiceProgrammingLanguage;
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
        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fds"></param>
        /// <param name="ea"></param>
        public FriendDetailViewModel(IFriendRepository fds, IEventAggregator ea, IMessageDialogService mds, IProgrammingLanguageLookupDataService ldspl )
        {
            _friendRepository = fds;
            _eventAggregator = ea;
            _messageDialog = mds;
            _lookupDataServiceProgrammingLanguage = ldspl;
            // Geenrate Event
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);

            ProgrammingLanguages = new ObservableCollection<LookupItem>();
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
            InitializeFriend(friend);

            await LoadProgrammingLanguageLookupAsync();
        }

        private void InitializeFriend(Friend friend)
        {
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

        private async Task LoadProgrammingLanguageLookupAsync()
        {
            ProgrammingLanguages.Clear();
            ProgrammingLanguages.Add(new NullLookupItem());
            var lookup = await _lookupDataServiceProgrammingLanguage.GetProgrammingLanguageLookupAsync();
            foreach (var li in lookup)
            {
                ProgrammingLanguages.Add(li);
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
