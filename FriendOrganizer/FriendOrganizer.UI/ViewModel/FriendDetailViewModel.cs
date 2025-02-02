﻿using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
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
            : base(ea, mds)
        {
            _friendRepository = fds;
            _lookupDataServiceProgrammingLanguage = ldspl;

            EventAggregator.GetEvent<AfterCollectionSaveEvent>().Subscribe(OnAfterCollectionSaveEvent);
            // Geenrate Event
            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);
            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
        }

        /// <summary>
        /// Raises the <see cref="E:AfterCollectionSaveEvent" /> event.
        /// </summary>
        /// <param name="args">The <see cref="AfterCollectionSaveEventArgs"/> instance containing the event data.</param>
        private async void OnAfterCollectionSaveEvent(AfterCollectionSaveEventArgs args)
        {
            if (args.ViewModelName == nameof(ProgrammingLanguageDetailViewModel))
            {
                await LoadProgrammingLanguageLookupAsync();
            }
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
            if (await _friendRepository.HasMeetingAsync(Friend.Id))
            {
                await MessageDialogService.ShowInfoDialogAsync($"{Friend.FirstName} {Friend.LastName} can't be deleted");
                return;
            }
            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you really want to delete the Friend {Friend.FirstName} {Friend.LastName}", "Question");

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
        /// <param name="friendId"></param>
        /// <returns></returns>
        public override async Task LoadAsync(int friendId)
        {
            var friend = friendId>0
                ? await _friendRepository.GetByIdAsync(friendId)
                : CreateNewFriend();

            Id = friendId;

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

                if (e.PropertyName == nameof(Friend.FirstName) || e.PropertyName == nameof(Friend.LastName)) SetTitle();

            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Friend.Id == 0)
            {
                // Little trick to execute the validation.
                Friend.FirstName = string.Empty;
            }
            SetTitle();
        }

        /// <summary>
        /// Sets the title.
        /// </summary>
        private void SetTitle()
        {
            Title = $"{Friend.FirstName} {Friend.LastName}";
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
            await SaveWithOptimisticConcurrencyAsync(_friendRepository.SaveAsync, () =>

            {
                HasChanges = _friendRepository.HasChanges();
                Id = Friend.Id;
                RaiseDetailSaveEvent(Friend.Id, $"{Friend.FirstName} {Friend.LastName}");
            });
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
