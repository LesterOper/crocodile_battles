using System.Collections.Generic;
using App;
using App.Audio;
using App.Cards;
using App.Crocodile.Fasteners;
using App.DAO;
using App.Dialogs.Merge;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using App.Weapon.WeaponBases;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.CanvasNamespace;
using Library.Common;
using Library.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Battle
{
    public class WeaponStater : AbstractDrag,  IStater
    {
        [SerializeField] private Image PreviewBack;
        [SerializeField] private RectTransform card;
        private WeaponEquipingHandler equipingHandler;
        private RectTransform _rectTransform;
        private IDataService _dataService;
        private WeaponView weaponView ;
        private IFastener TargetFastener = null;
        private Transform _parentForDrag;
        private Transform _contentPos;
        private Transform _parent;
        private Card _cardObject;
        private SoundPlayer _soundPlayer;
        private ICardInfo _cardInfo;
        private Image OurImage;
        private WeaponActionType weaponViewActionType;
        private Vector3 _fastenerStartScale;
        private Vector3 _cardStartScale;
        private Vector3 _startPreviewScale;
        private Vector3 _startPreviewPosition;
        private Vector2 contentStartPos;
        private Vector2 startPos = Vector2.zero;
        private int _childCount;
        private int level;
        private SignalBus _signalBus;
        private bool _isBombEquipped;
        private bool _ignoreCard;
        private bool ActivateDrag { set; get; }
        private bool Mana { get; set; }

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService, SoundPlayer soundPlayer)
        {
            _signalBus = signalBus;
            _dataService = dataService;
            _soundPlayer = soundPlayer;
        }
        
        protected override void Start()
        {
            _cardObject = gameObject.GetComponentInParent<Card>();
            _startPreviewPosition = GetComponent<RectTransform>().anchoredPosition;
            _camera = Camera.main;
            _parent = transform.parent;
            _startPreviewScale = transform.localScale;
            if(SceneManager.GetActiveScene().name == "BattleScene")
                _parentForDrag = FindObjectOfType<PlayerCardsHandler>().transform;
            PreviewBack.sprite = (OurImage = GetComponent<Image>()).sprite;
            _rectTransform = GetComponent<RectTransform>();
            startPos = _rectTransform.anchoredPosition;
            _childCount = GetCardsCount();
            if(equipingHandler != null)
                _fastenerStartScale = equipingHandler.GetBathFastenerHandler().GetFasteners()[0].transform.localScale;
            _cardStartScale = card.transform.localScale;
            _contentPos = GetComponentInParent<HorizontalLayoutGroup>()?.transform;
            Mana = true;
        }

        public override void SetMana(bool state) => Mana = state;
        public void SetActivateDrag(bool state) => ActivateDrag = state;
        

        public override void Setup(WeaponView weaponView, ICardInfo cardInfo, WeaponEquipingHandler equipingHandler)
        {
            base.Setup(weaponView, cardInfo, equipingHandler);
            
            this.weaponView = weaponView;
            this.level = cardInfo.GetLevel();
            _cardInfo = cardInfo;
            weaponViewActionType = ((WeaponType) _cardInfo.GetCardType()).GetAttribute<WeaponData>().ActionType;
            this.equipingHandler = equipingHandler;
        }

        public void SetTargets(WeaponEquipingHandler handler) => equipingHandler = handler;
        
        private void CheckDistance()
        {
            float Distance = 1;
            float conditionDistance = 0;

            if (weaponView.Type.GetAttribute<WeaponData>().IsNeedFastener)
            {
                foreach (IFastener Target in equipingHandler.GetBathFastenerHandler().GetFasteners())
                {
                    if (!Target.IsFreeFastener() ||
                        Target.GetActionType() != weaponViewActionType)
                    {
                        continue;
                    }

                    float _distance = Vector2.Distance(transform.position, Target.GetFastenerTransform().position);
                    if (_distance <= Distance)
                    {
                        Distance = _distance;
                        conditionDistance = 1;
                        TargetFastener = Target;
                    }
                }
            }
            else
            {
                float distance = 0;
                conditionDistance = 1f;
                Vector3 targetPosition = equipingHandler.GetFastenerByWeaponActionType(weaponViewActionType);
                distance = Vector2.Distance(transform.position, targetPosition);
    
                if (distance >= Distance)
                    Distance = distance;
            }

            var pos = (card.rect.height / 2) + startPos.y;
            if (Distance <= conditionDistance)
            {
                EquipWeapon();
            }
            else if (_rectTransform.anchoredPosition.y - pos > 0)
            {
                if (weaponView.Type.GetAttribute<WeaponData>().IsNeedFastener)
                {
                    Fastener freeFastener = equipingHandler.GetBathFastenerHandler().GetFasteners()
                        .Find(local => local.GetActionType() == weaponViewActionType && local.IsFreeFastener());
                    TargetFastener = freeFastener;
                    if (TargetFastener != null)
                    {
                        EquipWeapon();
                    }
                    else ResetWeapon();
                }
                else EquipWeapon();
            }
            else
            {
                ResetWeapon();
            }
        }

        private void ResetWeapon()
        {
            PreviewBack.DOFade(1f, 0.3f);
            gameObject.transform.SetParent(_parent);
            gameObject.transform.localScale = _startPreviewScale;
            card.DOScale(card.localScale / 1.05f, 0.2f);
            OurImage.DOFade(0, 0.3f).OnComplete(() => OurImage.DOFade(1, 0));
            transform.DOLocalMove(PreviewBack.transform.localPosition, 0.3f);
            _signalBus.Fire(new FadeTutorPointerSignal(){Value = 1f});
        }

        private void EquipWeapon()
        {
            if (equipingHandler.IsBombEquipped() && ((WeaponType)_cardInfo.GetCardType() == WeaponType.BOMB_ILLNESS ||
                                                     (WeaponType)_cardInfo.GetCardType() == WeaponType.MINA_BOMB))
            {
                ResetWeapon();
                return;
            }

            _soundPlayer.PlaySound("card_gun_01", AudioMixerController.MasterMixerGroup.SFX, 0.4f);
            gameObject.transform.SetParent(_parent); 
            gameObject.transform.localScale = _startPreviewScale;
            gameObject.GetComponent<RectTransform>().anchoredPosition = _startPreviewPosition;
            card.transform.SetAsLastSibling();
            card.gameObject.SetActive(false);
            Card handCard = card.GetComponent<Card>();

            handCard.CardStateHandlerProperty.SetCardUsed(true);
            _signalBus.Fire(new FoldCardSignal()
            {
                CardInfo = handCard.GetCardInfo()
            });
            equipingHandler.EquipWeapon(_cardInfo, _cardObject,
                weaponView.Type.GetAttribute<WeaponData>().IsNeedFastener ? TargetFastener : null, level);
            
            
            if(_dataService.GetTutorialCurrentStep() == TutorialStepType.BATTLE_TUTORIAL_END)
                return;
            
            if (_dataService.GetTutorialCurrentStep().GetAttribute<TutorialAttribute>().NextStep.GetAttribute<TutorialAttribute>().IsMaskNeed)
            {
                _signalBus.Fire<CompleteTutorialSignal>();
                _signalBus.Fire<TutorialViewPointerSignal>();
            }
            else
            {
                _signalBus.Fire<CompleteTutorialSignal>();
                _signalBus.Fire<ShowTutorialSignal>();
            }
        }

        private int GetCardsCount()
        {
            var allCards = _dataService.GetCardCollectionData().Collection.FindAll(card =>
                card.WeaponType.GetAttribute<WeaponData>().FilterType != FilterType.BATH);
            List<BattleCardInfo> cards = new List<BattleCardInfo>(_dataService.GetBattleCardInfoData().CardInfos);
            foreach (var card in cards)
            {
                allCards.Add(new CardInfo(){ID = card.ID, type = card.type, Level = card.Level, WeaponType = card.WeaponType});
            }
            //allCards.AddRange(_dataService.GetBattleCardInfoData().CardInfos);
            return allCards.Count;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            /*if(_cardObject.WeaponEquipped)
                return;*/
            /*if(Input.touchCount > 1)
                return;*/
            if (Mana)
            {
                transform.position = GetMouseWorldPos() + mOffset;
            }
            
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            //if (ActivateDrag && Mana)
            {
                CheckDistance();
                equipingHandler.GetBathFastenerHandler().GetFasteners().ForEach(obj =>
                {
                    if(obj.GetActionType() == weaponViewActionType)
                        obj.GetFastenerTransform().DOScale(_fastenerStartScale, 0.2f);
                });
                card.DOScale(_cardStartScale, 0.5f);
            }
            
            /*else
            {
                ActivateDrag = true;
            }*/
        }

        
        
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            /*if(_cardObject.WeaponEquipped)
                return;*/
            
            /*if(Input.touchCount > 1)
                return;*/
            
            mZCoord = _camera.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWorldPos();
            _soundPlayer.PlaySound("click_card_02", AudioMixerController.MasterMixerGroup.SFX, 0.5f);

        }

        protected override  Vector3 GetMouseWorldPos()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = mZCoord+1000;
            return _camera.ScreenToWorldPoint(mousePoint);
        }
        
        public void SetWeaponActionType(WeaponActionType weaponViewWeaponActionType) =>
            weaponViewActionType = weaponViewWeaponActionType;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            /*if(_cardObject.WeaponEquipped)
                return;*/
            /*if(Input.touchCount > 1)
                return;*/
            
            if (Mana)
            {
                equipingHandler.GetBathFastenerHandler().GetFasteners().ForEach(obj =>
                {
                    if(obj.IsFreeFastener() && obj.GetActionType() == weaponViewActionType)
                        obj.GetFastenerTransform().DOScale(new Vector3(obj.GetFastenerTransform().localScale.x * 1.2f, obj.GetFastenerTransform().localScale.y * 1.2f, obj.GetFastenerTransform().localScale.z), 0.2f);
                });        
            
                PreviewBack.DOFade(0.5f, 0);
                card.DOScale(card.localScale * 1.05f, 0.5f);
                gameObject.transform.SetParent(_parentForDrag); 
                _signalBus.Fire(new FadeTutorPointerSignal(){Value = 0f});
            }
            
        }

        protected override void ResetCard()
        {
            
        }
    }
}