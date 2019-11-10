using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
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
        protected async virtual void OnCloseDetailViewExecute() 
        {
            if (HasChanges)
            {
                var result = await MessageDialogService.ShowOkCancelDialogAsync("You 've made changes. Close this Item?", "Question");
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

        /// <summary>
        /// Raises the collection save event.
        /// </summary>
        protected virtual void RaiseCollectionSaveEvent() 
        {
            EventAggregator.GetEvent<AfterCollectionSaveEvent>()
                .Publish(new AfterCollectionSaveEventArgs
                {
                    ViewModelName = this.GetType().Name
                });
        }

        /// <summary>
        /// Saves the with optimistic concurrency asynchronous.
        /// </summary>
        /// <param name="funcSaveAsync">The function save asynchronous.</param>
        /// <param name="actionPostSave">The action post save.</param>
        protected async Task SaveWithOptimisticConcurrencyAsync(Func<Task> funcSaveAsync, Action actionPostSave)
        {
            try
            {
                await funcSaveAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var databaseValues = ex.Entries.Single().GetDatabaseValues();
                if (databaseValues == null)
                {
                    await MessageDialogService.ShowInfoDialogAsync("The entity has been deleted by other user.");
                    RaiseDetailDeletedEvent(Id);
                    return;
                }

                var result = await MessageDialogService.ShowOkCancelDialogAsync($"The entity has been changed in the meantime by someone else. Click Ok to save your changes anyway, click Cancel to reload the entity from the database.", "Question");
                if (result == MessageDialogResult.OK)
                {
                    // Update the original values 
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    await funcSaveAsync();
                }
                else
                {
                    await ex.Entries.Single().ReloadAsync();
                    await LoadAsync(Id);
                }
            }

            actionPostSave();
            
        }
    }
}
