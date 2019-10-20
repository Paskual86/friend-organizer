namespace FriendOrganizer.UI.View.Services
{
    public interface IMessageDialogService
    {
        /// <summary>
        /// Shows the ok cancel dialog.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        MessageDialogResult ShowOkCancelDialog(string text, string title);

        /// <summary>
        /// Shows the information dialog.
        /// </summary>
        /// <param name="text">The text.</param>
        void ShowInfoDialog(string text);
    }
}