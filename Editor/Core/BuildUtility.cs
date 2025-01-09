using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;

namespace GameWarriors.BuildConfiguration.Editor.Core
{
    public static class BuildUtility
    {
        public static void AddScriptingDefineSymbols(BuildTargetGroup targetGroup, string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                return;
            }
            //string currentDefines = GetScriptingDefineSymbols(targetGroup);
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            currentDefines = Regex.Replace(currentDefines, symbol, "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, $"{currentDefines};{symbol}");

        }

        public static void RemoveScriptingDefineSymbols(BuildTargetGroup targetGroup, string symbol)
        {
            //string currentDefines = GetScriptingDefineSymbols(targetGroup);
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            currentDefines = Regex.Replace(currentDefines, symbol, "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, $"{currentDefines}");
        }

        public static string CleanScriptingDefineSymbols(BuildTargetGroup group, params string[] defines)
        {
            string curretnDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            string oldDefines = curretnDefines;
            int length = defines.Length;
            for (int i = 0; i < length; ++i)
            {
                string replacePattern = defines[i].ToUpper();
                curretnDefines = Regex.Replace(curretnDefines, replacePattern, string.Empty);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, curretnDefines);
            return oldDefines;
        }

        public static string FindFirstScriptingDefineSymbol(BuildTargetGroup group, params string[] defines)
        {
            string curretnDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            int length = defines.Length;
            for (int i = 0; i < length; ++i)
            {
                string define = defines[i].ToUpper();
                if (curretnDefines.Contains(define))
                    return define;
            }
            return null;
        }

        public static bool IsContainScriptingDefineSymbol(BuildTargetGroup group, string define)
        {
            string curretnDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            if (curretnDefines.Contains(define.ToUpper()))
                return true;
            return false;
        }

        public static void AddScriptingDefineSymbols(NamedBuildTarget buildTarget, string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                return;
            }
            //string currentDefines = GetScriptingDefineSymbols(targetGroup);
            string currentDefines = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
            currentDefines = Regex.Replace(currentDefines, symbol, "");
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, $"{currentDefines};{symbol}");

        }

        public static void RemoveScriptingDefineSymbols(NamedBuildTarget buildTarget, string symbol)
        {
            //string currentDefines = GetScriptingDefineSymbols(targetGroup);
            string currentDefines = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
            currentDefines = Regex.Replace(currentDefines, symbol, "");
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, $"{currentDefines}");
        }

        public static string CleanScriptingDefineSymbols(NamedBuildTarget buildTarget, params string[] defines)
        {
            string curretnDefines = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
            string oldDefines = curretnDefines;
            int length = defines.Length;
            for (int i = 0; i < length; ++i)
            {
                string replacePattern = defines[i].ToUpper();
                curretnDefines = Regex.Replace(curretnDefines, replacePattern, string.Empty);
            }
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, curretnDefines);
            return oldDefines;
        }

        public static string FindFirstScriptingDefineSymbol(NamedBuildTarget buildTarget, params string[] defines)
        {
            string curretnDefines = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
            int length = defines.Length;
            for (int i = 0; i < length; ++i)
            {
                string define = defines[i].ToUpper();
                if (curretnDefines.Contains(define))
                    return define;
            }
            return null;
        }

        public static bool IsContainScriptingDefineSymbol(NamedBuildTarget buildTarget, string define)
        {
            string curretnDefines = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
            if (curretnDefines.Contains(define.ToUpper()))
                return true;
            return false;
        }

        public static NamedBuildTarget GetCurrentNamedBuildTarget()
        {
            BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(activeBuildTarget);
            return NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
        }
    }
}