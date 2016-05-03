using System;
using System.Linq.Expressions;
using JetBrains.Application.Settings.Store;

namespace JetBrains.ReSharper.Plugins.Unity
{
    public class UnitySettingsAccessor
    {
        public static readonly Expression<Func<UnitySettings, IIndexedEntry<string, string>>> s_unityClasses = key => key.UnityClasses;
        public static readonly Expression<Func<UnitySettings, IIndexedEntry<string, string>>> s_unityAttributes = key => key.UnityUsageAttributes;

        public static Expression<Func<UnitySettings, IIndexedEntry<string, string>>> UnityClasses => s_unityClasses;
        public static Expression<Func<UnitySettings, IIndexedEntry<string, string>>> UnityAttributes => s_unityAttributes;
    }
}