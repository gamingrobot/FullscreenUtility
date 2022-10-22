namespace FullscreenUtility
{
    public static class SettingsState
    {
        public static bool CursorLockEnabled { get; set; }
        public static bool FullscreenAppFocused { get; set; } //Only enable mouse transparency when fullscreen app is focused
        public static bool MouseTransparencyEnabled { get; set; }
    }
}
