using System;
using App.Audio;
using App.Cards;
using App.DAO;
using App.Dialogs.Merge;
using App.Signals;
using DG.Tweening;
using Effects;
using Library;
using Library.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.BeforeBattleStackCards
{
    public class DragCardsBeforeBattle : AbstractDrag
    {
        private MovePlayerMainScene _playerMainScene;
        private GameObject clone;
        private Transform target;
        private Transform spawnParent;
        private ICardInfo _cardInfo;
        private bool _ignoreCard;
        private DeckUtils _deckUtils;
        private SignalBus _signalBus;
        private Action _onDragCompleteSuccessfully;
        private SoundPlayer _soundPlayer;
        private IDataService _dataService;
        [Inject]
        public void Construct(SignalBus signalBus, SoundPlayer soundPlayer, IDataService dataService)
        {
            _signalBus = signalBus;
            _soundPlayer = soundPlayer;
            _dataService = dataService;
        }
        private void CloneCard()
        {
            spawnParent = parentWhileDragging.parent.parent.parent.parent;
            clone = Instantiate(gameObject, spawnParent);
            clone.transform.localScale = new Vector3(clone.transform.localScale.x * -1, clone.transform.localScale.y,clone.transform.localScale.z);
            GetComponent<Image>().DOFade(0.5f, 0.2f);
        }

        public override void Setup(ICardInfo info, Transform targetLocal, Action onDragCompleteSuccessfully)
        {
            _cardInfo = info;
            target = targetLocal;
            _onDragCompleteSuccessfully = onDragCompleteSuccessfully;
            _deckUtils = new DeckUtils(_dataService);
            enabled = true;
        }
    
        public override void OnDrag(PointerEventData eventData)
        {
            if(clone != null)
                clone.transform.position = GetMouseWorldPos() + mOffset;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            var canvasGroup = GetComponentInParent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
            _ignoreCard = false;
            if (!dragBlocked && DeckUtils.CardLimit >= _deckUtils.GetDeckSize() + 1)
            {
                if (clone != null)
                {
                    var dist = Vector2.Distance(target.transform.position, clone.transform.position);
                    if (dist < 3f)
                    {
                        int index = _dataService.GetCardCollectionData().Collection
                            .FindIndex(local => local.Equals(_cardInfo));
                        _dataService.GetCardCollectionData().Collection.RemoveAt(index);
                        _dataService.GetBattleCardInfoData().CardInfos.Add(new BattleCardInfo(_cardInfo));
                        _dataService.UpdateCardCollectionData();
                        _dataService.UpdateBattleCardInfoData();
                        _onDragCompleteSuccessfully?.Invoke();
                        _signalBus.Fire<UpdateItemsInListSignal>();
                        Destroy(clone);
                        return;
                    }

                    ResetCard();
                }
            }
            else ResetCard();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            float angle = Vector2.Angle(eventData.delta.normalized, Vector2.right);
            var canvasGroup = gameObject.GetComponentInParent<CanvasGroup>();

            if (!dragBlocked && angle > 15 && angle < 165)
            {
                CloneCard();
            }
            else
            {
                canvasGroup.blocksRaycasts = false;
                _ignoreCard = true;
            }
            _signalBus.Fire(new FadeTutorPointerSignal(){Value = 0f});
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (_cardInfo == null)
            {
                this.enabled = false;
                return;
            }

            base.OnPointerDown(eventData);
            _soundPlayer.PlaySound("click_card_01", AudioMixerController.MasterMixerGroup.SFX, 0.3f);
        }

        protected override void ResetCard()
        {
            GetComponent<Image>().DOFade(1, 0.2f);
            Destroy(clone);
        }
    }
}