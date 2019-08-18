using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : INavigationViewModel
    {
        public ObservableCollection<LookupItem> Friends { get; }

        private readonly ILookupDataService _lookupDataService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ALookupDataService"></param>
        public NavigationViewModel(ILookupDataService ALookupDataService)
        {
            Friends = new ObservableCollection<LookupItem>();
            _lookupDataService = ALookupDataService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task LoadAsync()
        {
            var lookup = await _lookupDataService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in lookup) Friends.Add(item);
        }
    }
}
