using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private IFriendDetailViewModel _friendDetailViewModel;
        public INavigationViewModel NavigationViewModel { get; }

        private readonly Func<IFriendDetailViewModel> _friendDetailViewModelCreator;
        private readonly IMessageDialogService _messageDialogService;

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
            NavigationViewModel = ANavigationViewModel;
            _friendDetailViewModelCreator = AFriendDetailViewModel;
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>().Subscribe(OnOpenFriendDetailViewEvent);
            _messageDialogService = mds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        private async void OnOpenFriendDetailViewEvent(int AFriendId)
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
