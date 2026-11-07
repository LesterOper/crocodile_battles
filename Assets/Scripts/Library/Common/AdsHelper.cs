using System;
using System.Collections;
using System.Collections.Generic;
using App.Analytic;
using App.DAO;
using CodeBase.Infrastructure.Services;
using Firebase.Analytics;
using MRGS;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


namespace Library.Common
{
    [DisallowMultipleComponent]
    public class AdsHelper : MonoBehaviour
    {
        [SerializeField] private float _interstitialCountdown = 20;
        [SerializeField] private string _androidIronSourceId;
        [SerializeField] private string _iosIronSourceId;
        [Inject] private IAnalytics _analytics;
        [Inject] private IDataService _dataService;
        public event Action<bool> OnVideoStateChanged;

        public bool IsNoAdsBought
        {
            get => _isNoAdsBought;
            set
            {
                _isNoAdsBought = value;
                SetBannerVisible(value == false);
            }
        }
        private bool _initialized = false;
        private bool _bannerVisible;
        private Action _onVideoShown;
        private float _interstitialTimer;
        private AmplitudePlacementProperty _place;
        private bool _rewarded;
        private bool _isNoAdsBought;


        public void InitAds(bool accepted)
        {
            if (!_initialized)
            {
                IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
                IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent; 
                IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
                IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
                IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
                IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent; 
                IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
                IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
            
                IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
                IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;        
                IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent; 
                IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent; 
                IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
                IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
                IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
            
                IronSourceEvents.onImpressionSuccessEvent += ImpressionSuccessEvent;

                IronSourceEvents.onSdkInitializationCompletedEvent += OnSdkInitializationCompletedEvent;
#if UNITY_ANDROID
                IronSource.Agent.init (_androidIronSourceId, 
                    IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);
#elif UNITY_IOS
            IronSource.Agent.init (_iosIronSourceId, 
                IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);
#endif
                
                StartCoroutine(InterstitialTimer());
            }
        }

        public void SetBannerVisible(bool bannerVisible)
        {
            if (_initialized)
            {
                if (bannerVisible)
                    IronSource.Agent.displayBanner();
                else
                    IronSource.Agent.hideBanner();
            }
            _bannerVisible = bannerVisible;
        }

        public bool HasVideo()
        {
#if UNITY_EDITOR
            return true;
#else
            return IronSource.Agent.isRewardedVideoAvailable();
#endif
        }

        public void PlayVideo(Action callback, AmplitudePlacementProperty placement)
        {
#if UNITY_EDITOR
            callback.Invoke();
            _interstitialTimer = - 6 * 60 + _interstitialCountdown;

#else
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                AudioListener.volume = 0;
                
              _analytics.SendEvent(AmplitudeEvent.ad_reward_show, 
                        new Dictionary<string, object>(){{AmplitudePlacement.placement.ToString(), placement.ToString()}});
                IronSource.Agent.showRewardedVideo();
                Time.timeScale = 0;
                _onVideoShown = callback;
                _place = placement;
                _interstitialTimer = 0;

}
            
#endif
        }

        public void ShowInterstitial()
        {
            if(_isNoAdsBought) return;
            if (IronSource.Agent.isInterstitialReady())
            {
                IronSource.Agent.showInterstitial();
                AudioListener.volume = 0;
                Time.timeScale = 0;
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            IronSource.Agent.onApplicationPause(pauseStatus);
        }

        private void OnApplicationFocus(bool focus)
        {
            IronSource.Agent.onApplicationPause(!focus);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            _onVideoShown = null;
        }
        
        private IEnumerator InterstitialTimer()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(_interstitialCountdown);
                if (IronSource.Agent.isInterstitialReady() == false)
                {
                    IronSource.Agent.loadInterstitial();
                }
                _interstitialTimer += _interstitialCountdown;
            }
        }

        private void OnSdkInitializationCompletedEvent()
        {
            IronSource.Agent.loadInterstitial();
            IronSource.Agent.validateIntegration();
            _initialized = true;
        }
        
        private void ImpressionSuccessEvent(IronSourceImpressionData impressionData) {
            if (impressionData != null)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param["ad_platform"] = "ironSource";
                param["adNetwork"] = impressionData.adNetwork;
                param["adUnit"] = impressionData.adUnit;
                param["instanceName"] = impressionData.instanceName;
                param["currency"] = "USD";
                param["value"] = impressionData.revenue.ToString();
                param["auctionId"] = impressionData.auctionId;
                param["lifetimeRevenue"] = impressionData.lifetimeRevenue.ToString();
                param["country"] = impressionData.country;
                param["ab"] = impressionData.ab;
                param["segmentName"] = impressionData.segmentName;
                param["placement"] = impressionData.placement;
                param["instanceId"] = impressionData.instanceId;
                param["precision"] = impressionData.precision;
                param["encryptedCPM"] = impressionData.encryptedCPM;
                param["conversionValue"] = impressionData.conversionValue.ToString();
                //MRGSAppsFlyer.getInstance().trackEvent("ad_impression", param);
                AppsFlyerSDK.AppsFlyer.sendEvent("ad_impression", param);
                Firebase.Analytics.Parameter[] adParameters =
                {
                    new Firebase.Analytics.Parameter("ad_platform", "ironsource"),
                    new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
                    new Firebase.Analytics.Parameter("ad_unit_name", impressionData.instanceName),
                    new Firebase.Analytics.Parameter("ad_format", impressionData.adUnit),
                    new Firebase.Analytics.Parameter("currency", "USD"),
                    new Firebase.Analytics.Parameter("value", impressionData.revenue.Value)
                };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", adParameters);
            }
        }

        private void CheckAdsClose()
        {
            _rewarded = false;
        }

        #region Rewards
        private void RewardedVideoAdRewardedEvent(IronSourcePlacement ironSourcePlacement) 
        {
            _rewarded = true;
            AudioListener.volume = 1;

            _onVideoShown?.Invoke();
            _onVideoShown = null;
            _interstitialTimer = 0;
        }
        
        private void RewardedVideoAvailabilityChangedEvent(bool b) 
        {
            OnVideoStateChanged?.Invoke(b);
        }
        
        private void RewardedVideoAdStartedEvent() 
        {
            Time.timeScale = 0;
            AudioListener.volume = 0;
        }
        
        private void RewardedVideoAdEndedEvent() 
        {
            Time.timeScale = 1;
            AudioListener.volume = 1;
            _analytics.SendEvent(AmplitudeEvent.ad_reward_shown, 
                    new Dictionary<string, object>(){{AmplitudePlacement.placement.ToString(), _place.ToString()}});
        }
        
        private void RewardedVideoAdShowFailedEvent(IronSourceError ironSourceError) 
        {
            Time.timeScale = 1;
           
        }
        
        private void RewardedVideoAdClickedEvent(IronSourcePlacement ironSourcePlacement) 
        {
          
        }
        
        private void RewardedVideoAdOpenedEvent()
        {
        }
        
        private void RewardedVideoAdClosedEvent()
        {
            Time.timeScale = 1;
            _analytics.SendEvent(AmplitudeEvent.ad_reward_cancel, 
                new Dictionary<string, object>(){{AmplitudePlacement.placement.ToString(), _place.ToString()}});
            CancelInvoke(nameof(CheckAdsClose));
            Invoke(nameof(CheckAdsClose),1);
        }
        #endregion
        
        #region Interstitials
        private void InterstitialAdReadyEvent() 
        {
        }
        
        private void InterstitialAdLoadFailedEvent(IronSourceError ironSourceError) 
        {
        }
        
        private void InterstitialAdShowSucceededEvent()
        {
            _analytics.SendEvent(AmplitudeEvent.ad_interstitial_show);
        }
        
        private void InterstitialAdShowFailedEvent(IronSourceError ironSourceError) 
        {
        }
        
        private void InterstitialAdClickedEvent() 
        {
           
        }
        
        private void InterstitialAdOpenedEvent() 
        {
        }
        
        private void InterstitialAdClosedEvent()
        {
            Time.timeScale = 1;
            AudioListener.volume = 1;
            _analytics.SendEvent(AmplitudeEvent.ad_interstitial_shown);
        }
        #endregion

    }
}