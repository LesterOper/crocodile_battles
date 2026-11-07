using System;
using App.Audio;
using App.DAO;
using App.Dialogs.Merge;
using App.SetupsScript;
using App.Signals;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Common;
using Library.View;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace App.Cards
{
    public class DragBathCard : AbstractDrag
    {
        private CardCharactaristicsUtils _charactaristicsUtils;
        private ICardInfo _bathData;
        private IDataService _dataService;
        private Transform target;
        private Transform contentPos;
        private GameObject clone;
        private BathView _view;
        private bool _ignoreCard;
        private bool isClone = false;
        private SignalBus _signalBus;
        private SoundPlayer _soundPlayer;

        [Inject]
        public void Construct(SignalBus signalBus, SoundPlayer soundPlayer, IDataService dataService)
        {
            _signalBus = signalBus;
            _soundPlayer = soundPlayer;
            _dataService = dataService;
        }
        protected override void Start()
        {
            base.Start();
            RecycleListView listView = GetComponentInParent<RecycleListView>();
            if(listView != null)
                contentPos = listView.gameObject.transform;
        }
        
        public override void Setup(ICardInfo data, bool clone, Transform target)
        {
            _charactaristicsUtils = new CardCharactaristicsUtils();
            this.target = target;
            _bathData = data;
            isClone = clone;
        }

        private void CloneCard()
        {
            clone = Instantiate(gameObject, parentWhileDragging);
            GetComponent<CanvasGroup>().DOFade(0.5f, 0.2f);
            RectTransform rect = clone.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition3D = new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y, 0);
            AbstractDrag drag = clone.GetComponent<AbstractDrag>();
            drag.enabled = false;
            //clone.GetComponent<Card>().Setup(_bathData);
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            if(!isClone)
                base.OnPointerDown(eventData);
            _soundPlayer.PlaySound("click_card_01", AudioMixerController.MasterMixerGroup.SFX, 0.3f);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!isClone)
            {
                var canvasGroup = GetComponentInParent<CanvasGroup>();
                canvasGroup.blocksRaycasts = true;
                _ignoreCard = false;

                if (dragBlocked) return;
                if (clone != null)
                {
                    if (Math.Abs(Vector2.Distance(clone.transform.position, target.transform.position)) < 3f)
                    {
                        Destroy(clone.gameObject);
                        BathData bath = new BathData(_bathData);
                        BathData old = new BathData(_dataService.GetBathData());
                        SetData(_bathData, old);
                        _signalBus.Fire(new RecreateCharacterOnMainSceneSignal(){NeedUpdateSlot = true});
                        _signalBus.Fire<UpdateItemsInListSignal>();
                        _signalBus.Fire(new UpdateBathSignal(){BathHP = _charactaristicsUtils.GetBathHPValueByLevel(bath.GetLevel(), (BathType)bath.GetCardType()), BathLvl = bath.GetLevel()});
                        clone = null;
                    }
                    else ResetCard();
                }
                _signalBus.Fire(new FadeTutorPointerSignal(){Value = 1f});
            }
        }

        private void SetData(ICardInfo bath, ICardInfo old)
        {
            _dataService.GetBathData().SetData(new BathData(_bathData));
            int index = _dataService.GetBathCollection().BathDatas.FindIndex(local => bath.GetId() == local.ID);
            _dataService.GetBathCollection().BathDatas.RemoveAt(index);
            _dataService.GetBathCollection().BathDatas.Add(new BathInCollectionData(old));
            _dataService.UpdateBathCollection();
            _dataService.UpdateBathData();
        }

        protected override void ResetCard()
        {
            GetComponent<CanvasGroup>().DOFade(1, 0.2f);
            Destroy(clone.gameObject);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!isClone)
            {
                float angle = Vector2.Angle(eventData.delta.normalized, Vector2.right);
                var canvasGroup = GetComponent<CanvasGroup>();
                if (!dragBlocked && DragAngle(angle))
                    CloneCard();
                else
                {
                    canvasGroup.blocksRaycasts = false;
                    _ignoreCard = true;
                }
                _signalBus.Fire(new FadeTutorPointerSignal(){Value = 0f});
            }
        }

        private bool DragAngle(float angle) => (angle >= 0 && angle <= 50) || (angle >= 130 && angle <= 230);

        public override void OnDrag(PointerEventData eventData)
        {
            if (!isClone)
            {
                if(Input.touchCount > 1)
                    OnPointerUp(eventData);
                if (_ignoreCard)
                {
                    contentPos.position += new Vector3(0, eventData.delta.y * Time.deltaTime * 0.25f, 0);
                    eventData.delta = Vector2.zero;
                    return;
                }

                if (!dragBlocked)
                    if(clone!=null)
                        clone.transform.position = GetMouseWorldPos() + mOffset;
            }
        }
    }
}