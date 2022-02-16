using UnityEngine;

namespace GameWrriors.BuildConfiguration.Editor.Data
{
    public enum EBuildType
    {
        NONE,
        DEVELOPMENT,
        TEST,
        STAGE,
        PRODUCTION
    }

    public enum EMarketType
    {
        None,
        Bazaar,
        Myket,
        Google,
        Apple
    }

    public class BuildConfigMainObject : ScriptableObject
    {
        [SerializeField, HideInInspector] private EBuildType _buildType;
        [SerializeField, HideInInspector] private EMarketType _marketType;
        [SerializeField, HideInInspector] private bool _useGameAnalytic;
        [SerializeField, HideInInspector] private bool _useFirebase;
        [SerializeField, HideInInspector] private bool _useMetrix;
        [SerializeField, HideInInspector] private bool _useAdmob;
        [SerializeField, HideInInspector] private bool _useTapsell;
        [SerializeField, HideInInspector] private bool _useAppMetrica;
        [SerializeField, HideInInspector] private bool _disableAppMetricaLocation;
        [SerializeField, HideInInspector] private bool _useAppsFlyer;


        public EBuildType BuildType
        {
            get => _buildType;
            set => _buildType = value;
        }

        public EMarketType MarketType
        {
            get => _marketType;
            set => _marketType = value;
        }

        public bool UseGameAnalytic
        {
            get => _useGameAnalytic;
            set => _useGameAnalytic = value;
        }

        public bool UseFirebase
        {
            get => _useFirebase;
            set => _useFirebase = value;
        }

        public bool UseMetrix
        {
            get => _useMetrix;
            set => _useMetrix = value;
        }

        public bool UseAdmob
        {
            get => _useAdmob;
            set => _useAdmob = value;
        }

        public bool UseTapsell
        {
            get => _useTapsell;
            set => _useTapsell = value;
        }

        public bool UseAppMetrica
        {
            get => _useAppMetrica;
            set => _useAppMetrica = value;
        }

        public bool DisableAppMetricaLocation
        {
            get => _disableAppMetricaLocation;
            set => _disableAppMetricaLocation = value;
        }

        public bool UseAppsFlyer
        {
            get => _useAppsFlyer;
            set => _useAppsFlyer = value;
        }
    }
}