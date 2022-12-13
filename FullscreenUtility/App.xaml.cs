using System;
using System.Threading;
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

        private static readonly Mutex SingleInstanceMutex = new Mutex(true, "{554f2f38-7358-4d9c-8b2b-43ae219d3887}");

        private readonly TaskbarIcon _trayIcon;
        private readonly Settings _settings;

        private CursorLockWatcher _cursorLockWatcher;
        private MouseTransparencyWatcher _mouseTransparencyWatcher;

        public App()
        {
            InitializeComponent();

            _trayIcon = (TaskbarIcon)FindResource("TrayIcon");

            _settings = SettingsFile.ReadSettings();
            SettingsState.CursorLockEnabled = _settings.CursorLockEnabled;
            SettingsState.MouseTransparencyEnabled = _settings.MouseTransparencyEnabled;
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            if (SingleInstanceMutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    _cursorLockWatcher = new CursorLockWatcher();
                    _mouseTransparencyWatcher = new MouseTransparencyWatcher();
                    base.OnStartup(e);
                }
                finally
                {
                    SingleInstanceMutex.ReleaseMutex();
                }
            }
            else
            {
                MessageBox.Show("FullscreenUtility is already running!");
                Application.Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _settings.CursorLockEnabled = SettingsState.CursorLockEnabled;
            _settings.MouseTransparencyEnabled = SettingsState.MouseTransparencyEnabled;
            SettingsFile.WriteSettings(_settings);

            _trayIcon.Dispose();
            base.OnExit(e);
        }

        private void ExitOnClick(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void CursorLockOnClick(object sender, RoutedEventArgs e)
        {
            SettingsState.CursorLockEnabled = !SettingsState.CursorLockEnabled;
            UpdateCursorLockText((MenuItem) e.OriginalSource);
        }

        private void CursorLockOnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateCursorLockText((MenuItem)e.OriginalSource);
        }

        private void UpdateCursorLockText(MenuItem item)
        {
            item.Header = GetStatusText(SettingsState.CursorLockEnabled, "Cursor Lock");
        }

        private void MouseTransparencyOnClick(object sender, RoutedEventArgs e)
        {
            SettingsState.MouseTransparencyEnabled = !SettingsState.MouseTransparencyEnabled;
            UpdateMouseTransparencyText((MenuItem)e.OriginalSource);
        }

        private void MouseTransparencyOnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateMouseTransparencyText((MenuItem)e.OriginalSource);
        }

        private void UpdateMouseTransparencyText(MenuItem item)
        {
            item.Header = GetStatusText(SettingsState.MouseTransparencyEnabled, "Mouse Transparency");
        }

        private static string GetStatusText(bool state, string tail)
        {
            var statusText = state ? "off" : "on";
            return $"Turn {statusText} {tail}";
        }



    }
}
