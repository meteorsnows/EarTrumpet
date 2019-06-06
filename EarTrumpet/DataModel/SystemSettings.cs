using EarTrumpet.Extensions;
using Microsoft.Win32;
using System;
using System.Globalization;

namespace EarTrumpet.DataModel
{
    public static class SystemSettings
    {
        static readonly string s_PersonalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        public static bool IsTransparencyEnabled => ReadDword(s_PersonalizeKey, "EnableTransparency");
        public static bool UseAccentColor => ReadDword(s_PersonalizeKey, "ColorPrevalence");
        public static bool IsLightTheme => ReadDword(s_PersonalizeKey, "AppsUseLightTheme", true);
        public static bool IsSystemLightTheme => LightThemeShim(ReadDword(s_PersonalizeKey, "SystemUsesLightTheme"));
        public static bool UseDynamicScrollbars => ReadDword(@"Control Panel\Accessibility", "DynamicScrollbars", true);
        public static bool UseAccentColorOnWindowBorders => ReadDword(@"Software\Microsoft\Windows\DWM", "ColorPrevalence");
        public static bool IsRTL => CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

        public static string BuildLabel
        {
            get
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var subKey = baseKey.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion"))
                {
                    return (string)subKey?.GetValue("BuildLabEx");
                }
            }
        }

        private static bool ReadDword(string key, string valueName, bool defaultValue = false)
        {
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            using (var subKey = baseKey.OpenSubKey(key))
            {
                bool readValue = defaultValue;
                if (subKey != null)
                {
                    readValue = subKey.GetValue<int>(valueName, defaultValue ? 1 : 0) > 0;
                }
                return readValue;
            }
        }

        private static bool LightThemeShim(bool registryValue)
        {
            if (Environment.OSVersion.IsGreaterThan(OSVersions.RS5_1809))
            {
                return registryValue;
            }
            return false; // No system theme prior to 19H1/RS6.
        }
    }
}
