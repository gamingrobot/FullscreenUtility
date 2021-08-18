namespace FullscreenUtility
{
    public static class SettingsState
    {
        public static bool CursorLockEnabled { get; set; }
        public static bool CursorCurrentlyLocked { get; set; } //Only enable mouse transparency when cursor is locked
        public static bool MouseTransparencyEnabled { get; set; }
    }
}
