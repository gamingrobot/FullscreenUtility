using System;
using System.Diagnostics;
using System.Text;
using NLog;

namespace FullscreenUtility
{
    public class MouseTransparencyWatcher
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly System.Threading.Timer _checkTimer;

        public MouseTransparencyWatcher()
        {
            _checkTimer = new System.Threading.Timer(CheckTask,
                null,
                0,
                500);
        }

        private static void CheckTask(object rawState)
        {
            var windowName = "Picture-in-Picture"; //Firefox picture in picture
            var windowHandle = IntPtr.Zero;
            NativeMethods.EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                var windowText = GetWindowText(wnd);
                //Logger.Trace(() => $"WindowText: {windowText}");
                if (windowText == windowName)
                {
                    windowHandle = wnd;
                    return false;
                }

                // return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            if (windowHandle == IntPtr.Zero)
            {
                return;
            }

            NativeMethods.GetWindowThreadProcessId(windowHandle, out var processId);
            var processName = Process.GetProcessById((int)processId).ProcessName;
            Logger.Trace($"Got {windowName} window in {processName}");

            var extendedStyle = NativeMethods.GetWindowLong(windowHandle, NativeMethods.GWL_EXSTYLE);
            if (extendedStyle == 0)
            {
                Logger.Error($"Failed getting extendedStyle for {processName}");
                return;
            }

            //Already set
            if ((extendedStyle & NativeMethods.WS_EX_TRANSPARENT) == NativeMethods.WS_EX_TRANSPARENT
                && (extendedStyle & NativeMethods.WS_EX_LAYERED) == NativeMethods.WS_EX_LAYERED)
            {
                //Disable if either setting is disabled
                if (!SettingsState.MouseTransparencyEnabled || !SettingsState.CursorCurrentlyLocked)
                {
                    Logger.Info($"Un-setting mouse transparency on {processName}:{windowName}");
                    var result = NativeMethods.SetWindowLong(windowHandle, NativeMethods.GWL_EXSTYLE,
                        extendedStyle & ~NativeMethods.WS_EX_TRANSPARENT & ~NativeMethods.WS_EX_LAYERED);
                    if (result == 0)
                    {
                        Logger.Error($"Failed un-setting extendedStyle for {processName}");
                    }
                }
            }
            else //Not set
            {
                //Enable if both are set
                if (SettingsState.MouseTransparencyEnabled && SettingsState.CursorCurrentlyLocked)
                {
                    Logger.Info($"Setting mouse transparency on {processName}:{windowName}");
                    var result = NativeMethods.SetWindowLong(windowHandle, NativeMethods.GWL_EXSTYLE,
                        extendedStyle | NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_LAYERED);
                    if (result == 0)
                    {
                        Logger.Error($"Failed setting extendedStyle for {processName}");
                    }
                }
            }
        }

        private static string GetWindowText(IntPtr hWnd)
        {
            var size = NativeMethods.GetWindowTextLength(hWnd);
            if (size <= 0)
            {
                return string.Empty;
            }
            var builder = new StringBuilder(size + 1);
            NativeMethods.GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }
    }
}
