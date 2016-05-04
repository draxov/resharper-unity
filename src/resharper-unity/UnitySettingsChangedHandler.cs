using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Threading;

namespace JetBrains.ReSharper.Plugins.Unity
{
    [SolutionComponent]
    public class UnitySettingsChangedHandler
    {
        public UnitySettingsChangedHandler(Lifetime lifetime, ISolution solution, IThreading threading, ISettingsStore settingsStore)
        {
            settingsStore.AdviseChange(lifetime, settingsStore.Schema.GetKey<UnitySettings>(), () =>
            {
                if (threading.Dispatcher.IsAsyncBehaviorProhibited)
                {
                    InvalidateDaemon(solution);
                }
                else
                {
                    threading.ReentrancyGuard.ExecuteOrQueue("UnitySettingsChangedHandler", () => 
                        InvalidateDaemon(solution));
                }
            });
        }

        private static void InvalidateDaemon(ISolution solution)
        {
            IDaemon component = solution.TryGetComponent<IDaemon>();
            component?.Invalidate();
        }
    }
}