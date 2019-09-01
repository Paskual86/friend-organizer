using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        public int Id { get; }
        private string _displayMember;
        private readonly IEventAggregator _eventAgregator;

        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenFriendDetailViewCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AId"></param>
        /// <param name="ADisplayMember"></param>
        public NavigationItemViewModel(int AId, string ADisplayMember, IEventAggregator ea)
        {
            Id = AId;
            _displayMember = ADisplayMember;
            _eventAgregator = ea;
            OpenFriendDetailViewCommand = new DelegateCommand(OnOpenFriendDetailView);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnOpenFriendDetailView()
        {
            _eventAgregator.GetEvent<OpenFriendDetailViewEvent>().Publish(Id);
        }
    }
}
