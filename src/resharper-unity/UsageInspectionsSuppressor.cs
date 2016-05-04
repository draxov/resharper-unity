using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ReSharper.Plugins.Unity
{
    [ShellComponent]
    public class UsageInspectionsSuppressor : IUsageInspectionsSuppressor
    {
        private readonly UnitySolutionHelper m_unitySolutionHelper;

        public UsageInspectionsSuppressor(UnitySolutionHelper unitySolutionHelper)
        {
            m_unitySolutionHelper = unitySolutionHelper;
        }

        public bool SuppressUsageInspectionsOnElement(IDeclaredElement element, out ImplicitUseKindFlags flags)
        {
            ISolution iSolution = element.GetSolution();
            IList<IAssembly> allAssemblies = iSolution.GetAllAssemblies();
            bool referencesUnity =
                allAssemblies.Any(
                    assembly => assembly.Name == "UnityEngine" || assembly.Name == "UnityEditor");
            if (!referencesUnity)
            {
                flags = ImplicitUseKindFlags.Default;
                return false;
            }
            IClass cls = element as IClass;
            if (cls != null)
            {
                if (m_unitySolutionHelper.IsUnityImplicitType(cls, cls.Module))
                {
                    flags = ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature;
                    return true;
                }
            }

            IMethod method = element as IMethod;
            if (method != null && MonoBehaviourUtil.IsEventHandler(method.ShortName))
            {
                ITypeElement containingType = method.GetContainingType();
                if (containingType != null && m_unitySolutionHelper.IsUnityImplicitType(containingType, method.Module))
                {
                    flags = ImplicitUseKindFlags.Access;
                    return true;
                }
            }

            IField field = element as IField;
            if (field != null)
            {
                if (m_unitySolutionHelper.CheckFieldForUnityImplicits(field, field.Module))
                {
                    // Public fields gets exposed to the Unity Editor and assigned from the UI. But it still should be checked if the field is ever accessed from the code.
                    flags = ImplicitUseKindFlags.Assign;
                    return true;
                }
            }

            flags = ImplicitUseKindFlags.Default;
            return false;
        }
    }
}
