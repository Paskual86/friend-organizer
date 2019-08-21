using FriendOrganizer.UI.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FriendOrganizer.UI.Wrapper
{
    public class NotifyDataErrorInfoBase : ViewModelBase, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errorByPropertyName = new Dictionary<string, List<string>>();

        public bool HasErrors => _errorByPropertyName.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return _errorByPropertyName.ContainsKey(propertyName) ? _errorByPropertyName[propertyName] : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnErrorChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            base.OnPropertyChanged(nameof(HasErrors));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="error"></param>
        protected virtual void AddError(string propertyName, string error)
        {
            if (!_errorByPropertyName.ContainsKey(propertyName))
            {
                _errorByPropertyName[propertyName] = new List<string>();
            }

            if (!_errorByPropertyName[propertyName].Contains(error))
            {
                _errorByPropertyName[propertyName].Add(error);
                OnErrorChanged(propertyName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void ClearErrors(string propertyName)
        {
            if (_errorByPropertyName.ContainsKey(propertyName))
            {
                _errorByPropertyName.Remove(propertyName);
                OnErrorChanged(propertyName);
            }
        }
    }
}
