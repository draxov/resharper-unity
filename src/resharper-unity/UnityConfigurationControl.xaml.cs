using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Resources;

namespace JetBrains.ReSharper.Plugins.Unity
{
    /// <summary>
    /// Interaction logic for UnityConfigurationControl.xaml
    /// </summary>
    public partial class UnityConfigurationControl : IOptionsPage
    {
        public static readonly DependencyProperty UnityClassesProperty = 
            DependencyProperty.Register(
                "UnityClasses", typeof(ObservableCollection<string>), typeof(UnityConfigurationControl));
        private readonly OptionsSettingsSmartContext m_settings;
        private const string UNITY_CONFIGURATION_PAGE = "UNITY_CONFIGURATION_PAGE";
        
        public EitherControl Control => this;
        public string Id => UNITY_CONFIGURATION_PAGE;

        public ObservableCollection<string> UnityClasses
        {
            get { return (ObservableCollection<string>)GetValue(UnityClassesProperty); }
            set { SetValue(UnityClassesProperty, value); }
        }

        public UnityConfigurationControl(Lifetime lifetime, OptionsSettingsSmartContext settings)
        {
            m_settings = settings;
            InitializeComponent();
//            Expression<Func<UnitySettings, ObservableCollection<string>>> expression = s => s.TestCollection;
//            settings.SetBinding(lifetime, expression, this, UnityClassesProperty);
            SetValue(UnityClassesProperty, new ObservableCollection<string>());
            DataContext = GetValue(UnityClassesProperty);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<string> value = (ObservableCollection<string>) GetValue(UnityClassesProperty);
            value.Add("New Class");
        }

        public bool OnOk()
        {
            return true;
        }

        public bool ValidatePage()
        {
            return true;
        }
    }
}
