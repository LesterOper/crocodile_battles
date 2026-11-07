using System;
using Alexplay.GDPR.Logic.Config;
using Alexplay.GDPR.Logic.Constants;
using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace Alexplay.GDPR.Logic.View
{
    public class GdprView : MonoBehaviour
    {
        public GDPR GDPR => _gdpr;
        
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _termsOfUseText;
        [SerializeField] private TextMeshProUGUI _andArticleText;
        [SerializeField] private TextMeshProUGUI _privacyPoliceText;
        [SerializeField] private TextMeshProUGUI _acceptButtonText;
        [SerializeField] private TextMeshProUGUI _declineButtonText;
        
        [Space(10)]
        [Header("Buttons")]
        [SerializeField] private UnityEngine.UI.Button _usingLinkButton;
        [SerializeField] private UnityEngine.UI.Button _privacyLinkButton;
        [SerializeField] private UnityEngine.UI.Button _acceptButton;
        [SerializeField] private UnityEngine.UI.Button _declineButton;
        
        [Space(10)]
        [Header("GdprBackgroundCanvasGroup")]
        [SerializeField] private CanvasGroup _viewCanvasGroup;

        private GDPR _gdpr;
        
        private void Awake()
        {
            _gdpr = new GDPR(_usingLinkButton, _privacyLinkButton, _acceptButton, _declineButton);

            BreakState();
            Localize();
            Animate();
        }

        public void CloseGdpr()
        {
            _viewCanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                _gdpr.Dispose();
                Destroy(gameObject);
            });

        }
        
        private void BreakState()
        {
            _viewCanvasGroup.transform.localScale = Vector3.zero;
            _viewCanvasGroup.alpha = 0;
        }
        
        private void Animate()
        {
            _viewCanvasGroup.DOFade(1f, 0.2f);
            _viewCanvasGroup.transform.DOScale(1.7f, 0.1f).OnComplete(() => 
                _viewCanvasGroup.transform.DOScale(1.64f, 0.1f));
        }

        private void Localize()
        {
            GdprTranslationConfig translationConfig = Resources.Load<GdprTranslationConfig>(GdprConstants.GdprConfigDirectory + "GdprTranslationConfig");

            if (translationConfig == null)
                throw new Exception(
                    message: "Can't load gdpr translation config with equal name. Check Directory " +
                             "(Alexplay->GDPR->Resources->GdprConfigs). If config contains, check its name. " +
                             "Its name must be (GdprTranslationConfig)");

            _titleText.text = LocalizationManager.GetTranslation(translationConfig.TitleTranslationKey);
            _descriptionText.text = LocalizationManager.GetTranslation(translationConfig.DescriptionTranslationKey);
            _termsOfUseText.text = LocalizationManager.GetTranslation(translationConfig.TermsOfUseTranslationKey);
            _andArticleText.text = LocalizationManager.GetTranslation(translationConfig.AndArticleTranslationKey);
            _privacyPoliceText.text = LocalizationManager.GetTranslation(translationConfig.PrivacyPoliceTranslationKey);
            _acceptButtonText.text = LocalizationManager.GetTranslation(translationConfig.AcceptButtonTranslationKey);
            _declineButtonText.text = LocalizationManager.GetTranslation(translationConfig.DeclineButtonTranslationKey);
        }
    }
}
