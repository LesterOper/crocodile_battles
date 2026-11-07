using System;
using Alexplay.GDPR.Logic.Constants;
using UnityEngine;
using UnityEngine.UI;

namespace Alexplay.GDPR.Logic
{
    public class GDPR : IDisposable
    {
        public event Action Accepted;
        public event Action Decline;
        
        public GDPR(
            Button usingLinkButton,
            Button privacyLinkButton,
            Button acceptButton,
            Button declineButton
        )
        {
            usingLinkButton.onClick.AddListener(TransferToTermsOfUseLink);
            privacyLinkButton.onClick.AddListener(TransferToPrivacyPolicyLink);
            acceptButton.onClick.AddListener(AcceptGdpr);
            declineButton.onClick.AddListener(DeclineGdpr);
        }

        private void TransferToTermsOfUseLink() => Application.OpenURL(GdprConstants.TermsOfUseLink);
        private void TransferToPrivacyPolicyLink() => Application.OpenURL(GdprConstants.PrivacyPolicyLink);
        private void AcceptGdpr() => Accepted?.Invoke();
        private void DeclineGdpr() => Decline?.Invoke();

        private void ClearLinks()
        {
            Accepted = null;
            Decline = null;
        }

        public void Dispose() => ClearLinks();
    }
}
