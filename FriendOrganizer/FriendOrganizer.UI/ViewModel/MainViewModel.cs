using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private IFriendDetailViewModel _friendDetailViewModel;
        public INavigationViewModel NavigationViewModel { get; }

        private readonly Func<IFriendDetailViewModel> _friendDetailViewModelCreator;
        private readonly IMessageDialogService _messageDialogService;

        public ICommand CreateNewFriendCommand { get; }
        public IFriendDetailViewModel FriendDetailViewModel
        {
            get { return _friendDetailViewModel; }
            set
            {
                _friendDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(INavigationViewModel ANavigationViewModel, 
                            Func<IFriendDetailViewModel> AFriendDetailViewModel, 
                            IEventAggregator eventAggregator,
                            IMessageDialogService mds)
        {
            _eventAggregator = eventAggregator;
            _friendDetailViewModelCreator = AFriendDetailViewModel;
            _messageDialogService = mds;

            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>().Subscribe(OnOpenFriendDetailViewEvent);
            _eventAggregator.GetEvent<AfterFriendDeleteEvent>().Subscribe(OnAfterFriendDeleteEvent);

            CreateNewFriendCommand = new DelegateCommand(OnCreateNewFriendExecute);

            NavigationViewModel = ANavigationViewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void OnAfterFriendDeleteEvent(int friendId)
        {
            FriendDetailViewModel = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnCreateNewFriendExecute()
        {
            OnOpenFriendDetailViewEvent(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        private async void OnOpenFriendDetailViewEvent(int? AFriendId = null)
        {
            if (FriendDetailViewModel != null && FriendDetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You've made changes. Navigate away?", "Question?");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            FriendDetailViewModel = _friendDetailViewModelCreator();
            await FriendDetailViewModel.LoadAsync(AFriendId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }
        
    }
}
