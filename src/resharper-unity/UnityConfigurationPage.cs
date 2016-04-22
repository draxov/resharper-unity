using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Resources;

namespace JetBrains.ReSharper.Plugins.Unity
{
    [OptionsPage(UNITY_CONFIGURATION_PAGE, "Unity Configuration", typeof(OptionsThemedIcons.Options))]
    public class UnityConfigurationPage : IOptionsPage
    {
        private const string UNITY_CONFIGURATION_PAGE = "UNITY_CONFIGURATION_PAGE";
        private readonly Lifetime m_lifetime;
        private readonly OptionsSettingsSmartContext m_settings;
        private readonly UnityConfigurationControl m_unityConfigurationControl;

        public bool OnOk()
        {
            return true;
        }

        public bool ValidatePage()
        {
            return true;
        }

        public EitherControl Control => m_unityConfigurationControl;

        public string Id => UNITY_CONFIGURATION_PAGE;

        public UnityConfigurationPage(Lifetime lifetime, OptionsSettingsSmartContext settings)
        {
            m_unityConfigurationControl = new UnityConfigurationControl();
            m_lifetime = lifetime;
            m_settings = settings;
            m_settings.SetBinding(lifetime, (UnityConfigurationSettings s) => s.ObjectCollection, m_unityConfigurationControl.UnityClassesListBox, ItemsControl.ItemsSourceProperty);
        }
    }

    [SettingsKey(typeof(Missing), "Unity Settings")]
    public class UnityConfigurationSettings
    {
        [SettingsEntry(null, "Collection of Unity Classes")]
        public IEnumerable ObjectCollection { get; set; }
    }
}
