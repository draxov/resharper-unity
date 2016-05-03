using System.Reflection;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store;

namespace JetBrains.ReSharper.Plugins.Unity
{
    [SettingsKey(typeof(Missing), "Unity Settings")]
    public class UnitySettings
    {
        [SettingsIndexedEntry("Collection of Unity Classes")]
        public IIndexedEntry<string, string> UnityClasses { get; set; }
        [SettingsIndexedEntry("Collection of Attributes for Implicity Usages")]
        public IIndexedEntry<string, string> UnityUsageAttributes { get; set; }
    }
}
