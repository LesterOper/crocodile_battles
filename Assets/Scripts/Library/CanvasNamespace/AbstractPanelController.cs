using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Library.CanvasNamespace
{
    public abstract class AbstractPanelController : MonoBehaviour, IPanelController
    {
        public void SetupButton(Button TargetButton, UnityAction Action)
        {
            if (TargetButton != null) TargetButton.onClick.AddListener(Action);
        }
    
    }
}