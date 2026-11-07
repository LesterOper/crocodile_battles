using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Dialogs.Merge;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using DG.Tweening;
using Library;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Tutorial
{
    public class HandPointer : AbstractPointerVariant
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private WeaponSetup _setup;
        [SerializeField] private Image iconTarget;
        [SerializeField] private SkeletonGraphic _skeletonGraphic;
    
        private Vector2 startPosition;
        private Transform _targetEnd;
        private bool anim;
        private CanvasGroup targetCanvasGroup;
        private bool isMerge;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _signalBus.Subscribe<FadeTutorPointerSignal>(Fade);
        }

        public override void Setup(WeaponType type, Transform targetPoition, Transform targetCard, bool isClickAnim, bool isMerge, bool isMirror = false)
        {
            base.Setup(type, targetPoition, targetCard, isClickAnim, isMerge);
            _targetEnd = targetPoition;
            this.isMerge = isMerge;

            /*if (targetCard.TryGetComponent(out Card cardObject))
            {
                startPosition = cardObject.GetStaterTransform().position;
            }
            else
            {*/
                if (isMerge)
                {
                    GameObject card = Instantiate(targetCard, transform).gameObject;
                    card.GetComponent<AbstractDrag>().enabled = false;
                    card.transform.SetAsFirstSibling();
                    List<Image> imageComponent = card.GetComponentsInChildren<Image>().ToList();

                    foreach (var image in imageComponent)
                    {
                        image.raycastTarget = false;
                    }
                    targetCanvasGroup = card.GetComponent<CanvasGroup>();
                    targetCanvasGroup.interactable = false;
                    RectTransform rectTransform =card.GetComponent<RectTransform>(); 
                    rectTransform.anchoredPosition = Vector2.zero;
                    card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, 0);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    startPosition = targetCard.position;
                    iconTarget.gameObject.SetActive(false);
                }
                else
                    startPosition = targetCard.GetComponentInChildren<AbstractDrag>().transform.position;
            //}
            iconTarget.transform.position = targetCard.position;
            transform.position = startPosition;
            _canvasGroup.DOFade(1, 0.2f).OnComplete(()=>
            {
                iconTarget.sprite = _setup.GetWeaponSprite(type);
                Animation();
            });
        }

        public override void Animation()
        {
            anim = true;
            StartCoroutine(Animate());
        }

        public override void Fade(FadeTutorPointerSignal signal)
        {
            base.Fade(signal);
            float alpha = signal.Value; 
            _canvasGroup.DOFade(alpha, 0.2f);
        }

        private IEnumerator Animate()
        {
            while (anim)
            {
                yield return new WaitForSeconds(0.2f);
                Tween tween;
                if (!isMerge)
                    tween = iconTarget.DOFade(1, 0.2f);
                else tween = targetCanvasGroup.DOFade(1, 0.2f);
                TrackEntry entry = _skeletonGraphic.AnimationState.SetAnimation(0, "pointDown", false);
                yield return entry.IsComplete;
                yield return new WaitForSeconds(0.3f);
                Tween tween1 = transform.DOMove(_targetEnd.position, 0.3f);
                yield return tween1.IsComplete();
                yield return new WaitForSeconds(0.4f);
                if (!isMerge)
                    tween = iconTarget.DOFade(0, 0.2f);
                else tween = targetCanvasGroup.DOFade(0, 0.2f);
                entry = _skeletonGraphic.AnimationState.SetAnimation(0, "pointUp", false);
                yield return entry.IsComplete;
                yield return new WaitForSeconds(0.3f);
                tween1 = transform.DOMove(startPosition, 0.3f);
                yield return tween1.IsComplete();
                yield return new WaitForSeconds(0.4f);
            }
        }
    }
}