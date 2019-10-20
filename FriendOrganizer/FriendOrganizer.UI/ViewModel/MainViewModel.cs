using Autofac.Features.Indexed;
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
        private IDetailViewModel _detailViewModel;
        private readonly IMessageDialogService _messageDialogService;

        public ICommand CreateNewDetailCommand { get; }
        public INavigationViewModel NavigationViewModel { get; }

        private readonly IIndex<string, IDetailViewModel> _detailViewModelCreator;

        public IDetailViewModel DetailViewModel
        {
            get { return _detailViewModel; }
            set
            {
                _detailViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(INavigationViewModel ANavigationViewModel, 
                            IIndex<string, IDetailViewModel> detailViewModelCreator,
                            IEventAggregator eventAggregator,
                            IMessageDialogService mds)
        {
            NavigationViewModel = ANavigationViewModel;
            _detailViewModelCreator = detailViewModelCreator;

            _eventAggregator = eventAggregator;
            _messageDialogService = mds;

            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnOpenDetailViewEvent);
            _eventAggregator.GetEvent<AfterDetailDeleteEvent>().Subscribe(OnAfterDetailDeleteEvent);

            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void OnAfterDetailDeleteEvent(AfterDetailDeleteEventArgs args)
        {
            DetailViewModel = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailViewEvent(new OpenDetailViewEventArgs() { ViewModelName = viewModelType.Name});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        private async void OnOpenDetailViewEvent(OpenDetailViewEventArgs args)
        {
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You've made changes. Navigate away?", "Question?");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            
            DetailViewModel = _detailViewModelCreator[args.ViewModelName];
            
            await DetailViewModel.LoadAsync(args.Id);
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
