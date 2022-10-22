using System;
using System.Diagnostics;
using NLog;

namespace FullscreenUtility
{
    public class CursorLockWatcher
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // ReSharper disable once NotAccessedField.Local
        private readonly System.Threading.Timer _checkTimer;

        public CursorLockWatcher()
        {
            _checkTimer = new System.Threading.Timer(CheckTask,
                null,
                0,
                500);
        }

        private static void CheckTask(object state)
        {
            //Cleaned up version of https://github.com/blaberry/FullscreenLock/blob/master/FullscreenLock/Checker.cs
            var desktopHandle = NativeMethods.GetDesktopWindow();
            NativeMethods.GetWindowRect(desktopHandle, out var desktopBounds);

            var shellHandle = NativeMethods.GetShellWindow();
            var fgHandle = NativeMethods.GetForegroundWindow();

            if (!fgHandle.Equals(IntPtr.Zero) && !(fgHandle.Equals(desktopHandle) || fgHandle.Equals(shellHandle)))
            {
                NativeMethods.GetWindowRect(fgHandle, out var appBounds);
                NativeMethods.GetWindowThreadProcessId(fgHandle, out var processId);
                var processName = Process.GetProcessById((int)processId).ProcessName;

                var appBoundsHeight = appBounds.Bottom - appBounds.Top;
                var appBoundsWidth = appBounds.Right - appBounds.Left;
                var desktopBoundsHeight = desktopBounds.Bottom - desktopBounds.Top;
                var desktopBoundsWidth = desktopBounds.Right - desktopBounds.Left;

                Logger.Trace($"Forground Process {processName}");
                Logger.Trace($"AppBounds {appBoundsHeight} {appBoundsWidth}");
                Logger.Trace($"DesktopBounds {desktopBoundsHeight} {desktopBoundsWidth}");
                if (appBoundsHeight == desktopBoundsHeight && appBoundsWidth == desktopBoundsWidth)
                {
                    SettingsState.FullscreenAppFocused = true;
                    if (SettingsState.CursorLockEnabled)
                    {
                        Logger.Info($"Restricting cursor on {processName}");
                        NativeMethods.ClipCursor(ref appBounds);
                    }
                }
                else
                {
                    SettingsState.FullscreenAppFocused = false;
                    if (SettingsState.CursorLockEnabled)
                    {
                        NativeMethods.ClipCursor(IntPtr.Zero);
                    }
                }
            }
        }
    }
}