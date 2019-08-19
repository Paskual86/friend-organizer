namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        public int Id { get; }
        private string _displayMember;

        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AId"></param>
        /// <param name="ADisplayMember"></param>
        public NavigationItemViewModel(int AId, string ADisplayMember)
        {
            Id = AId;
            _displayMember = ADisplayMember;
        }
    }
}
