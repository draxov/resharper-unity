using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store.Implementation;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Unity
{
    [ShellComponent]
    public class DefaultUnitySettings : HaveDefaultSettings
    {
        public override string Name => "Default Unity Settings";
        public DefaultUnitySettings(ILogger logger, ISettingsSchema settingsSchema)
            : base(logger, settingsSchema)
        {
        }

        public override void InitDefaultSettings(ISettingsStorageMountPoint mountPoint)
        {
            SetIndexedValue(mountPoint, UnitySettingsAccessor.UnityClasses, "UnityEngine.MonoBehaviour");
            SetIndexedValue(mountPoint, UnitySettingsAccessor.UnityClasses, "UnityEngine.ScriptableObject");
            SetIndexedValue(mountPoint, UnitySettingsAccessor.UnityAttributes, "UnityEngine.SerializeField");
        }
    }
}