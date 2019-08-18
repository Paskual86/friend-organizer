using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FriendOrganizer.UI.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string APropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(APropertyName));

        }
    }
}
