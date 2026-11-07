using I2.Loc;

namespace GameServices.Mrgs
{
    public class MrgsConsts
    {
        #if UNITY_IOS
        public const string APP_ID = "1034";
        public const int app_ip = 1034;
        public const string MY_TRACKER_SDK_KEY = "89821236465886394412";
        public const string CLIENT_SECRET = "K7HMGiW2KYHgZJKdYy9@Tl@5FlLbxdzD";
        #elif UNITY_ANDROID
        public const string APP_ID = "1027";
        public const int app_ip = 1027;
        public const string MY_TRACKER_SDK_KEY = "10406734827698864431";
        public const string CLIENT_SECRET = "JhPFXZfXMpcccTMUzq@w8nv6_DodNj46";
        #endif
        
        public const string APPS_FLYER_DEVKEY = "a8zXEpwuocjChhs3D2RR9k";
        public const string WEB_CLIENT_ID = "488744410183-jm0lmf61ee19ii0bv5073qno3lu0ese9.apps.googleusercontent.com";
        public const string APPLE_APP_ID = "1626808627";

        #region GDPR

        #if UNITY_ANDROID
        public static bool GDPR_USE_DEFAULT_HTML => false;
        #else
        public static bool GDPR_USE_DEFAULT_HTML => false;
        #endif
        
        public const bool GDPR_WITH_ADVERTISING = false;
        public const bool GDPR_ONLY_FOR_EU = false;
        public static readonly string GDPR_CUSTOM_HTML_PATH = "Assets/customGdpr.html";
        public static string LANGUAGE_CODE => LocalizationManager.CurrentLanguageCode;
        
        #endregion

        #region SERVICE_PARAMS

        public const bool SERVICE_DISABLE_PAYMENT_CHECK = false;
        public const bool SERVICE_AUTO_SHOW_IDFA_AT_STARTUP = true;
        public const bool SERVICE_PARAMS_DEBUG = false;
        public const bool SERVICE_TEST_DEVICE = false;
        public const bool SERVICE_CRASH_REPORT_ENABLED = true;
        public const bool SERVICE_LOCATION_TRACKING_ENABLED = false;
        public const bool SERVICE_AUTO_PAYMENT_TRACKING = false;
        public const bool SERVICE_SHOULD_RESET_BADGE = true;
        public const string SERVICE_STORE_PATH = "unityMRGS";
        public const bool SERVICE_SHOW_AUTH_DIALOG_ALL_IOS_VERSIONS = false;
        public const string SERVICE_ANDROID_BILLING_TYPE = "google";

        #endregion

        #region SERVICE_PARAMS

        public static bool SDK_MYTRACKER_FORWARD_METHRICS = true;
        public static bool SDK_MYTRACKER_DEBUG = false;
        public static bool SDK_APPSFLYER_DEBUG = false;
        public static bool SDK_APPSFLYER_FORWARD_METRICS = true;
        public static float SDK_APPSFLYER_WAIT_FOR_ATT_USER_AUTH = 10.0f;
        public static bool SDK_APPPSFLYER_DONT_FORWARD_REPORTED_PLAYMENTS = false;
        public static bool SDK_APPLE_GAME_CENTER_SAUTO_START = true;
        public static string SDK_APPSFLYER_APP_INVITE_ONE_LINK_ID = "test_one_link_id";

        #endregion

        #region BANKS_PARAMS

        public const bool BANK_AUTO_RESTORE_PURCHASE = false;

        #endregion
    }
}