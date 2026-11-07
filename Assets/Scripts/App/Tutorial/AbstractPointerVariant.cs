using App.PrototypeBattle;
using App.Signals;
using DG.Tweening;
using Library;
using UnityEngine;
using UnityEngine.UI;

namespace App.Tutorial
{
    public abstract class AbstractPointerVariant : MonoBehaviour
    {
        [SerializeField] protected Image icon;
        private Transform target;

        protected virtual void Awake()
        {

        }

        public virtual void Setup(Transform target, bool isMirror = false)
        { }

        public virtual void Setup(WeaponType type, Transform targetPosition, Transform targetCard, bool isClickAnim, bool isMerge, bool isMirror = false){}
    
        public abstract void Animation();

        public void Fade(float alpha = 0, float duration = 0.2f)
        {
            icon.DOFade(alpha, duration);
        }
    
        public virtual void Fade(FadeTutorPointerSignal signal){}
    }
}