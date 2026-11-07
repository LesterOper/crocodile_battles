using System;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace MRGS.MRGSAuthenticationControllers
{
    public class DetailedAuthSignInWithApple
    {
        //private IMRGSAuthenticationAppleGameCenter AppleGameCenter => MRGSAuthenticationAppleGameCenter.Instance;
        
        private Action OnLoginComplete;

        public DetailedAuthSignInWithApple(Action OnComplete)
        {
            OnLoginComplete = OnComplete;
            VoxelBusters.EssentialKit.GameServices.OnAuthStatusChange += GameServicesOnOnAuthStatusChange;
            Login();
        }

        private void GameServicesOnOnAuthStatusChange(GameServicesAuthStatusChangeResult result, Error error)
        {
            if (result.AuthStatus == LocalPlayerAuthStatus.Authenticated)
            {
            
            }
        }

        private void Login()
        {
            VoxelBusters.EssentialKit.GameServices.Authenticate(); 
            OnLoginComplete?.Invoke();
        }
        
        /*public void SetAchievement(string achievementId)
        {
            IMRGSAuthenticationAppleGameCenter gamesProtocolInstance = AppleGameCenter;
            gamesProtocolInstance.getAchievements((achievements, achievementsError) =>
            {
                Debug.Log(" GOOGLE AUTH getAchievements : " + achievements);
                if (achievements != null && achievements.Count > 0)
                {
                    gamesProtocolInstance.setSteps(100, achievementId, error => { });
                }
                else
                {
                    string resultDescription = "getAchievements error:\nAchievements: " + GetStringDesc(achievements) + "\nError:\n" + GetStringDesc(achievementsError);
                    Log(resultDescription);
                }
            });
        }
    
    
        public void OnShowAchievements()
        {
            IMRGSAuthenticationAppleGameCenter gamesProtocolInstance = AppleGameCenter;
            gamesProtocolInstance.showAchievements(() =>
            {
                const string message = "showAchievements closed";
                Log(message);
            });
        }
    
        public void OnShowLeaderboard()
        {
            IMRGSAuthenticationAppleGameCenter gamesProtocolInstance = AppleGameCenter;
            Debug.Log(" APPLE AUTH gamesProtocolInstance : " + gamesProtocolInstance);
            gamesProtocolInstance.getLeaderboards((leaderboards, error) =>
            {
                Debug.Log(" APPLE AUTH getLeaderboards : " + leaderboards);
                if (leaderboards != null && leaderboards.Count > 0)
                {
                    gamesProtocolInstance.showLeaderboardWithId(leaderboards[0].LeaderboardId, () =>
                    {
                        const string message = "showLeaderboardWithId closed";
                        Log(message);
                    });
                }
                else
                {
                    string resultDescription = "getLeaderboards error:\nLeaderboards: " + GetStringDesc(leaderboards) + "\nError:\n" + GetStringDesc(error);
                    Log(resultDescription);
                }
            });
        }
    
        public void SubmitScoreForLeaderboard(ulong score , string leaderboardId)
        {
            IMRGSAuthenticationAppleGameCenter gamesProtocolInstance = AppleGameCenter;
            gamesProtocolInstance.getLeaderboards((leaderboards, leaderboardsError) =>
            {
                if (leaderboards != null && leaderboards.Count > 0)
                {
                    gamesProtocolInstance.submitScore(score, leaderboardId, error =>
                    {
                        string resultDescription = "submitScore:\nError:\n" + GetStringDesc(error);
                        Log(resultDescription);
                    });
                }
                else
                {
                    string resultDescription = "getLeaderboards error:\nLeaderboards: " + GetStringDesc(leaderboards) + "\nError:\n" + GetStringDesc(leaderboardsError);
                    Log(resultDescription);
                }
            });
        }
        */



        /*public void onAuthenticationProviderDidLogoutUser(MRGSAuthentication provider){
            MRGSLog.getInstance().addLog("MRGSAuthentication<Unity> - onAuthenticationProviderDidLogoutUser: " + "\nNetwork: " + provider.socialId().ToString());
        }*/

        public void onSignInWithAppleLoginClick()
        {
            VoxelBusters.EssentialKit.GameServices.Authenticate();
            /*ProjectContext.Instance.Container.Resolve<IDelayedCallbackManager>().Invoke(3f, () =>*/ OnLoginComplete?.Invoke();
        }
    
        public void onSignInWithAppleLogoutClick()
        {
            VoxelBusters.EssentialKit.GameServices.Signout();
        }
    

        private string getStringDescr(object objectToConvert){
            return objectToConvert != null ? objectToConvert.ToString() : "null";
        }
    
    
        private string GetStringDesc(object objectToConvert)
        {
            return objectToConvert != null ? objectToConvert.ToString() : "null";
        }
    
        /*private void Log(string message)
        {
            MRGSLog.getInstance().addLog("MRGSAuthentication<Unity> - "   + message);
        }*/

    }
}
