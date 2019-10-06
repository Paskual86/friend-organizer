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
        private readonly string _detailViewModelName;

        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenDetailViewCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AId"></param>
        /// <param name="ADisplayMember"></param>
        public NavigationItemViewModel(int AId, string ADisplayMember, string detailViewModelName, IEventAggregator ea)
        {
            Id = AId;
            _displayMember = ADisplayMember;
            _eventAgregator = ea;
            _detailViewModelName = detailViewModelName;
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnOpenDetailViewExecute()
        {
            _eventAgregator.GetEvent<OpenDetailViewEvent>().Publish(
                new OpenDetailViewEventArgs()
                {
                    Id = this.Id,
                    ViewModelName = _detailViewModelName
                }
                );
        }
    }
}
