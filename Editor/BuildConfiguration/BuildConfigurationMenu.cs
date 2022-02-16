using GameWrriors.EditorTools.BuildConfiguration.Data;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
#if ADMOB
using GoogleMobileAds.Editor;

#endif

namespace GameWrriors.EditorTools.BuildConfiguration
{
    public class BuildConfigurationMenu : ScriptableWizard
    {
        private const string ASSET_PATH = "Assets/AssetData/BuildConfiguration/MainConfig.asset";
        private const string GAME_ANALYTIC_DEFINE = "GAME_ANALYTICS";
        private const string FIRE_BASE_DEFINE = "FIREBASE";
        private const string METRIX_DEFINE = "METRIX";
        private const string APPS_FLYER_DEFINE = "APPS_FLYER";
        private const string ADMOB_DEFINE = "ADMOB";
        private const string TAPSELL_DEFINE = "TAPSELL";
        private const string APPMETRICA_DEFINE = "APPMETRICA";
        private const string APPMETRICA_LOCATION_DEFINE = "APP_METRICA_TRACK_LOCATION_DISABLED";
        [SerializeField] private EBuildType _buildType;
        [SerializeField] private EMarketType _marketType;
        [SerializeField] private bool _isUseGameAnalytic;
        [SerializeField] private bool _isUseFirebase;
        [SerializeField] private bool _isUseMetrix;
        [SerializeField] private bool _isUseAdmob;
        [SerializeField] private bool _isUseTapsell;
        [SerializeField] private bool _isUseAppMetrica;
        [SerializeField] private bool _disableAppMetricaLocation;
        [SerializeField] private bool _isUseAppsFlyer;


        [InitializeOnLoadMethod]
        private static void OnUnityStartUp()
        {
            Directory.CreateDirectory("Assets/AssetData/BuildConfiguration");
            BuildConfigurationMenu tmp = CreateInstance<BuildConfigurationMenu>();
            tmp.Initialization(true);
            DestroyImmediate(tmp);
        }

        [MenuItem("Tools/Build Configuration")]
        private static void OpenBuildConfigWindow()
        {
            BuildConfigurationMenu tmp = DisplayWizard<BuildConfigurationMenu>("Build Configuration", "Apply");
            tmp.Initialization();
        }

        private void Initialization(bool isRefresh = false)
        {
            BuildConfigMainObject mainAsset = AssetDatabase.LoadAssetAtPath<BuildConfigMainObject>(ASSET_PATH);
            if (mainAsset)
            {
                if (isRefresh)
                {
                    EBuildType buildType = mainAsset.BuildType;
                    EMarketType marketType = mainAsset.MarketType;
                    string buildTypeString = buildType.ToString().ToUpper();
                    string marketTypeString = marketType.ToString().ToUpper();

                    BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                    string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
                    if (!currentDefines.Contains(buildTypeString))
                    {
                        if (Enum.TryParse<EBuildType>(buildTypeString, out var type))
                            buildType = type;
                        else
                            buildType = EBuildType.NONE;
                    }

                    if (!currentDefines.Contains(marketTypeString))
                    {
                        if (Enum.TryParse<EMarketType>(marketTypeString, out var type))
                            marketType = type;
                        else
                            marketType = EMarketType.None;
                    }

                    _isUseGameAnalytic = currentDefines.Contains(GAME_ANALYTIC_DEFINE);
                    _isUseFirebase = currentDefines.Contains(FIRE_BASE_DEFINE);
                    _isUseAppsFlyer = currentDefines.Contains(APPS_FLYER_DEFINE);
                    _isUseMetrix = currentDefines.Contains(METRIX_DEFINE);
                    _isUseAdmob = currentDefines.Contains(ADMOB_DEFINE);
                    _isUseTapsell = currentDefines.Contains(TAPSELL_DEFINE);
                    _isUseAppMetrica = currentDefines.Contains(APPMETRICA_DEFINE);
                    _disableAppMetricaLocation = currentDefines.Contains(APPMETRICA_LOCATION_DEFINE);
                    UpdateMainAsset(ref mainAsset, buildType, marketType);
                }
                else
                {
                    _buildType = mainAsset.BuildType;
                    _marketType = mainAsset.MarketType;
                    _isUseAdmob = mainAsset.UseAdmob;
                    _isUseFirebase = mainAsset.UseFirebase;
                    _isUseGameAnalytic = mainAsset.UseGameAnalytic;
                    _isUseMetrix = mainAsset.UseMetrix;
                    _isUseTapsell = mainAsset.UseTapsell;
                    _isUseAppsFlyer = mainAsset.UseAppsFlyer;
                    _isUseAppMetrica = mainAsset.UseAppMetrica;
                    _disableAppMetricaLocation = mainAsset.DisableAppMetricaLocation;
                }
            }
        }

        private void OnWizardCreate()
        {
            BuildConfigMainObject mainAsset = AssetDatabase.LoadAssetAtPath<BuildConfigMainObject>(ASSET_PATH);
            if (mainAsset)
            {
                UpdateMainAsset(ref mainAsset, _buildType, _marketType);
                EditorUtility.SetDirty(mainAsset);
            }
            else
            {
                mainAsset = ScriptableObject.CreateInstance<BuildConfigMainObject>();
                UpdateMainAsset(ref mainAsset, _buildType, _marketType);
                //mainAsset.Initialization();
                AssetDatabase.CreateAsset(mainAsset, ASSET_PATH);
            }

            AssetDatabase.SaveAssets();
        }

        private void UpdateMainAsset(ref BuildConfigMainObject mainAsset, EBuildType buildType, EMarketType marketType)
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            ApplyNewBuildConfig(ref mainAsset, buildType, targetGroup);
            ApplyNewMarketType(ref mainAsset, marketType, targetGroup);
            UpdateAdPluginState(ref mainAsset, targetGroup);
            UpdateAnalyticPluginState(ref mainAsset, targetGroup);
            UpdateLunarConsoleState(buildType);
        }

        private void UpdateAnalyticPluginState(ref BuildConfigMainObject mainAsset, BuildTargetGroup targetGroup)
        {
            if (mainAsset.UseGameAnalytic != _isUseGameAnalytic)
            {
                mainAsset.UseGameAnalytic = _isUseGameAnalytic;
                SetGameAnalyticState(_isUseGameAnalytic, targetGroup);
            }

            if (mainAsset.UseAppsFlyer != _isUseAppsFlyer)
            {
                mainAsset.UseAppsFlyer = _isUseAppsFlyer;
                SetAppsFlyerState(_isUseAppsFlyer, targetGroup);
            }

            if (mainAsset.UseFirebase != _isUseFirebase)
            {
                mainAsset.UseFirebase = _isUseFirebase;
                SetFirebaseState(_isUseFirebase, targetGroup);
            }

            if (mainAsset.UseMetrix != _isUseMetrix)
            {
                mainAsset.UseMetrix = _isUseMetrix;
                SetMetrixticState(_isUseMetrix, targetGroup);
            }

            if (mainAsset.UseAppMetrica != _isUseAppMetrica ||
                mainAsset.DisableAppMetricaLocation != _disableAppMetricaLocation)
            {
                mainAsset.UseAppMetrica = _isUseAppMetrica;
                _disableAppMetricaLocation = _isUseAppMetrica & _disableAppMetricaLocation;
                mainAsset.DisableAppMetricaLocation = _disableAppMetricaLocation;
                SetAppMetricState(_isUseAppMetrica, _disableAppMetricaLocation, targetGroup);
            }
        }



        private void SetAppMetricState(bool isUseAppMetrica, bool disableAppMetricaLocation,
            BuildTargetGroup targetGroup)
        {
            UpdateScriptingDefineSymbolsForPlugin(disableAppMetricaLocation, APPMETRICA_LOCATION_DEFINE, targetGroup);
            UpdateScriptingDefineSymbolsForPlugin(isUseAppMetrica, APPMETRICA_DEFINE, targetGroup);
        }

        private void SetMetrixticState(bool isUse, BuildTargetGroup targetGroup)
        {
            UpdateScriptingDefineSymbolsForPlugin(isUse, METRIX_DEFINE, targetGroup);
        }

        private void SetFirebaseState(bool isUse, BuildTargetGroup targetGroup)
        {
            UpdateScriptingDefineSymbolsForPlugin(isUse, FIRE_BASE_DEFINE, targetGroup);
            if (targetGroup == BuildTargetGroup.Android)
            {
                PluginImporter tmp =
                    AssetImporter.GetAtPath("Assets/Plugins/Android/FirebaseApp.androidlib") as PluginImporter;
                tmp?.SetCompatibleWithPlatform(BuildTarget.Android, isUse);
                //tmp = AssetImporter.GetAtPath("Assets/Plugins/Android/firebase-analytics-unity-7.0.0.aar") as PluginImporter;
                //tmp?.SetCompatibleWithPlatform(BuildTarget.Android, isUse);
                //tmp = AssetImporter.GetAtPath("Assets/Plugins/Android/firebase-app-unity-7.0.0.aar") as PluginImporter;
                //tmp?.SetCompatibleWithPlatform(BuildTarget.Android, isUse);
                //tmp = AssetImporter.GetAtPath("Assets/Plugins/Android/firebase-messaging-unity-7.0.0.aar") as PluginImporter;
                //tmp?.SetCompatibleWithPlatform(BuildTarget.Android, isUse);
            }
        }

        private void SetAppsFlyerState(bool isUse, BuildTargetGroup targetGroup)
        {
            UpdateScriptingDefineSymbolsForPlugin(isUse, APPS_FLYER_DEFINE, targetGroup);

        }

        private void SetGameAnalyticState(bool isUse, BuildTargetGroup targetGroup)
        {
            UpdateScriptingDefineSymbolsForPlugin(isUse, GAME_ANALYTIC_DEFINE, targetGroup);
            //PluginImporter dll = AssetImporter.GetAtPath("Assets/GameAnalytics/Plugins/GameAnalytics.dll") as PluginImporter;
            //dll?.SetCompatibleWithAnyPlatform(isUse);
            //dll?.SaveAndReimport();
            if (targetGroup == BuildTargetGroup.Android)
            {
                PluginImporter tmp =
                    AssetImporter.GetAtPath("Assets/GameAnalytics/Plugins/Android/gameanalytics.aar") as PluginImporter;
                tmp?.SetCompatibleWithPlatform(BuildTarget.Android, isUse);
                tmp = AssetImporter.GetAtPath("Assets/GameAnalytics/Plugins/Android/gameanalytics-imei.jar") as
                    PluginImporter;
                tmp?.SetCompatibleWithPlatform(BuildTarget.Android, isUse);
                tmp = AssetImporter.GetAtPath("Assets/GameAnalytics/Plugins/Android/instantapps-1.1.0.aar") as
                    PluginImporter;
                tmp?.SetCompatibleWithPlatform(BuildTarget.Android, isUse);
                tmp = AssetImporter.GetAtPath("Assets/GameAnalytics/Plugins/Android/unity_gameanalytics.jar") as
                    PluginImporter;
                tmp?.SetCompatibleWithPlatform(BuildTarget.Android, isUse);
            }

#if GAME_ANALYTIC_DEFINE
            GameObject eventObject =
 GameObject.FindObjectOfType<GameAnalyticsSDK.Events.GA_SpecialEvents>()?.gameObject;
            GameObject analyticObject = GameObject.FindObjectOfType<GameAnalyticsSDK.GameAnalytics>()?.gameObject;
            if (isUse)
            {
                GameObject gameObject = eventObject;
                if (gameObject == null)
                    gameObject = analyticObject;
                if (eventObject == null && analyticObject == null)
                    gameObject = new GameObject("GameAnalytics");
                if (eventObject == null)
                    gameObject.AddComponent<GameAnalyticsSDK.Events.GA_SpecialEvents>();
                if (analyticObject == null)
                    gameObject.AddComponent<GameAnalyticsSDK.GameAnalytics>();
            }
            else
            {
                if (eventObject != null)
                    DestroyImmediate(eventObject);
                analyticObject = GameObject.FindObjectOfType<GameAnalyticsSDK.GameAnalytics>()?.gameObject;
                if (analyticObject != null)
                    DestroyImmediate(analyticObject);
            }
#endif
        }

        private void ApplyNewBuildConfig(ref BuildConfigMainObject mainAsset, EBuildType newBuildType,
            BuildTargetGroup targetGroup)
        {
            if (mainAsset.BuildType != newBuildType)
            {
                mainAsset.BuildType = newBuildType;
                UpdateScriptingDefineSymbolsForBuildType(newBuildType, targetGroup);
                EditorUtility.SetDirty(mainAsset);
            }
        }

        private void ApplyNewMarketType(ref BuildConfigMainObject mainAsset, EMarketType newMarketType,
            BuildTargetGroup targetGroup)
        {
            if (mainAsset.MarketType != newMarketType)
            {
                mainAsset.MarketType = newMarketType;
                UpdateScriptingDefineSymbolsForMarketType(newMarketType, targetGroup);
                EditorUtility.SetDirty(mainAsset);
            }

            SetGoogleMarketState(newMarketType == EMarketType.Google, targetGroup);
        }

        private void UpdateScriptingDefineSymbolsForBuildType(EBuildType buildType, BuildTargetGroup targetGroup)
        {
            string curretnDefines = GetScriptingDefineSymbols(targetGroup);
            // \b means whole word.
            //Remove already existing build type and add new one.
            string[] buildTypes = Enum.GetNames(typeof(EBuildType));

            int length = buildTypes.Length;
            for (int i = 0; i < length; ++i)
            {
                string replacePattern = buildTypes[i].ToUpper();
                curretnDefines = Regex.Replace(curretnDefines, replacePattern, "");
            }

            if (buildType != EBuildType.NONE)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup,
                    $"{curretnDefines} {buildType.ToString().ToUpper()}");
            }
            else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, curretnDefines);
        }

        private void UpdateScriptingDefineSymbolsForMarketType(EMarketType newType, BuildTargetGroup targetGroup)
        {
            string curretnDefines = GetScriptingDefineSymbols(targetGroup);
            // \b means whole word.
            //Remove already existing build type and add new one.
            string[] marketTypes = Enum.GetNames(typeof(EMarketType));
            int length = marketTypes.Length;
            for (int i = 0; i < length; ++i)
            {
                string replacePattern = marketTypes[i].ToUpper();
                curretnDefines = Regex.Replace(curretnDefines, replacePattern, "");
            }

            if (newType != EMarketType.None)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup,
                    $"{curretnDefines};{newType.ToString().ToUpper()}");
            else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, curretnDefines);
        }

        private void SetGoogleMarketState(bool state, BuildTargetGroup targetGroup)
        {
            PluginImporter tmp =
                AssetImporter.GetAtPath("Assets/Plugins/UnityPurchasing/Bin/Android/billing-3.0.1.aar") as
                    PluginImporter;
            tmp?.SetCompatibleWithPlatform(BuildTarget.Android, state);
            tmp = AssetImporter.GetAtPath("Assets/Plugins/UnityPurchasing/Bin/Android/common.aar") as PluginImporter;
            tmp?.SetCompatibleWithPlatform(BuildTarget.Android, state);
        }

        private void UpdateScriptingDefineSymbolsForPlugin(bool isUse, string symbol, BuildTargetGroup targetGroup)
        {
            string currentDefines = GetScriptingDefineSymbols(targetGroup);
            currentDefines = Regex.Replace(currentDefines, symbol, "");
            if (isUse)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, $"{currentDefines};{symbol}");
            else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, $"{currentDefines}");
        }

        private string GetScriptingDefineSymbols(BuildTargetGroup targetGroup)
        {
            string curretnDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            curretnDefines = curretnDefines.Replace(";", " ");
            return curretnDefines;
        }

        private void UpdateLunarConsoleState(EBuildType buildType)
        {
            bool isEnable = buildType == EBuildType.TEST || buildType == EBuildType.STAGE;
            string[] assetHash = AssetDatabase.FindAssets("lunar-console");
            if (assetHash?.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetHash[0]);
                PluginImporter tmp = AssetImporter.GetAtPath(path) as PluginImporter;
                tmp?.SetCompatibleWithPlatform(BuildTarget.Android, isEnable);
            }
        }

        private void UpdateAdPluginState(ref BuildConfigMainObject mainAsset, BuildTargetGroup targetGroup)
        {
            if (mainAsset.UseAdmob != _isUseAdmob)
            {
                mainAsset.UseAdmob = _isUseAdmob;
                SetAdMobeState(_isUseAdmob, targetGroup);
            }

            if (mainAsset.UseTapsell != _isUseTapsell)
            {
                mainAsset.UseTapsell = _isUseTapsell;
                SetTapsellState(_isUseTapsell, targetGroup);
            }
        }

        private void SetAdMobeState(bool state, BuildTargetGroup targetGroup)
        {
            UpdateScriptingDefineSymbolsForPlugin(state, ADMOB_DEFINE, targetGroup);

            PluginImporter tmp =
                AssetImporter.GetAtPath("Assets/Plugins/Android/GoogleMobileAdsPlugin") as PluginImporter;
            tmp?.SetCompatibleWithPlatform(BuildTarget.Android, state);

            tmp = AssetImporter.GetAtPath("Assets/Plugins/Android/GoogleMobileAdsPlugin.androidlib") as PluginImporter;
            tmp?.SetCompatibleWithPlatform(BuildTarget.Android, state);

            tmp = AssetImporter.GetAtPath("Assets/Plugins/Android/externs.cpp") as PluginImporter;
            tmp?.SetCompatibleWithPlatform(BuildTarget.Android, state);
            tmp = AssetImporter.GetAtPath("Assets/Plugins/Android/googlemobileads-unity.aar") as PluginImporter;
            tmp?.SetCompatibleWithPlatform(BuildTarget.Android, state);
            tmp = AssetImporter.GetAtPath("Assets/GoogleMobileAds/GoogleMobileAds.dll") as PluginImporter;
            tmp?.SetCompatibleWithPlatform(BuildTarget.Android, state);
            tmp?.SetCompatibleWithAnyPlatform(state);

#if GOOGLE_AD
            string path = "Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset";
            GoogleMobileAdsSettings setting = AssetDatabase.LoadAssetAtPath<GoogleMobileAdsSettings>(path);
            //Debug.Log(setting);
            if (setting)
            {
                setting.IsAdManagerEnabled = !state;
                setting.IsAdMobEnabled = state;
                EditorUtility.SetDirty(setting);
            }
#endif
        }

        private void SetTapsellState(bool state, BuildTargetGroup targetGroup)
        {
            UpdateScriptingDefineSymbolsForPlugin(state, TAPSELL_DEFINE, targetGroup);
            PluginImporter tmp = AssetImporter.GetAtPath("Assets/Plugins/Android/Tapsell") as PluginImporter;
            tmp?.SetCompatibleWithPlatform(BuildTarget.Android, state);
        }
    }
}