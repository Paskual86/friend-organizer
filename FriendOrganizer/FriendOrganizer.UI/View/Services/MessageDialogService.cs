using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.View.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        // Obtengo la instancia de la pantalla principal
        private MetroWindow MetroWindow => (MetroWindow)App.Current.MainWindow;
        /// <summary>
        /// Shows the information dialog.
        /// </summary>
        /// <param name="text">The text.</param>
        public async Task ShowInfoDialogAsync(string text)
        {
            await MetroWindow.ShowMessageAsync("Info", text, MessageDialogStyle.Affirmative);
        }

        /// <summary>
        /// Shows the ok cancel dialog.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public async Task<MessageDialogResult> ShowOkCancelDialogAsync(string text, string title)
        {
            
            var result = await MetroWindow.ShowMessageAsync(title, text, MessageDialogStyle.AffirmativeAndNegative);

            return result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative
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
