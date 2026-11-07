namespace Alexplay.GDPR.Logic.Constants
{
    public class GdprConstants
    {
        public const string TermsOfUseLink = "https://alexplay.net/terms-of-use/";
        public const string PrivacyPolicyLink = "https://alexplay.net/privacy-policy/";
        
        public const string GdprConfigDirectory = "GdprConfigs/";
        
        private static int _globalVersionIndex = 0;
        private static int _globalMajorIndex = 0;
        private static int _globalMinorIndex = 1;
        
        public static readonly string GDPR_VERSION = $"GDPR:{_globalVersionIndex}.{_globalMajorIndex}.{_globalMinorIndex}";
    }
}