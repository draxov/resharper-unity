using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace JetBrains.ReSharper.Plugins.Unity
{
    [ShellComponent]
    public class UnitySolutionHelper
    {
        private readonly UnitySettings m_unitySettings;

        public UnitySolutionHelper(ISettingsStore settingsStore)
        {
            IContextBoundSettingsStore boundSettings = settingsStore.BindToContextTransient(ContextRange.ApplicationWide);
            m_unitySettings = boundSettings.GetKey<UnitySettings>(SettingsOptimization.DoMeSlowly);
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