using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
    {
        private readonly IMessageDialogService _messageDialog;
        private readonly IProgrammingLanguageLookupDataService _lookupDataServiceProgrammingLanguage;
        private readonly IFriendRepository _friendRepository;
        private FriendWrapper _friend;
        private FriendPhoneNumberWrapper _friendPhoneNumberWrapper;

        public FriendWrapper Friend
        {
            get { return _friend; }
            set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public FriendPhoneNumberWrapper SelectedPhoneNumber
        {
            get { return _friendPhoneNumberWrapper; }
            set
            {
                _friendPhoneNumberWrapper = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddPhoneNumberCommand { get; }
        public ICommand RemovePhoneNumberCommand { get; }
        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }
        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fds"></param>
        /// <param name="ea"></param>
        public FriendDetailViewModel(IFriendRepository fds
                                    , IEventAggregator ea
                                    , IMessageDialogService mds
                                    , IProgrammingLanguageLookupDataService ldspl )
            : base(ea)
        {
            _friendRepository = fds;
            _messageDialog = mds;
            _lookupDataServiceProgrammingLanguage = ldspl;
            // Geenrate Event
            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);
            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
        }

        /// <summary>
        /// Called when [add phone number execute].
        /// </summary>
        private void OnAddPhoneNumberExecute()
        {
            var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            PhoneNumbers.Add(newNumber);
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            newNumber.Number = string.Empty; // Trigger Validation.
        }

        

        /// <summary>
        /// Called when [remove phone number execute].
        /// </summary>
        private void OnRemovePhoneNumberExecute()
        {
            SelectedPhoneNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            _friendRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);
            
            PhoneNumbers.Remove(SelectedPhoneNumber);
            SelectedPhoneNumber = null;
            HasChanges = _friendRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedPhoneNumber != null;

        }

        /// <summary>
        /// Called when [delete execute].
        /// </summary>
        protected override async void OnDeleteExecute()
        {
            var result = _messageDialog.ShowOkCancelDialog($"Do you really want to delete the Friend {Friend.FirstName} {Friend.LastName}", "Question");
            if (result == MessageDialogResult.OK)
            {
                _friendRepository.Remove(Friend.Model);
                await _friendRepository.SaveAsync();
                RaiseDetailDeletedEvent(Friend.Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        /// <returns></returns>
        public override async Task LoadAsync(int? AFriendId)
        {
            var friend = AFriendId.HasValue
                ? await _friendRepository.GetByIdAsync(AFriendId.Value)
                : CreateNewFriend();
            InitializeFriend(friend);
            InitializeFriendPhoneNumbers(friend.PhoneNumbers);

            await LoadProgrammingLanguageLookupAsync();
        }

        /// <summary>
        /// Initializes the friend phone numbers.
        /// </summary>
        /// <param name="phoneNumbers">The phone numbers.</param>
        private void InitializeFriendPhoneNumbers(ICollection<FriendPhoneNumber> phoneNumbers)
        {
            foreach (var w in PhoneNumbers)
            {
                w.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            }
            PhoneNumbers.Clear();
            foreach (var friendPhoneNumber in phoneNumbers)
            {
                var wrapper = new FriendPhoneNumberWrapper(friendPhoneNumber);
                PhoneNumbers.Add(wrapper);
                wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the FriendPhoneNumberWrapper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _friendRepository.HasChanges();
            }
            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
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
        protected override async void OnSaveExecute()
        {
            await _friendRepository.SaveAsync();
            HasChanges = _friendRepository.HasChanges();
            RaiseDetailSaveEvent(Friend.Id, $"{Friend.FirstName} {Friend.LastName}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool OnSaveCanExecute()
        {
            return Friend != null && !Friend.HasErrors && PhoneNumbers.All(pn => !pn.HasErrors) && HasChanges;
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
