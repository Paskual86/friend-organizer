using FriendOrganizer.Model;
using System;
using System.Runtime.CompilerServices;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendWrapper : ModelWrapper<Friend>
    {
        public FriendWrapper(Friend model) : base(model)
        {
        }

        public int Id { get { return Model.Id; } }
        
        public string FirstName { get { return GetValue<string>(); }
            set
            {
                SetValue(value);
                VaidateProperty();
            }
        }

        public string LastName { get { return GetValue<string>(); }
            set
            {
                SetValue(value);
            }
        }
        
        public string Email { get { return GetValue<string>(); }
            set
            {
                SetValue(value);
            }
        }

        private void VaidateProperty([CallerMemberName] string propertyName = null)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(FirstName):
                    {
                        if (string.Equals(FirstName, "Robot", StringComparison.OrdinalIgnoreCase))
                        {
                            AddError(propertyName, "Robots are not valid Friends");
                        }
                    }
                    break;

            }
        }
    }
}
