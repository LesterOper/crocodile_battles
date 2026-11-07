using System.Collections;
using DG.Tweening;
using Library;
using UnityEngine;
using UnityEngine.UI;

namespace App.View
{
    public class YoYoEffect : MonoBehaviour
    {
        [SerializeField] private Vector2 _delayRandomBetween = new Vector2(0,0);
        [SerializeField] private bool _reversed;
        [SerializeField] private float _amplitude = 0.05f;
        [SerializeField] private float _duration = 3f;
        [SerializeField] private Ease _easeMove= Ease.InOutSine;
        [SerializeField] private bool _withScale ;
#if UNITY_EDITOR
        [ConditionalHide("_withScale", true)]
#endif
        [SerializeField] private float _maxScale;
        
        [SerializeField] private bool _withAlpha;
#if UNITY_EDITOR
        [ConditionalHide("_withAlpha", true)]
#endif
        [SerializeField] private float _endAlpha;
#if UNITY_EDITOR
        [ConditionalHide("_withAlpha", true)]
#endif
        [SerializeField] private Ease _ease;
        private bool _playing;
        
        private Sequence _sequence;

        private void OnEnable()
        {
            _playing = true;
            StartCoroutine(DelayedCall());
        }

        private void OnDisable()
        {
            transform.DOScale(1, 0);
            _sequence.Kill();
            _playing = false;
        }

        private IEnumerator DelayedCall()
        {
            var delay = UnityEngine.Random.Range(_delayRandomBetween.x, _delayRandomBetween.y);
            yield return new WaitForSeconds(delay);
            if (_playing)
            {
                Vector3 startPos = transform.position;
                _sequence = DOTween.Sequence();
                if (_reversed)
                {
                    if(_amplitude!=0)
                        _sequence.Append(transform.DOMoveY((startPos.y - _amplitude), _duration).SetEase(_easeMove));
                    if (_withScale)
                    {
                        _sequence.Join(transform.DOScale(_maxScale, _duration));
                    }

                    if (_withAlpha)
                    {
                        _sequence.Join(transform.GetComponent<Image>().DOFade(_endAlpha, _duration)).SetEase(_ease);
                    }
                }
                else
                {
                    if(_amplitude!=0)
                        _sequence.Append(transform.DOMoveY((startPos.y + _amplitude), _duration).SetEase(_easeMove));
                    if (_withScale)
                    {
                        _sequence.Join(transform.DOScale(_maxScale, _duration));
                    }
                    if (_withAlpha)
                    {
                        _sequence.Join(transform.GetComponent<Image>().DOFade(_endAlpha, _duration)).SetEase(_ease);
                    }
                }
        
                _sequence.SetLoops(-1, LoopType.Yoyo); 
            }
        }
    }
}
