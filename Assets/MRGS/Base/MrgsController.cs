using System;
using App.DAO;
using CodeBase.Infrastructure.Services;
/*using CodeBase.ApplicationLibrary.Data;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Ads;
using CodeBase.Infrastructure.Merge;*/
using GameServices.Mrgs;
using Library.Common;
using MRGS;
using MRGS.Imp;
using UnityEngine;

namespace MRGS.Base
{
    /*public class MrgsController :  IMRGSService, MRGSGDPR.IShowDelegate
    {
        private bool _isDebug;
        //public MrgsBankController Bank => _bank;

        private IDataService _dataService;
        //private MrgsBankController _bank;
        private MrgsServicesController _gameServices;
        private AdsHelper _adsHelper;
        private Action OnCompleteAction;
        private Action loadScene;

        public void Setup(AdsHelper adsHelper, IDataService dataService, Action loadScene)
        {
            _adsHelper = adsHelper;
            _adsHelper.InitAds(true);
            _dataService = dataService;
            ShowGdpr();
            this.loadScene = loadScene;
        }

        private void Initialize()
        {
            MRGServiceParams serviceParams = GetServiceParams();
            MRGSExternalSDKParams sdkParams = GetSdkParams();
            MRGService.Instance.Initialize(serviceParams, sdkParams);
            
            MRGSMyTracker.Instance.SetAttributionListener(attribution =>
            {
                Debug.Log("Received attribution from MRGSMyTracker - " + attribution);
            });

            Debug.Log("Set AppsFlyer CustomerUserID - " + MRGSAppsFlyer.Instance.CustomerUserID);
            
#if UNITY_IOS
            bool isLoggedIn = MRGSAuthenticationAppleGameCenter.getInstance().isLoggedIn();
            if (!isLoggedIn)
            {
                // В ответе придет объект, описывающий пользователя и токен, и объект ошибки, если она случилась
                MRGSAuthenticationAppleGameCenter.getInstance().loginWithCompletionHandler(
                    (MRGSAuthenticationCredential credentials, MRGSError error) =>
                    {
                        string resultDescription = "loginWithCompletionHandler:\nCredentials: " + credentials +
                                                   "\nError:\n" + error;
                    });
            }
#endif

            AuthorizeUser();
        }

        private void AuthorizeUser()
        {
            if (string.IsNullOrEmpty(_dataService.GetString("PLAYER_ID")))
            {
                string userIdentifier = MRGSUsers.Instance.GenerateUserId();
                MRGSUsers.Instance.SetUserId(userIdentifier);
                _dataService.SetString("PLAYER_ID", userIdentifier);
#if UNITY_IOS
                CheckMRGSIDFATrackingAuthorization();
#endif
                MRGSMyTracker.Instance.TrackRegistrationEvent();
                MRGSMyTracker.Instance.TrackLoginEvent();
            }
            else
            {
                MRGSUsers.Instance.SetUserId(_dataService.GetString("PLAYER_ID"));
#if UNITY_IOS
                CheckMRGSIDFATrackingAuthorization();
#endif
                MRGSMyTracker.Instance.TrackLoginEvent();
            }

            _gameServices = new MrgsServicesController(() =>
            {
                loadScene.Invoke();
            });
        }

        private MRGServiceParams GetServiceParams()=>
            new MRGServiceParams(MrgsConsts.APP_ID, MrgsConsts.CLIENT_SECRET)
            {
                Debug = MrgsConsts.SERVICE_PARAMS_DEBUG,
                TestDevice = MrgsConsts.SERVICE_TEST_DEVICE,
                CrashReportEnabled = MrgsConsts.SERVICE_CRASH_REPORT_ENABLED,
                LocationTrackingEnabled = MrgsConsts.SERVICE_LOCATION_TRACKING_ENABLED,
                IOSExtraOptions =
                {
                    AutomaticallyShowIDFARequestAtStartup = MrgsConsts.SERVICE_AUTO_SHOW_IDFA_AT_STARTUP,
                    DisablePaymentsCheck = MrgsConsts.SERVICE_DISABLE_PAYMENT_CHECK,
                    AutomaticPaymentTracking = MrgsConsts.SERVICE_AUTO_PAYMENT_TRACKING,
                    ShouldResetBadge = MrgsConsts.SERVICE_SHOULD_RESET_BADGE,
                    StorePath = MrgsConsts.SERVICE_STORE_PATH,
                    ShowTrackingAuthorizationDialogOnAlliOSVersions =
                        MrgsConsts.SERVICE_SHOW_AUTH_DIALOG_ALL_IOS_VERSIONS
                },
                AndroidExtraOptions =
                {
                    BillingType = MrgsConsts.SERVICE_ANDROID_BILLING_TYPE, Platform = MRGSPlatformAndroid.Android
                }
            };

        private MRGSExternalSDKParams GetSdkParams() =>
            new MRGSExternalSDKParams
            {
                AppsFlyerParams = new MRGSAppsFlyerParams(MrgsConsts.APPS_FLYER_DEVKEY,
                    appId: MrgsConsts.APPLE_APP_ID)
                {
                    Debug = MrgsConsts.SDK_APPSFLYER_DEBUG,
                    ForwardMetrics = MrgsConsts.SDK_APPSFLYER_FORWARD_METRICS,
                    WaitForATTUserAuthorization = MrgsConsts.SDK_APPSFLYER_WAIT_FOR_ATT_USER_AUTH,
                    DontForwardReportedPayments = MrgsConsts.SDK_APPPSFLYER_DONT_FORWARD_REPORTED_PLAYMENTS,
                    AppInviteOneLinkID = MrgsConsts.SDK_APPSFLYER_APP_INVITE_ONE_LINK_ID
                },
                MyTrackerParams = new MRGSMyTrackerParams(MrgsConsts.MY_TRACKER_SDK_KEY)
                {
                    ForwardMetrics = MrgsConsts.SDK_MYTRACKER_FORWARD_METHRICS,
                    Debug = MrgsConsts.SDK_MYTRACKER_DEBUG,
                },
                AppleGameCenterParams = new MRGSAppleGameCenterParams()
                {
                    AutomaticallyStartOnLaunch = MrgsConsts.SDK_APPLE_GAME_CENTER_SAUTO_START
                }
            };
        

        #region GDPR

        private void ShowGdpr()
        {
            MRGSGDPR instance = MRGSGDPR.getInstance();
            instance.Delegate = this;
            instance.withAdvertising(MrgsConsts.GDPR_WITH_ADVERTISING);
            instance.onlyForEU(MrgsConsts.GDPR_ONLY_FOR_EU);
            instance.setLocalizationLanguage(MrgsConsts.LANGUAGE_CODE);
            //instance.showDefaultAgreement(MrgsConsts.APP_ID);
            instance.showAgreementFromFile(MrgsConsts.APP_ID, "mrgsgdpr.html");
        }

        public void OnUserHasAcceptedGDPR(bool withAdvertising)
        {
            Initialize();
        }

        public void OnGDPRError()
        {
            MRGSGDPR.getInstance().showDefaultAgreement(MrgsConsts.APP_ID);
        }

        #endregion

        #region ATE

        private void CheckMRGSIDFATrackingAuthorization()
        {
#if UNITY_IOS
            Debug.Log("IDFA: start");
            bool hasAgreed = MRGSDevice.getInstance().hasUserAgreedToBeTracked();
            Debug.Log("IDFA: agreed status - " + hasAgreed);

            if (hasAgreed)
            {
                if (FB.IsInitialized)
                {
                    FB.Mobile.SetAdvertiserTrackingEnabled(true);
                    Debug.Log("IDFA: fb agreed");
                }
                else
                {
                    FB.Init(() =>
                    {
                        FB.Mobile.SetAdvertiserTrackingEnabled(true);
                        Debug.Log("IDFA: fb agreed");
                    });
                }
            }
            else
            {
                bool should = MRGSDevice.getInstance().shouldShowTrackingAuthorizationDialog();
                Debug.Log("IDFA: agreed status should - " + should);

                if (should)
                {
                    MRGSDevice.getInstance().requestTrackingAuthorizationWithCompletionHandler((status =>
                    {
                        MRGSLog.getInstance().addLog("Did recieve auth status - " + status);
                        switch (status)
                        {
                            case MRGSIDFATrackingAuthorizationStatus.NotDetermined: break;
                            case MRGSIDFATrackingAuthorizationStatus.Denied:
                                if (FB.IsInitialized)
                                {
                                    FB.Mobile.SetAdvertiserTrackingEnabled(false);
                                    Debug.Log("IDFA: fb not agreed");
                                }
                                else
                                {
                                    FB.Init(() =>
                                    {
                                        FB.Mobile.SetAdvertiserTrackingEnabled(false);
                                        Debug.Log("IDFA: fb not agreed");
                                    });
                                }

                                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(false);
                                Debug.Log("IDFA: audience not agreed");
                                break;
                            case MRGSIDFATrackingAuthorizationStatus.Restricted: break;
                            case MRGSIDFATrackingAuthorizationStatus.Authorized:
                                if (FB.IsInitialized)
                                {
                                    FB.Mobile.SetAdvertiserTrackingEnabled(true);
                                    Debug.Log("IDFA: fb agreed");
                                }
                                else
                                {
                                    FB.Init(() =>
                                    {
                                        FB.Mobile.SetAdvertiserTrackingEnabled(true);
                                        Debug.Log("IDFA: fb agreed");
                                    });
                                }

                                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
                                Debug.Log("IDFA: audience agreed");
                                break;
                        }
                    }));
                }
                else
                {
                    FB.Mobile.SetAdvertiserTrackingEnabled(true);
                    Debug.Log("IDFA: audience agreed");
                }
            }
#endif
        }

        #endregion


        public void Initialize(AdsHelper adsHelper, Action OnComplete)
        {
            
        }
    }*/
}

public interface IMRGSService : IService
{
    void Initialize(AdsHelper adsHelper, Action OnComplete);
    void Setup(AdsHelper adsHelper, IDataService dataService, Action loadScene);
}