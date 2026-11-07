using UnityEngine.Events;
using UnityEngine.UI;

namespace Library.CanvasNamespace
{
    public interface IPanelController
    {
        void SetupButton(Button TargetButton, UnityAction Action);
    }
}