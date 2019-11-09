using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private bool _hasChanges;
        private int _id;

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

        private string _title;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return _title; }
            protected set { _title = value;
                OnPropertyChanged();
            }
        }


        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand CloseDetailViewCommand { get; private set; }

        protected readonly IMessageDialogService MessageDialogService;

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DetailViewModelBase"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public DetailViewModelBase(IEventAggregator eventAggregator, IMessageDialogService mds)
        {
            EventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
            MessageDialogService = mds;
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
        public abstract Task LoadAsync(int id);
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

        /// <summary>
        /// Called when [close detail view execute].
        /// </summary>
        protected virtual void OnCloseDetailViewExecute() 
        {
            if (HasChanges)
            {
                var result = MessageDialogService.ShowOkCancelDialog("You 've made changes. Close this Item?", "Question");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            EventAggregator.GetEvent<AfterDetailClosedEvent>().Publish(new AfterDetailClosedEventArgs
            {
                Id = this.Id,
                ViewModelName = this.GetType().Name
            });
        }
    }
}
