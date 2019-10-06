using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private bool _hasChanges;
        protected readonly IEventAggregator EventAggregator;

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

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailViewModelBase"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public DetailViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);

        }
        /// <summary>
        /// Called when [delete execute].
        /// </summary>
        protected abstract void OnDeleteExecute();
        /// <summary>
        /// Called when [save can execute].
        /// </summary>
        /// <returns></returns>
        protected abstract bool OnSaveCanExecute();
        /// <summary>
        /// Called when [save execute].
        /// </summary>
        protected abstract void OnSaveExecute();
        /// <summary>
        /// Loads the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public abstract Task LoadAsync(int? id);
        /// <summary>
        /// Raises the detail deleted event.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        protected virtual void RaiseDetailDeletedEvent(int modelId) 
        {
            EventAggregator.GetEvent<AfterDetailDeleteEvent>().Publish(new AfterDetailDeleteEventArgs()
            {
                Id = modelId,
                ViewModelName = this.GetType().Name
            });
        }
        /// <summary>
        /// Raises the detail save event.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="displayMember">The display member.</param>
        protected virtual void RaiseDetailSaveEvent(int modelId, string  displayMember)
        {
            EventAggregator.GetEvent<AfterDetailSaveEvent>().Publish(new AfterDetailSaveEventArgs()
            {
                Id = modelId,
                DisplayMember = displayMember,
                ViewModelName = this.GetType().Name
            });
        }
    }
}
