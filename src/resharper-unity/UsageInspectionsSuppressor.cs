using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ReSharper.Plugins.Unity
{
    [ShellComponent]
    public class UsageInspectionsSuppressor : IUsageInspectionsSuppressor
    {
        private readonly UnitySolution m_unitySolution;

        public UsageInspectionsSuppressor(UnitySolution unitySolution)
        {
            m_unitySolution = unitySolution;
        }

        public bool SuppressUsageInspectionsOnElement(IDeclaredElement element, out ImplicitUseKindFlags flags)
        {
            // TODO: Only do any work if the element belongs to a project that references Unity.Engine
            var cls = element as IClass;
            if (cls != null)
            {
                if(m_unitySolution.IsUnityImplicitType(cls, cls.Module))
                {
                    flags = ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature;
                    return true;
                }
            }

            var method = element as IMethod;
            if (method != null && MonoBehaviourUtil.IsEventHandler(method.ShortName))
            {
                var containingType = method.GetContainingType();
                if (containingType != null && m_unitySolution.IsUnityImplicitType(containingType, method.Module))
                {
                    flags = ImplicitUseKindFlags.Access;
                    return true;
                }
            }

            var field = element as IField;
            if (field != null)
            {
                if (m_unitySolution.CheckFieldForUnityImplicits(field, field.Module))
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
