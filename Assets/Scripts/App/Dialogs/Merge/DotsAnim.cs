using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.Dialogs.Merge
{
    public class DotsAnim : MonoBehaviour
    {
        [SerializeField] private List<Image> dots;
        [SerializeField] private GameObject effect;
        [SerializeField] private Sprite active;
        [SerializeField] private Sprite nonActive;

        [SerializeField] private CanvasGroup _canvasGroup;

        public void ActivateCoroutine(Action onAnimationComplete) => StartCoroutine(DotsAnimCoroutine(onAnimationComplete));
    

        private IEnumerator DotsAnimCoroutine(Action onAnimationComplete)
        {
            effect.SetActive(true);
            foreach (var DOT in dots)
            {
                DOT.sprite = active;
                effect.transform.DOMove(DOT.transform.position, 0.005f);
                DOT.SetNativeSize();
                yield return new WaitForSeconds(0.01f);
            }
            effect.SetActive(false);
            onAnimationComplete?.Invoke();
        }

        public void FadeOut()
        {
            _canvasGroup.DOFade(0, 0.3f);
            foreach (var DOT in dots)
            {
                DOT.sprite = nonActive;
                DOT.SetNativeSize();
            }
        }
        public void FadeIn()
        {
            _canvasGroup.DOFade(1, 0.3f);
        }
    
    }
}
