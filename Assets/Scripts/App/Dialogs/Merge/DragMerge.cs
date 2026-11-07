using App.Audio;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using DG.Tweening;
using Library;
using Library.Common;
using Library.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace App.Dialogs.Merge
{
    public class DragMerge : AbstractDrag
    {
        private IGridMergeHandler gridHandler;
        private MergePlace targetLeft, targetRight;
        private WeaponView _view;
        private ICardInfo _cardInfo;
        private GameObject clone;
        private BathView _bathView;
        private ICardInfo _bathData;
        private bool isBathMerge;
        
        private SignalBus _signalBus;
        private SoundPlayer _soundPlayer;
        [Inject]
        public void Construct(SignalBus signalBus, SoundPlayer soundPlayer)
        {
            _signalBus = signalBus;
            _soundPlayer = soundPlayer;
        }

        private void CloneCard()
        {
            clone = Instantiate(gameObject, parentWhileDragging);
            GetComponent<CanvasGroup>().DOFade(0.4f, 0.1f);
            clone.GetComponent<AbstractDrag>().enabled = false;
            clone.GetComponent<Card>().Setup(_cardInfo);
        }

        public override void Setup(ICardInfo cardInfo, MergePlace left, MergePlace right)
        {
            isBathMerge = cardInfo.IsBathCard();
            _cardInfo = cardInfo;
            targetLeft = left;
            targetRight = right;
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (clone != null)
            {
                if (Vector2.Distance(clone.transform.position, targetLeft.gameObject.transform.position) < 1f)
                {
                    if (targetLeft.IsFree())
                    {
                        TargetPlace(targetLeft);
                        return;
                    }
                }

                if (Vector2.Distance(clone.transform.position, targetRight.gameObject.transform.position) < 1f)
                {
                    if (targetRight.IsFree())
                    {
                        TargetPlace(targetRight);
                        return;
                    }

                }
                ResetCard();
                ResetPlaces();
            }
            _signalBus.Fire(new FadeTutorPointerSignal(){Value = 1f});
        }

        private void ResetPlaces()
        {
            if (targetLeft.gameObject.transform.childCount == 0)
            {
                targetLeft.SetFree();
            }
        
            if (targetRight.gameObject.transform.childCount == 0)
            {
                targetRight.SetFree();
            }
        }

        protected override void ResetCard()
        {
            if(clone != null)
                Destroy(clone.gameObject);
            GetComponent<CanvasGroup>().DOFade(1, 0.2f);
        }

        private void TargetPlace(MergePlace target)
        {
            if (target.IsFree())
            {
                WeaponCardPutted(target);
            }
            else ResetCard();
        }

        private void WeaponCardPutted(MergePlace target)
        {
            target.Setup(clone.GetComponent<Card>(), _cardInfo);
            SetCardProperties(target.transform);
            clone = null;
            _signalBus.Fire<CompleteTutorialSignal>();
            _signalBus.Fire(new TutorialSendTargetSignal(){FreeMergePlace = false});
        }

        private void SetCardProperties(Component target)
        {
            clone.transform.DOMove(target.transform.position, 0.2f);
            if(isBathMerge)
                clone.transform.DOScale( 0.8f, 0.3f);
            else if(((WeaponType)_cardInfo.GetCardType()).GetAttribute<WeaponData>().FilterType == FilterType.ONE_ACTION)
                clone.transform.DOScale( 0.8f, 0.3f);
            else 
                clone.transform.DOScale( 0.8f, 0.3f);
        }

        public override void OnDrag(PointerEventData eventData) => clone.transform.position = GetMouseWorldPos() + mOffset;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _soundPlayer.PlaySound("click_card_01", AudioMixerController.MasterMixerGroup.SFX, 0.3f);
            _signalBus.Fire(new FadeTutorPointerSignal(){Value = 0f});
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            CloneCard();
        }
    }
}