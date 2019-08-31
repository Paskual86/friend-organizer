using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fds"></param>
        /// <param name="ea"></param>
        public FriendDetailViewModel(IFriendRepository fds, IEventAggregator ea)
        {
            _friendRepository = fds;
            _eventAggregator = ea;
            // Geenrate Event
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFriendId"></param>
        /// <returns></returns>
        public async Task LoadAsync(int AFriendId)
        {
            var friend = await _friendRepository.GetByIdAsync(AFriendId);
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
        private async void OnSaveExecute()
        {
            await _friendRepository.SaveAsync();
            HasChanges = _friendRepository.HasChanges();
            _eventAggregator.GetEvent<AfterFriendSaveEvent>().Publish(new AfterFriendSaveEventArgs { Id = Friend.Id, DisplayMember = $"{Friend.FirstName} {Friend.LastName}" });
        }
        
    }
}
