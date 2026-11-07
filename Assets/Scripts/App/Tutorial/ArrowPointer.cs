using Spine.Unity;
using UnityEngine;

namespace App.Tutorial
{
    public class ArrowPointer : AbstractPointerVariant
    {
        [SerializeField] private SkeletonGraphic _graphic;
        public override void Setup(Transform target, bool isMirror = false)
        {
            transform.parent.SetParent(target);
            Animation();
        }
    
        public override void Animation()
        {
            _graphic.AnimationState.SetAnimation(0, "click", true);
        }
    }
}