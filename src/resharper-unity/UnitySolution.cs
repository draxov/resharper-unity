using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace JetBrains.ReSharper.Plugins.Unity
{
    [ShellComponent]
    public class UnitySolution
    {
        private readonly SolutionsManager m_solutionsManager;
        private readonly UnitySettings m_unitySettings;
        private readonly SettingsKey m_settingsKey;
        private Lifetime m_lifetime;

        public UnitySolution(ISettingsStore settingsStore, Lifetime lifetime, SolutionsManager solutionsManager)
        {
            m_solutionsManager = solutionsManager;
            m_lifetime = lifetime;
            IContextBoundSettingsStore boundSettings = settingsStore.BindToContextTransient(ContextRange.ApplicationWide);
            m_unitySettings = boundSettings.GetKey<UnitySettings>(SettingsOptimization.DoMeSlowly);
            m_settingsKey = boundSettings.Schema.GetKey<UnitySettings>();
            settingsStore.Changed.Advise(lifetime, SettingsChanged);
        }

        private void SettingsChanged(SettingsStoreChangeArgs settingsStoreChangeArgs)
        {
            bool hasChanged = settingsStoreChangeArgs.ChangedKeys.Contains(m_settingsKey);
            if (!hasChanged)
            {
                return;
            }
            ISolution iSolution = m_solutionsManager.Solution;
            if (null == iSolution)
            {
                return;
            }
        }

        public bool IsUnityImplicitType([NotNull] ITypeElement typeElement, [NotNull] IPsiModule module)
        {
            foreach (KeyValuePair<string, string> pair in m_unitySettings.UnityClasses.EnumIndexedValues())
            {
                IDeclaredType typeByClrName = TypeFactory.CreateTypeByCLRName(pair.Value, module);
                ITypeElement unityClass = typeByClrName.GetTypeElement();
                if (typeElement.IsDescendantOf(unityClass))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsUnityImplicitType([NotNull] IDeclaredType declaredType, [NotNull] IPsiModule module)
        {
            foreach (KeyValuePair<string, string> pair in m_unitySettings.UnityClasses.EnumIndexedValues())
            {
                IDeclaredType typeByClrName = TypeFactory.CreateTypeByCLRName(pair.Value, module);
                if (declaredType.IsSubtypeOf(typeByClrName))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckFieldForUnityImplicits([NotNull] IField field, [NotNull] IPsiModule module)
        {
            ITypeElement containingType = field.GetContainingType();
            if (containingType == null)
            {
                return false;
            }
            bool serializable = containingType.HasAttributeInstance(new ClrTypeName("System.SerializableAttribute"), true);
            bool unityObject = containingType.GetSuperTypes().Any(t => IsUnityImplicitType(t, module));
            if (!serializable && !unityObject)
            {
                return false;
            }
            if (field.GetAccessRights() == AccessRights.PUBLIC)
            {
                return true;
            }
            foreach (KeyValuePair<string, string> pair in m_unitySettings.UnityUsageAttributes.EnumIndexedValues())
            {
                bool hasAtttribute = field.HasAttributeInstance(new ClrTypeName(pair.Value), true);
                if (hasAtttribute)
                {
                    return true;
                }
            }
            return false;
        }
    }
}