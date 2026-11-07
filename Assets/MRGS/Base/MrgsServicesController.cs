using System;
using MRGS.Base;
using MRGS.MRGSAuthenticationControllers;

namespace MRGS.Imp
{
    public class MrgsServicesController : MRGSServicesDelegate
    {
        private DetailedAuthGoogleGame _authGoogle;
        private DetailedAuthSignInWithApple _authWithApple;
        //private readonly MrgsController _mrgsController;
        private bool _loginCalled;

        public MrgsServicesController(Action OnComplete)
        {
#if UNITY_ANDROID
            _authGoogle = new DetailedAuthGoogleGame(OnComplete);            
#elif UNITY_IOS
            _authWithApple = new DetailedAuthSignInWithApple(OnComplete);
#endif
            Login();
        }
        
        public void Login()
        {
#if UNITY_ANDROID
            _authGoogle.Login();
#elif UNITY_IOS
            _authWithApple.onSignInWithAppleLoginClick();
#endif
            _loginCalled = true;
        }

        public void Logout()
        {
#if UNITY_ANDROID
            _authGoogle.Logout();
#elif UNITY_IOS
            _authWithApple.onSignInWithAppleLogoutClick();
#endif
        }

        public void OpenAchievements()
        {
#if UNITY_ANDROID
            //if(_loginCalled) _authGoogle.OnShowAchievements();
#elif UNITY_IOS
            _authWithApple.OnShowAchievements();
#endif  
        }
    
        public void OpenLeaderboard()
        {
#if UNITY_ANDROID
            //if(_loginCalled) _authGoogle.OnShowLeaderboard();
#elif UNITY_IOS
            _authWithApple.OnShowLeaderboard();
#endif 
        }
    
        public void SendScoreLeaderboard(ulong score, string id)
        {
#if UNITY_ANDROID
            //if(_loginCalled) _authGoogle.SubmitScoreForLeaderboard(score,id);
#elif UNITY_IOS
            _authWithApple.SubmitScoreForLeaderboard(score,id);
#endif  
        }
    
        public void SendAchievement(string id)
        {
#if UNITY_ANDROID
            //if(_loginCalled) _authGoogle.SetAchievement(id);
#elif UNITY_IOS
             _authWithApple.SetAchievement(id);
#endif  
        }
    }
}
