using System.Windows;

namespace WeddingApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainView = new MainWindow();
            mainView.Show();  // I've read, do this first, because of a .NET 4.5 issue
            mainView.DataContext = Control.Init();
        }
    }
}
