using System;
using System.Linq;
using App;
using App.AddressablesManager.Addressables;
using App.AddressablesTest;
using App.Dialogs.Merge;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Weapon.WeaponBases;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Utils;
using UnityEngine;
using Zenject;

public class Card : MonoBehaviour
{
    #region SerializedFields

    [SerializeField] private CardView cardView;
    [Inject] private BathSetup _bathSetup;
    [Inject] private WeaponSetup _weaponSetup;
    [Inject] private IGameInfoService _gameInfoService;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AbstractDrag drag;
    [SerializeField] private CardCharacteristicViewHandler handler;
    
    #endregion

    #region PrivateFields

    private BathView _bath;
    private ICardInfo _card;
    private WeaponView _view;
    private BathCardHandler _bathCardHandler;
    private CardStateHandler _cardStateHandler;
    private WeaponCardHandler _weaponCardHandler;
    
    #endregion

    #region Properties

    public BathCardHandler BathCardHandlerProperty => _bathCardHandler;
    public CardStateHandler CardStateHandlerProperty => _cardStateHandler;
    public WeaponCardHandler WeaponCardHandlerProperty => _weaponCardHandler;

    #endregion

    public void Setup( ICardInfo cardInfo)
    {
        _card = cardInfo;
        _cardStateHandler = new CardStateHandler(this);
        if (_card.IsBathCard())
        {
            _bathCardHandler = new BathCardHandler(this);
            _bathCardHandler.Setup();
        }
        else
        {
            _weaponCardHandler = new WeaponCardHandler(this);
            _weaponCardHandler.Setup();
        }
        gameObject.SetActive(true);
        cardView.gameObject.SetActive(true);
        cardView.EnableIconBack(false);
        canvasGroup.DOFade(1, 0.3f);
    }

    private void Interactable(bool block)
    {
        if(drag!=null)
            drag.BlockedDrag(block);
    }
    
    public void ActivateNewCardIcon()
    {
        if(_card.IsBathCard())
            cardView.ActivateNewCardIcon(_gameInfoService.GetNewBathsCardInfo().Exists(bath => bath.ID == _card.GetId() /*.Equals(_card)*/));
        else
            cardView.ActivateNewCardIcon(_gameInfoService.GetNewCardsInfo()
                .Exists(card => card.GetId() == _card.GetId() && !card.IsInUseForBattle() /*..Equals(_card)*/));

    }

    private void OnDisable()
    {
        cardView.ActivateNewCardIcon(false);
    }

    public void SetDrag(bool activate, Transform target =null, Action onDragCompleteSuccessfully = null)
    {
        if (target == null) drag.enabled = activate;
        else drag.Setup(_card,target, onDragCompleteSuccessfully);
        cardView.EnableIconBack(false);
    }

    public void SetMergeDrag(MergePlace left, MergePlace right)
    {
        if (drag == null) return;
        drag.Setup(_card, left, right);
        drag.SetupSetups(_bathSetup, _weaponSetup);
        cardView.gameObject.SetActive(true);
        cardView.EnableIconBack(false);
    }
    
    public void Show(float delay)
    {
        bool isLeft = false;

        if (cardView != null) isLeft = cardView.transform.position.x < 2;
        
        cardView.gameObject.SetActive(true);
        Transform cardViewTransform = cardView.transform;
        Vector3 position = cardViewTransform.position;
        position = new Vector3(position.x - delay*12 * (isLeft?1:-1), position.y-5, position.z);
        cardViewTransform.position = position;
        cardView.transform.rotation = Quaternion.Euler(0, -90, 0);
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(cardView.transform.DOLocalMove(Vector3.zero, 0.35f).SetEase(Ease.OutSine))
            .Join(cardView.transform.DOLocalRotate(new Vector3(0, 180f, 0), 0.35f)
                .SetDelay(0.15f));
    }

    public void Rotate()
    {
        cardView.transform.DOLocalRotate(new Vector3(0, -90, 0), 0.1f).OnComplete(() =>
        {
            _cardStateHandler.ShowCardFront();
        });
        cardView.transform.DOLocalRotate(new Vector3(0, -180, 0), 0.1f).SetDelay(0.1f);
        cardView.transform.DOPunchScale(cardView.transform.localScale * 1.3f, 0.7f, 10, 0);
    }

    public void ChangeDragState(bool active) => drag.enabled = active;

    public class BathCardHandler
    {
        private readonly Card _cardComponent;

        public BathCardHandler(Card card)
        {
            _cardComponent = card;
        }
        
        public void Setup()
        {
            _cardComponent.handler.Setup(_cardComponent._card);
            _cardComponent.cardView.SetCardIcon(_cardComponent._bathSetup.GetBathByType((BathType)_cardComponent._card.GetCardType()).GetSprite());
            _cardComponent.cardView.SetBathBorder(_cardComponent._card.GetLevel());
            _cardComponent.cardView.EnableIconBack(false);
        }
        
        public void SetBathDrag(ICardInfo cardInfo, Transform target, bool clone)
        { 
            if (target == null) _cardComponent.drag.enabled = false;
            else _cardComponent.drag.Setup(cardInfo, clone, target);
        }
    }

    public class WeaponCardHandler
    {
        private readonly Card _cardComponent;

        public WeaponCardHandler(Card card)
        {
            _cardComponent = card;
        }
        
        public void Setup()
        {
            _cardComponent._view = _cardComponent._weaponSetup.GetWeaponView((WeaponType)_cardComponent._card.GetCardType());
            _cardComponent.handler.Setup(_cardComponent._card, _cardComponent._view.RoundDuration, _cardComponent._view.Mana);
            _cardComponent.cardView.SetCardIcon(_cardComponent._view.GetSprite());
            _cardComponent.cardView.SetBorder(_cardComponent._card.GetLevel()
                , ((WeaponType)_cardComponent._card
                    .GetCardType()));
        }

        public void SetBattleDrag(WeaponView weaponView, ICardInfo cardInfo, WeaponEquipingHandler weaponEquipingHandler)
        {
            _cardComponent.drag.Setup(weaponView, cardInfo, weaponEquipingHandler);
        }
    }

    public class CardStateHandler
    {
        private readonly Card _cardComponent;
        private bool _isCardUsed;
        public CardStateHandler(Card cardComponent)
        {
            _cardComponent = cardComponent;
        }
        
        public void UpdateCardUsing(int mana)
        {
            if (_cardComponent._view.Mana <= mana)
            {
                _cardComponent.drag.SetMana(true);
                _cardComponent.cardView.ChangeStateOfParticles(true);
                _cardComponent.handler.SetupManaCost(true);
            }
            else
            {
                _cardComponent.drag.SetMana(false);
                _cardComponent.cardView.ChangeStateOfParticles(false);
                _cardComponent.handler.SetupManaCost(false);
            }
        }

        public bool IsUsed() => _isCardUsed;

        public void SetCardUsed(bool used) => _isCardUsed = used;

        public void ShowCardFront() => _cardComponent.cardView.ChangeCardState(true);
        public void ShowBackside() => _cardComponent.cardView.ChangeCardState(false);

        public void ChangeActivationStatePArticle(bool active) =>
            _cardComponent.cardView.ChangeStateOfParticles(active);

        public void SetCardPosition(Vector2 rectPos)
        {
            _cardComponent.cardView.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }
    public ICardInfo GetCardInfo() => _card;
}

public class CardCharactaristicsUtils
{
    public int GetAttackValueByLevel(int level, WeaponType weaponType) => 
        (int) Math.Round(weaponType.GetAttribute<WeaponData>().BaseAttackProperty * Math.Pow(1.18f, level), MidpointRounding.AwayFromZero);

    public long GetArmorValueByLevel(int level, WeaponType weaponType) => 
        (int)Math.Round(weaponType.GetAttribute<WeaponData>().ArmorProperty * Math.Pow(1.18, level), MidpointRounding.AwayFromZero);

    public int GetHealthPointValueByLevel(int level, WeaponType weaponType) => 
        (int) (weaponType.GetAttribute<WeaponData>().BaseHealthPointProperty * (Math.Pow(1.18f, level)));

    public int GetHealingPoint(int level, WeaponType weaponType) =>
        (int) Math.Round(weaponType.GetAttribute<WeaponData>().HealProperty * Mathf.Pow(1.18f, level), MidpointRounding.AwayFromZero);

    public long GetBathHPValueByLevel(int level, BathType bathType, bool isPlayer = false)
    {
        return (int) Math.Round(bathType.GetAttribute<BathAttribute>().HP * Math.Pow(1.18f, level), MidpointRounding.AwayFromZero);
    }
}