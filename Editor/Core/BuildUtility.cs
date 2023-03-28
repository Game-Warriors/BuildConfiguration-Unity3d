using GameWarriors.BuildConfiguration.Editor.Data;
using System;
using System.Text.RegularExpressions;
using UnityEditor;

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
    }
}