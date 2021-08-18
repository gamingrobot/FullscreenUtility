using System;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using NLog;

namespace FullscreenUtility
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly TaskbarIcon _trayIcon;

        private CursorLockWatcher _cursorLockWatcher;
        private MouseTransparencyWatcher _mouseTransparencyWatcher;

        public App()
        {
            InitializeComponent();

            _trayIcon = (TaskbarIcon)FindResource("TrayIcon");

            SettingsState.MouseTransparencyEnabled = true;
            SettingsState.CursorLockEnabled = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _cursorLockWatcher = new CursorLockWatcher();
            _mouseTransparencyWatcher = new MouseTransparencyWatcher();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayIcon.Dispose();
            base.OnExit(e);
        }

        private void ExitOnClick(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void CursorLockOnClick(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem) e.OriginalSource;
            SettingsState.CursorLockEnabled = !SettingsState.CursorLockEnabled;
            item.Header = GetStatusText(SettingsState.CursorLockEnabled, "Cursor Lock");
        }

        private void MouseTransparencyOnClick(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)e.OriginalSource;
            SettingsState.MouseTransparencyEnabled = !SettingsState.MouseTransparencyEnabled;
            item.Header = GetStatusText(SettingsState.MouseTransparencyEnabled, "Mouse Transparency");
        }

        private string GetStatusText(bool state, string tail)
        {
            var statusText = state ? "off" : "on";
            return $"Turn {statusText} {tail}";
        }


    }
}
