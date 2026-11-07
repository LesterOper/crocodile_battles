using System;
using App;
using App.Audio;
using App.Cards;
using App.DAO;
using App.Dialogs.Merge;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using Library;
using Library.Common;
using Library.View;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class DeckCard: AbstractDrag, IPointerClickHandler
{
    [SerializeField] private Card normalCard;
    [SerializeField] private Image back;
    private float _lastTimeClick;
    private WeaponView _weaponView;
    private ICardInfo _cardInfo;
    private IDataService _dataService;
    private RectTransform _cloneRect;
    private Transform _contentPos;
    private bool _ignoreCard;
    private Vector3 _startCardPos;
    private Transform _cardParent;
    private bool _isOnBattleCollection = false;
    private Action _onDragCompleteSuccessfully;
    private SignalBus _signalBus;
    private SoundPlayer _soundPlayer;
    private IGameInfoService _infoService;

    [Inject]
    public void Construct(SignalBus signalBus, SoundPlayer soundPlayer, IDataService dataService, IGameInfoService infoService)
    {
        _signalBus = signalBus;
        _soundPlayer = soundPlayer;
        _dataService = dataService;
        _infoService = infoService;
    }
    
    protected override void Start()
    {
        base.Start();
        _contentPos = gameObject.GetComponentInParent<RecycleListView>(true)?.transform;
        _cardParent = transform.parent;
    }
    public GameObject GetObject() => gameObject;
    public void Setup(WeaponSetup setup, ICardInfo cardInfo, Transform target, bool isBattleCollection, Action onDragCompleteSuccessfully)
    {
        _weaponSetup = setup;
        _isOnBattleCollection = isBattleCollection;
        _lastTimeClick = Single.MinValue;
        _cardInfo = cardInfo;
        _weaponView = _weaponSetup.GetWeaponView((WeaponType)cardInfo.GetCardType());
        if (_weaponView == null)
        {
            back.gameObject.SetActive(true);
            return;
        }

        _onDragCompleteSuccessfully = onDragCompleteSuccessfully;
        normalCard.Setup( cardInfo);
        /*if(isBattleCollection)
            normalCard.SetDrag(false, null, onDragCompleteSuccessfully);*/
         normalCard.SetDrag(false, target, onDragCompleteSuccessfully);
        //normalCard.GetComponent<CanvasGroup>().DOFade(1, 0);
        normalCard.GetComponentInChildren<AbstractDrag>().enabled = false;
        back.gameObject.SetActive(false);
    }
    
    
    public void OnPointerClick(PointerEventData eventData)
    {
        dragBlocked = true;
        float currentTimeClick = eventData.clickTime;
        if (Mathf.Abs(currentTimeClick - _lastTimeClick) < .75f)
        {
            RemoveCardFromDeck();
        }
            
        _lastTimeClick = currentTimeClick;
    }

    private void RemoveCardFromDeck()
    {
        if (_isOnBattleCollection)
        {
            int index = _dataService
                .GetBattleCardInfoData().CardInfos
                .FindIndex(local => local.ID == _cardInfo.GetId() /*local.WeaponType == (WeaponType)_cardInfo.GetCardType()*/);
            _dataService.GetBattleCardInfoData().CardInfos.RemoveAt(index);
            _dataService.GetCardCollectionData().Collection
                .Add(new CardInfo(_cardInfo));
            _dataService.UpdateCardCollectionData();
            _dataService.UpdateBattleCardInfoData();
            _signalBus.Fire<UpdateItemsInListSignal>();
            _lastTimeClick = Single.MinValue;
        }
        else
        {
            int index = _dataService
                .GetCardCollectionData().Collection
                .FindIndex(local => local.ID == _cardInfo.GetId() /*local.WeaponType == (WeaponType)_cardInfo.GetCardType()*/);
            _dataService.GetCardCollectionData().Collection.RemoveAt(index);
            _dataService.GetBattleCardInfoData().CardInfos
                .Add(new BattleCardInfo(new CardInfo(_cardInfo)));
            _dataService.UpdateCardCollectionData();
            _dataService.UpdateBattleCardInfoData();
            _onDragCompleteSuccessfully?.Invoke();
            _signalBus.Fire<UpdateItemsInListSignal>();
            _lastTimeClick = Single.MinValue;
        }
        
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        var canvasGroup = GetComponentInParent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
        _ignoreCard = false;
        
        var dist = Vector2.Distance(transform.position, _startCardPos);
        switch (dragBlocked)
        {
            case false when dist > 1.3f:
                transform.SetParent(_cardParent);
                transform.position = _startCardPos;
                if (_dataService.GetTutorialCurrentStep() == TutorialStepType.FIRST_WEAPON_POINTER)
                {
                    _signalBus.Fire<CompleteTutorialSignal>();
                    _signalBus.Fire<ShowTutorialSignal>();
                }
                RemoveCardFromDeck();
                break;
            case false:
                transform.SetParent(_cardParent);
                transform.position = _startCardPos;
                break;
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        float angle = Vector2.Angle(eventData.delta.normalized, Vector2.right);
        var canvasGroup = gameObject.GetComponentInParent<CanvasGroup>();

        Debug.Log("ANGLE___" + angle);
        if ((angle >= 0 && angle <= 50) || (angle >= 130 && angle <= 230) )
        {
            _startCardPos = gameObject.transform.position;
            transform.SetParent(parentWhileDragging.parent);
            dragBlocked = false;                          
        }
        else
        {
            dragBlocked = true;
            canvasGroup.blocksRaycasts = false;
            _ignoreCard = true;
        }
    }

    protected override void ResetCard()
    {
        
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount > 1)
        {
            OnPointerUp(eventData);
            return;
        }
        if (_ignoreCard)
        {
            _contentPos.position += new Vector3(0, eventData.delta.y*Time.deltaTime*0.25f, 0);
            eventData.delta = Vector2.zero;
            return;   
        }

        if (!dragBlocked)
        {
            base.OnDrag(eventData);
        }
            
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _soundPlayer.PlaySound("click_card_01", AudioMixerController.MasterMixerGroup.SFX, 0.3f);
        _startCardPos = gameObject.transform.position;
    }

    public void ActivateNewCardIcon()
    {
        normalCard.ActivateNewCardIcon();
    }
}