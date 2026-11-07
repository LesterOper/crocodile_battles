using System;
using Alexplay.GDPR.Logic.Constants;
using Alexplay.GDPR.Logic.Enum;
using Alexplay.GDPR.Logic.View;
using UnityEngine;

namespace Alexplay.GDPR.Logic.Service
{
    public class GdprService
    {
        public event Action Accepted;
        public event Action Decline;
        
        public GdprState GetGdprState
        {
            get => (GdprState)PlayerPrefs.GetInt($"GdprService.{GdprConstants.GDPR_VERSION}");
            set => PlayerPrefs.SetInt($"GdprService.{GdprConstants.GDPR_VERSION}", (int)value);
        }

        private GdprView _gdprInstance;
        private static GdprService _instance;
        private static bool _isInitialized = false;

        public static GdprService Instance
        {
            get
            {
                if (_isInitialized == true) return _instance;
                _instance = new GdprService();
                _isInitialized = true;
                return _instance;
            }
        }

        public void ShowGdpr(Transform parent)
        {
            if (parent == null)
                throw new Exception("GDPR parent can't be null, check it.");

            RectTransform rect = parent.GetComponent<RectTransform>();
            if (rect == null)
                throw new Exception("GDPR parent can contains a RectTransform component, check it.");
            
            GdprState currentState = GetGdprState;

            switch (currentState)
            {
                case GdprState.WAITING_TO_ACCEPT:
                case GdprState.DECLINED:
                    CallGdpr(rect);
                    break;
                case GdprState.ACCEPTED: 
                    
                    Accepted?.Invoke(); 
                    break;
            }
        }

        private void CallGdpr(RectTransform parent)
        {
            _gdprInstance = UnityEngine.Object.Instantiate(Resources.Load<GdprView>("GDPR"), parent);

            _gdprInstance.GDPR.Accepted += accept_gdpr;
            _gdprInstance.GDPR.Decline += decline_gdpr;
        }

        private void accept_gdpr()
        {
            _gdprInstance.GDPR.Accepted -= accept_gdpr;
            _gdprInstance.CloseGdpr();
            
            Resources.UnloadUnusedAssets();
            
            GetGdprState = GdprState.ACCEPTED;
            Accepted?.Invoke();
        }
        
        private void decline_gdpr()
        {
            _gdprInstance.GDPR.Decline -= decline_gdpr;
            _gdprInstance.CloseGdpr();
            
            Resources.UnloadUnusedAssets();

            GetGdprState = GdprState.DECLINED;
            Decline?.Invoke();
        }
    }
}