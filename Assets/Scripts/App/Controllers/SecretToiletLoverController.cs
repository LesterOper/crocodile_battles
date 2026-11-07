using DG.Tweening;
using UnityEngine;

namespace App.Controllers
{
    public class SecretToiletLoverController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private Tween tween;
   
        void Start()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(target.position, 6f).SetEase(Ease.InOutSine));
            sequence.SetLoops(-1, LoopType.Restart);
            sequence.SetDelay(2.5f, true);
        }
    
    }
}
