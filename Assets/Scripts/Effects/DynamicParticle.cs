using App;
using App.Signals;
using Library;
using UnityEngine;
using Zenject;

namespace Effects
{
    public class DynamicParticle : MonoBehaviour
    {
        [SerializeField] private GameObject calmEffects;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<MoveEffectSignal>(ShowEffect);
        }

        private void ShowEffect(MoveEffectSignal signal)
        {
            calmEffects.SetActive(signal.ShowEffect);
        }

        private void Start()
        {
            calmEffects.SetActive(true);
            transform.localScale = transform.parent.localScale;
            _signalBus.Subscribe<MoveEffectSignal>(ShowEffect);
        }
        
    }
}