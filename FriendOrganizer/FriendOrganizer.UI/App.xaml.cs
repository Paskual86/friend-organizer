using Autofac;
using FriendOrganizer.UI.Startup;
using System.Windows;

namespace FriendOrganizer.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var bo = new Bootstrapper();
            var container = bo.Bootstrap();
            var main = container.Resolve<MainWindow>();
            main.Show();
        }
    }
}
