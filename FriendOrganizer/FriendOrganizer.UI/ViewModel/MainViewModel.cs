using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public INavigationViewModel NavigationViewModel { get; }
        public IFriendDetailViewModel FriendDetailViewModel { get; }

        public MainViewModel(INavigationViewModel ANavigationViewModel, IFriendDetailViewModel AFriendDetailViewModel)
        {
            NavigationViewModel = ANavigationViewModel;
            FriendDetailViewModel = AFriendDetailViewModel;
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
