using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Wrapper
{
    public class MeetingWrapper : ModelWrapper<Meeting>
    {
        public MeetingWrapper(Meeting model) : base(model)
        {
        }

        public int Id { get { return Model.Id; } }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the date from.
        /// </summary>
        /// <value>
        /// The date from.
        /// </value>
        public System.DateTime DateFrom
        {
            get { return GetValue<System.DateTime>(); }
            set
            {
                SetValue(value);
                if (DateTo < DateFrom) DateTo = DateFrom;
            }
        }

        /// <summary>
        /// Gets or sets the date to.
        /// </summary>
        /// <value>
        /// The date to.
        /// </value>
        public System.DateTime DateTo
        {
            get { return GetValue<System.DateTime>(); }
            set
            {
                SetValue(value);
                if (DateTo < DateFrom) DateFrom = DateTo;
            }
        }
    }
}
