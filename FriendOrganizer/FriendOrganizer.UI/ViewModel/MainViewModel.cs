using Autofac.Features.Indexed;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private IDetailViewModel _selectedDetailViewModel;
        private readonly IMessageDialogService _messageDialogService;

        public ICommand CreateNewDetailCommand { get; }
        public INavigationViewModel NavigationViewModel { get; }

        private readonly IIndex<string, IDetailViewModel> _detailViewModelCreator;

        public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

        public IDetailViewModel SelectedDetailViewModel
        {
            get { return _selectedDetailViewModel; }
            set
            {
                _selectedDetailViewModel = value;
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

            DetailViewModels = new ObservableCollection<IDetailViewModel>();

            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnOpenDetailViewEvent);
            _eventAggregator.GetEvent<AfterDetailDeleteEvent>().Subscribe(OnAfterDetailDeleteEvent);
            _eventAggregator.GetEvent<AfterDetailClosedEvent>().Subscribe(OnAfterDetailCloseEvent);

            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
        }

        /// <summary>
        /// Raises the <see cref="E:AfterDetailCloseEvent" /> event.
        /// </summary>
        /// <param name="obj">The <see cref="AfterDetailClosedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnAfterDetailCloseEvent(AfterDetailClosedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void OnAfterDetailDeleteEvent(AfterDetailDeleteEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        /// <summary>
        /// Removes the detail view model.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="viewModelName">Name of the view model.</param>
        private void RemoveDetailViewModel(int id, string viewModelName)
        {
            var detailViewModel = DetailViewModels
                                                .SingleOrDefault(vm => vm.Id == id
                                                                && vm.GetType().Name == viewModelName);

            if (detailViewModel != null) DetailViewModels.Remove(detailViewModel);
        }

        private int nextNewItemId = 0;
        /// <summary>
        /// 
        /// </summary>
        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailViewEvent(new OpenDetailViewEventArgs() { Id = nextNewItemId--, 
                                                                ViewModelName = viewModelType.Name});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        private async void OnOpenDetailViewEvent(OpenDetailViewEventArgs args)
        {
            var detailViewModel = DetailViewModels
                                    .SingleOrDefault(vm => vm.Id == args.Id 
                                                    && vm.GetType().Name == args.ViewModelName);

            if(detailViewModel == null)
            {
                detailViewModel = _detailViewModelCreator[args.ViewModelName];
                await detailViewModel.LoadAsync(args.Id);
                DetailViewModels.Add(detailViewModel);
            }
            
            SelectedDetailViewModel = _detailViewModelCreator[args.ViewModelName];
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
