using MRGS.MRGSAuthenticationControllers;
using UnityEngine;

namespace MRGS
{
    public class AuthHelper : MonoBehaviour
    {
        private DetailedAuthGoogleGame _authGoogle;
        private DetailedAuthSignInWithApple _authWithApple;

        private bool _loginCalled;

        private void Start()
        {
            _authGoogle = GetComponent<DetailedAuthGoogleGame>();
            _authWithApple = GetComponent<DetailedAuthSignInWithApple>();
        }

        public void Login()
        {
#if UNITY_ANDROID
            //_authGoogle.Login();
            Debug.Log("[MasterController] - LogedIn");
#elif UNITY_IOS
      //_authWithApple.onSignInWithAppleLoginClick();
#endif
            _loginCalled = true;
        }

        public void Logout()
        {
#if UNITY_ANDROID
            //_authGoogle.Logout();
#elif UNITY_IOS
       //_authWithApple.onSignInWithAppleLogoutClick();
#endif  
        }

        /*public void OpenAchievements()
    {
#if UNITY_ANDROID
        if(_loginCalled)
            _authGoogle.OnShowAchievements();
#elif UNITY_IOS
  
            _authWithApple.OnShowAchievements();
#endif  
    }
    
    public void OpenLeaderboard()
    {
#if UNITY_ANDROID
        if(_loginCalled)
            _authGoogle.OnShowLeaderboard();
#elif UNITY_IOS
      
            _authWithApple.OnShowLeaderboard();
#endif 
    }*/
    
        /*public void SendScoreLeaderboard(ulong score, string id)
    {
        
#if UNITY_ANDROID
        if(_loginCalled)
            _authGoogle.SubmitScoreForLeaderboard(score,id);
#elif UNITY_IOS
          
            _authWithApple.SubmitScoreForLeaderboard(score,id);
#endif  
    }
    
    public void SendAchievement( string id)
    {
#if UNITY_ANDROID
        if(_loginCalled)
            _authGoogle.SetAchievement(id);
       
#elif UNITY_IOS

        _authWithApple.SetAchievement(id);
#endif  
    }*/
    }
}
