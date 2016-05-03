using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using JetBrains.Application;
using JetBrains.Application.Interop.NativeHook;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store;
using JetBrains.CommonControls;
using JetBrains.DataFlow;
using JetBrains.Threading;
using JetBrains.UI.Application;
using JetBrains.UI.Controls;
using JetBrains.UI.Options;
using JetBrains.UI.Options.Helpers;
using JetBrains.UI.Resources;
using JetBrains.UI.Validation;

namespace JetBrains.ReSharper.Plugins.Unity
{
    [OptionsPage(PID, "Unity Configuration", typeof(OptionsThemedIcons.Options))]
    public class UnityOptionsPage : AStackPanelOptionsPage
    {
        public const string PID = "UnityOptionsPage";
        private const int MARGIN = 10;
        private readonly FormValidators m_formValidators;
        private readonly Lifetime m_lifetime;
        private readonly IMainWindow m_mainWindow;
        private readonly OptionsSettingsSmartContext m_settings;
        private readonly IWindowsHookManager m_windowsHookManager;
        private StringCollectionEdit m_unityClassesCollectionEdit;
        private StringCollectionEdit m_unityAttributesCollectionEdit;

        public UnityOptionsPage(
            IUIApplication environment,
            OptionsSettingsSmartContext settings,
            Lifetime lifetime,
            IShellLocks shellLocks,
            IWindowsHookManager windowsHookManager,
            FormValidators formValidators,
            IMainWindow mainWindow = null)
            : base(lifetime, environment, PID)
        {
            m_settings = settings;
            m_lifetime = lifetime;
            m_windowsHookManager = windowsHookManager;
            m_formValidators = formValidators;
            m_mainWindow = mainWindow;

            InitControls();
            shellLocks.QueueRecurring(lifetime, "Force settings merge", TimeSpan.FromMilliseconds(300.0), () => OnOk());
        }

        private void InitControls()
        {
            using (new LayoutSuspender(this))
            {
                TableLayoutPanel tablePanel = new TableLayoutPanel
                {
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Margin = Padding.Empty,
                    Padding = Padding.Empty,
                    Size = ClientSize
                };
                tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
                tablePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
                tablePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
                Controls.Add(tablePanel);

                GroupingEvent sizeEvent = Environment.Threading.GroupingEvents[Rgc.Invariant].CreateEvent(
                    m_lifetime,
                    PID + ".SizeChanged",
                    TimeSpan.FromMilliseconds(300.0), () =>
                    {
                        Size clientSize = new Size(ClientSize.Width - MARGIN, ClientSize.Height - MARGIN);
                        if (!clientSize.Equals(tablePanel.Size))
                        {
                            using (new LayoutSuspender(this))
                            {
                                tablePanel.Size = clientSize;
                            }
                        }
                    });
                EventHandler handler = (sender, args) => sizeEvent.FireIncoming();
                m_lifetime.AddBracket(() => SizeChanged += handler, () => SizeChanged -= handler);

                string titleCaption = "Specify Unity Classes" +
                                      System.Environment.NewLine;
                Controls.Label titleLabel = new Controls.Label(titleCaption) {AutoSize = true, Dock = DockStyle.Top};
                tablePanel.Controls.Add(titleLabel);

                string[] unityClasses = m_settings.EnumIndexedValues(UnitySettingsAccessor.UnityClasses).ToArray();
                m_unityClassesCollectionEdit = new StringCollectionEdit(Environment,
                    "Unity Classes:", null, m_mainWindow, m_windowsHookManager, m_formValidators)
                {
                    Dock = DockStyle.Fill
                };
                m_unityClassesCollectionEdit.Items.Value = unityClasses;
                tablePanel.Controls.Add(m_unityClassesCollectionEdit, 0, 1);

                string attributesTitleCaption = "Implicit Attributes" +
                                      System.Environment.NewLine;
                Controls.Label attributesTitleLabel = new Controls.Label(attributesTitleCaption) { AutoSize = true, Dock = DockStyle.Top };
                tablePanel.Controls.Add(attributesTitleLabel, 0, 2);

                string[] unityAttributes = m_settings.EnumIndexedValues(UnitySettingsAccessor.UnityAttributes).ToArray();
                m_unityAttributesCollectionEdit = new StringCollectionEdit(Environment,
                    "Unity Attributes:", null, m_mainWindow, m_windowsHookManager, m_formValidators)
                {
                    Dock = DockStyle.Fill
                };
                m_unityAttributesCollectionEdit.Items.Value = unityAttributes;
                tablePanel.Controls.Add(m_unityAttributesCollectionEdit, 0, 3);
            }
        }

        /// <summary>
        ///     Invoked when OK button in the options dialog is pressed.
        ///     If the page returns <c>false</c>, the the options dialog won't be closed, and focus will be put into this page.
        /// </summary>
        public override bool OnOk()
        {
            Expression<Func<UnitySettings, IIndexedEntry<string, string>>> unityClasses = key => key.UnityClasses;
            Expression<Func<UnitySettings, IIndexedEntry<string, string>>> unityAttributes = key => key.UnityUsageAttributes;

            string[] addedUnityClasses = m_unityClassesCollectionEdit.Items.Value;
            HashSet<string> currentUnityClasses = new HashSet<string>();
            foreach (string currentUnityClass in m_settings.EnumEntryIndices(unityClasses))
            {
                if (!addedUnityClasses.Contains(currentUnityClass))
                {
                    m_settings.RemoveIndexedValue(unityClasses, currentUnityClass);
                }
                else
                {
                    currentUnityClasses.Add(currentUnityClass);
                }
            }
            foreach (string entryIndex in addedUnityClasses.Where(x => !currentUnityClasses.Contains(x)))
            {
                m_settings.SetIndexedValue(unityClasses, entryIndex, entryIndex);
            }

            string[] addedUnityAttributes = m_unityAttributesCollectionEdit.Items.Value;
            HashSet<string> currentUnityAttributes = new HashSet<string>();
            foreach (string currentUnityAttribute in m_settings.EnumEntryIndices(unityAttributes))
            {
                if (!addedUnityAttributes.Contains(currentUnityAttribute))
                {
                    m_settings.RemoveIndexedValue(unityAttributes, currentUnityAttribute);
                }
                else
                {
                    currentUnityAttributes.Add(currentUnityAttribute);
                }
            }
            foreach (string entryIndex in addedUnityAttributes.Where(x => !currentUnityAttributes.Contains(x)))
            {
                m_settings.SetIndexedValue(unityAttributes, entryIndex, entryIndex);
            }

            return base.OnOk();
        }
    }
}