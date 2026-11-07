using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace App.View
{
    [ExecuteInEditMode]
    public class CrocoItemsDOFader : MonoBehaviour
    {
        [Range(0, 1)] [SerializeField] private float alpha = 1;
        private List<SpriteRenderer> _renderers;
        private List<SkeletonAnimation> _skeletonAnimations = new List<SkeletonAnimation>();
        private LineRenderer _renderer;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
            _skeletonAnimations.AddRange( GetComponentsInChildren<SkeletonAnimation>());
            _renderer = GetComponentInChildren<LineRenderer>();
        }

        public void DOFadeCroco(float alpha, float duration, float delay)
        {
            DOFade(alpha, duration, delay);
        }

        private void DOFade(float alpha, float duration, float delay)
        {
            _renderers?.ForEach(render => render.DOFade(alpha, duration).SetDelay(delay));
            _skeletonAnimations?.ForEach(_skeletonAnimation => DOTween.To(() => _skeletonAnimation.Skeleton.A, value => _skeletonAnimation.Skeleton.A = value, alpha, duration).SetDelay(delay));
            if (_renderer != null)
            {
                _renderer.startColor = new Color(_renderer.startColor.r, _renderer.startColor.g, _renderer.startColor.b, alpha);
                _renderer.endColor = new Color(_renderer.endColor.r, _renderer.endColor.g, _renderer.endColor.b, alpha);
            }
        }
    
        private void OnValidate()
        {
            DOFadeCroco(alpha, 0, 0);
        }
    }
}
