using System;
using System.Threading.Tasks;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace MRGS.MRGSAuthenticationControllers
{
    public class DetailedAuthGoogleGame
    {
        private Action OnLoginComplete;
        public DetailedAuthGoogleGame(Action OnComplete)
        {
            OnLoginComplete = OnComplete;
            VoxelBusters.EssentialKit.GameServices.OnAuthStatusChange += GameServicesOnOnAuthStatusChange;
        }
        private void GameServicesOnOnAuthStatusChange(GameServicesAuthStatusChangeResult result, Error error)
        {
            if (result.AuthStatus == LocalPlayerAuthStatus.Authenticated)
            {
                
            }
        }
        public async void Login()
        {
            VoxelBusters.EssentialKit.GameServices.Authenticate();

            await Task.Delay(3000);
            
            OnLoginComplete?.Invoke();
        }

        public void Logout() => VoxelBusters.EssentialKit.GameServices.Signout();
    }
}