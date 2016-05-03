using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.StructuralSearch.Patterns;
using JetBrains.ReSharper.Feature.Services.StructuralSearch.Settings;
using JetBrains.ReSharper.Features.StructuralSearch.UI.Daemon;
using JetBrains.UI.Options;
using JetBrains.UI.Options.OptionsDialog2.SimpleOptions;
using JetBrains.UI.Wpf;

namespace JetBrains.ReSharper.Plugins.Unity
{
    public class UnityCustomOptionsPage : CustomSimpleOptionsPage
    {
        public const string PID = "CustomPatterns";
        private readonly SettingsIndexedKey myUnityKey;
        private readonly ObservableCollection<UnityClassItem> myCustomUnityItems;

        public UnityCustomOptionsPage(Lifetime lifetime, OptionsSettingsSmartContext optionsSettingsSmartContext)
            : base(lifetime, optionsSettingsSmartContext)
        {
            myUnityKey = (SettingsIndexedKey)optionsSettingsSmartContext.Schema.GetKey<UnityCustomSettings>();
            myCustomUnityItems = new ObservableCollection<UnityClassItem>();
            foreach (GuidIndex id in ((IContextBoundSettingsStore)OptionsSettingsSmartContext).
                EnumIndexedKey(myUnityKey, null))
            {
                Dictionary<SettingsKey, object> unityKeyIndices = new Dictionary<SettingsKey, object>
                {
                  {
                    myUnityKey,
                    id
                  }
                };
                if (OptionsSettingsSmartContext.IsIndexedKeyDefined(myUnityKey, unityKeyIndices))
                {
                    string unityClass = UnitySettingsUtil.ReadCustomPattern(optionsSettingsSmartContext, unityKeyIndices);
                    if (unityClass != null)
                    {
                        myCustomUnityItems.Add(new UnityClassItem(id, unityClass, this.OptionsSettingsSmartContext));
                    }
                }
            }
            lifetime.AddBracket((Action)(() => this.myCustomUnityItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCustomPatternItemsCollectionChanged)), (Action)(() => this.myCustomUnityItems.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnCustomPatternItemsCollectionChanged)));
            this.AddCustomOption((IAutomation)new CustomPatternsViewModel2(locks, engine, psiProjectFileTypeCoordinator, mainWindow, windowBranding, formValidators, windowsHookManager, documentMarkupManager, solution, this.OptionsSettingsSmartContext, documentManager, structuralSearchActionManager, searchDomainFactory, languages, this.myCustomUnityItems));
        }
    }

    public class UnitySettingsUtil
    {
        public static string ReadCustomPattern(OptionsSettingsSmartContext optionsSettingsSmartContext, Dictionary<SettingsKey, object> unityKeyIndices)
        {
            throw new NotImplementedException();
        }
    }

    public class UnityCustomSettings
    {
    }

    internal class UnityClassItem
    {
        public UnityClassItem(GuidIndex id, string unityClass, OptionsSettingsSmartContext optionsSettingsSmartContext)
        {
            throw new NotImplementedException();
        }
    }
}
