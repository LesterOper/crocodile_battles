using UnityEngine.Events;

namespace App
{
    public static class CustomEventManager
    {
        public static UnityEvent<bool> ActivateCardsEvent = new UnityEvent<bool>();
        public static void InvokeActivateCardsEvent(bool active) => ActivateCardsEvent.Invoke(active);
        
        public static UnityEvent CompleteFightState = new UnityEvent();

        public static void InvokeCompleteFightState() => CompleteFightState.Invoke();
        
        public static UnityEvent PreparingWeaponEvent = new UnityEvent();

        public static void InvokePreparingWeaponEvent() => PreparingWeaponEvent.Invoke();
        
        public static UnityEvent ResettingWeaponEvent = new UnityEvent();
        public static void InvokeResettingWeaponEvent() => ResettingWeaponEvent.Invoke();
        
        public static UnityEvent BeforePreparingState = new UnityEvent();

        public static void InvokeBeforePreparingState() => BeforePreparingState.Invoke();
        public static UnityEvent AntiFreezeState = new UnityEvent();
    }
}
