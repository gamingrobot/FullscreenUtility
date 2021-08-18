using System;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace FullscreenUtility
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly TaskbarIcon _trayIcon;

        public App()
        {
            InitializeComponent();

            _trayIcon = (TaskbarIcon)FindResource("TrayIcon");
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void ExitOnClick(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void ToggleCursorLock(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ToggleMouseTransparency(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
