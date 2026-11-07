using System;
using UnityEngine;

namespace Alexplay.GDPR.Logic.Config
{
    [CreateAssetMenu(fileName = "GdprTranslationConfig", menuName = "GDPR/Translation/Config")]
    [Serializable]
    public class GdprTranslationConfig : ScriptableObject
    {
        public string TitleTranslationKey;
        public string DescriptionTranslationKey;
        public string TermsOfUseTranslationKey;
        public string AndArticleTranslationKey;
        public string PrivacyPoliceTranslationKey;
        
        public string AcceptButtonTranslationKey;
        public string DeclineButtonTranslationKey;
    }
}
