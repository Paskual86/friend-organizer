using System.Windows;

namespace FriendOrganizer.UI.View.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        /// <summary>
        /// Shows the information dialog.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ShowInfoDialog(string text)
        {
            MessageBox.Show(text);
        }

        /// <summary>
        /// Shows the ok cancel dialog.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public MessageDialogResult ShowOkCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);

            return result == MessageBoxResult.OK
                ? MessageDialogResult.OK
                : MessageDialogResult.Cancel;
        }

        
    }
    public enum MessageDialogResult
    {
        OK,
        Cancel
    }
}
