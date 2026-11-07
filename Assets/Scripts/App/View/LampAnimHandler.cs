using DG.Tweening;
using UnityEngine;

namespace App.View
{
    public class LampAnimHandler : MonoBehaviour
    {
        [SerializeField] public bool reverseAnim;
        [SerializeField] private SpriteRenderer _renderer;
   
        private void Start()
        {
            var startRotation = reverseAnim ? -8f : 8f;
            transform.eulerAngles = new Vector3(0f,0f,-startRotation);
            Vector3 rot = new Vector3(0, 0, startRotation);
            Sequence sequence1 = DOTween.Sequence();
            sequence1.Append(_renderer.DOFade(0, Random.Range(0.1f, 0.5f)));
            sequence1.Append(_renderer.DOFade(1, Random.Range(0.1f,0.5f)));
            sequence1.SetDelay(Random.Range(1, 4)).SetLoops(-1);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DORotate(rot, 5f));
            sequence.Append(transform.DORotate(rot * -1, 5f));
            sequence.SetEase(Ease.InOutSine).SetLoops(-1);
        }
    }
}
