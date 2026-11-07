using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using App.Analytic;
using App.Audio;
using App.DAO;
using App.Dialogs;
using App.PrototypeBattle;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library.Common;
using Library.Dialogs;
using Library.View;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace App.Chests
{
    public class ChestHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshPro open;
        [SerializeField] private CountdownTimer timer;
        [Inject] private ChestSetup _chestSetup;
        [SerializeField] private ChestType _type;
        [SerializeField] private ParticleSystem _dustEffect;
    
        private bool canOpen;
        private bool isClicked;
        private bool _chestFall = false;
        private DialogsContainer _dialogsContainer;
        private SoundPlayer _soundPlayer;
        private Sequence sequence;
        private IDataService _dataService;
        private IAnalytics _analytics;
        private bool _onLocation;
        private BoxCollider2D clickCollider;
    
        [Inject]
        public void Construct(DialogsContainer  dialogsContainer, SoundPlayer soundPlayer, IDataService dataService, IAnalytics analytics)
        {
            _dialogsContainer = dialogsContainer;
            _soundPlayer = soundPlayer;
            _dataService = dataService;
            _analytics = analytics;
        }
    
        public ChestType ChestType
        {
            get { return _type; }
            private set{}
        }

        public ChestSlot ChestSlot
        {
            get;
            private set;
        }

        private GameObject ShowChestsContainer()
        { 
            _analytics.SendEvent(AmplitudeEvent.ui_chests_show, new Dictionary<string, object>(){{AmplitudePlacement.placement.ToString(), AmplitudePlacementProperty.shade}});
            return  _dialogsContainer.ShowChestsContainerDialog();
        }
        private void ShowChestDialog()
        {
            _analytics
                .SendEvent(AmplitudeEvent.ui_chest_timer_show);
            GameObject chestDialog = _dialogsContainer.ShowChestDialog().gameObject;
            if (_type == ChestType.DAILY)
                chestDialog.GetComponent<ChestsDialog>().Setup(true, this);
            else
                chestDialog.GetComponent<ChestsDialog>().Setup(canOpen, this);
            chestDialog.SetActive(true);
        }

        private void Awake()
        {
            if(_type != ChestType.DAILY)
                ChestSlot = GetComponentInParent<ChestLocationController>().ChestSlot;

            clickCollider = GetComponent<BoxCollider2D>();
        }
    
        public void SetupChest() => SetChestOnLocation();

        private void SetChestOnLocation()
        {
            _chestFall = true;
            Sequence seq = DOTween.Sequence();
            var finishPos = transform.position;
            transform.position = new Vector3(transform.position.x, 20, 0);
            seq.Append(transform.DOMove(finishPos, 0.5f, false).OnComplete(() =>
                {
                    _soundPlayer.PlaySound("SFX_ChestFall", AudioMixerController.MasterMixerGroup.SFX, 0.5f);
                    _dustEffect.Play();
                })).SetEase(Ease.Linear)
                .Append(transform.DOPunchScale(new Vector3(1,-1,0)*0.1f, 0.2f,1,0))
                .Join(transform.DOPunchPosition(Vector3.down*0.1f, 0.2f,1,0))
                .OnComplete(() => {  
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    _dataService.SetString(AppConstants.TIMER + _type + ChestSlot, (DateTime.UtcNow.Ticks + TimeSpan.TicksPerMinute * GetReadyChestTime(_type)).ToString());
                    SetupTimer();
                    _chestFall = false;
                });
        }
        
        private void Start()
        {
            int stateNumber = (int)_dataService.GetLong(AppConstants.CHEST_STATE + _type + ChestSlot);
            if (ChestType == ChestType.DAILY && ((ChestState) stateNumber == ChestState.NONE || (ChestState)stateNumber == ChestState.READY))
            {
                _chestFall = false;
                canOpen = true;
                open.gameObject.SetActive(true);
            }
            if (ChestType == ChestType.DAILY && (ChestState)stateNumber == ChestState.WAIT)
            {
                _chestFall = false;
                SetupTimer();
            }
            if (!_chestFall && ChestType != ChestType.DAILY)
            {
                SetupTimer();
            }
        }

        public void ResetTimer()
        {
            timer.ResetText();
            timer.ResetTimeLeft();
            timer.StopCountdown();
        }
        public void SetupTimer()
        {
            int stateNumber = (int)_dataService.GetLong(AppConstants.CHEST_STATE + _type + ChestSlot);
            if (Enum.IsDefined(typeof(ChestState), stateNumber))
            {
                ChestState state = (ChestState) stateNumber;
                if (state == ChestState.OPENED || state == ChestState.NONE || state == ChestState.WAIT_ON_LOCATION)
                {
                    _dataService.SetString(AppConstants.TIMER + _type + ChestSlot,
                        (DateTime.UtcNow.Ticks + TimeSpan.TicksPerMinute * GetReadyChestTime(_type)).ToString());
                    _dataService.SetLong(AppConstants.CHEST_STATE + _type + ChestSlot, (int)ChestState.WAIT);
                    StartTimer();
                }
                else if (state == ChestState.WAIT) StartTimer();
                else if(state == ChestState.READY)
                    ChangeState(true);
            }
        }

        private int GetReadyChestTime(ChestType type) => 
            _chestSetup.GetChestCharacteristic(type).readyTimeMinutes;

        private void StartTimer()
        {
            long time = 0;
            string ticksString = _dataService.GetString(AppConstants.TIMER + _type + ChestSlot);
            if (Int64.TryParse(ticksString, out time))
            {
                long restTime = time - DateTime.UtcNow.Ticks;
                if (restTime > 0)
                {
                    if (ChestType == ChestType.DAILY) clickCollider.enabled = false;
                    timer.StartCountdown(DateTime.UtcNow.Ticks, restTime);
                    timer.OnCountdownFinish += () => ChangeState(true);
                    return;
                }
                ChangeState(true);
            }
        }

        private void ChangeState(bool canOpen)
        {
            if (sequence == null)
            {
                sequence = DOTween.Sequence();
                sequence.Append(transform.DOPunchPosition(Vector3.up*0.2f, 0.25f,1,0).SetEase(Ease.Unset))
                    .Join(transform.DOPunchScale(new Vector3(-1,1,0)*0.1f,0.25f,1,0))
                    .Append(transform.DOPunchScale(new Vector3(1,-1,0)*0.1f, 0.2f,1,0))
                    .Join(transform.DOPunchPosition(Vector3.down*0.1f, 0.2f,1,0))
                    .Append(transform.DOScale(transform.localScale, UnityEngine.Random.Range(1,1.5f))).SetLoops(-1, LoopType.Restart);
                sequence.Pause();
            }
            
            
            open.gameObject.SetActive(canOpen);
            timer.gameObject.SetActive(!canOpen);
            if (ChestType == ChestType.DAILY) clickCollider.enabled = true;

            if (canOpen)
            {
                sequence.Play();
            }
            else
            {
                sequence.Pause();
            }
            _dataService.SetLong(AppConstants.CHEST_STATE + _type + ChestSlot, canOpen ? (int)ChestState.READY :(int)ChestState.WAIT);
            this.canOpen = canOpen;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _soundPlayer.PlaySound("SFX_ButtonClickBase", AudioMixerController.MasterMixerGroup.SFX, 0.1f);
            if (_type == ChestType.DAILY)
            {
                if (canOpen)
                {
                    FindObjectOfType<AdsHelper>().PlayVideo(() =>
                    {
                        _dataService.SetLastInterTicks();
                        Jump();
                        SetupTimer();
                    }, AmplitudePlacementProperty.free_chest);
                    return;
                }
                return;
            }
            if(_dataService.GetChestCollectionData().ChestCollection.Find(chest=>chest.type==_type).onLocation)
                Jump();
            else
                ShowChestsContainer();
        }

        private void Jump()
        {
            if(!isClicked)
            {
                isClicked = true;
                transform.DOMoveY(transform.position.y - 0.045f, 0.1f)
                    .SetEase(Ease.Linear);
                transform.DOScaleY(transform.localScale.y - 0.045f, 0.1f)
                    .SetEase(Ease.Unset)
                    .OnComplete(() =>
                    {
                        transform.DOMoveY(transform.position.y + 0.045f, 0.1f)
                            .SetEase(Ease.Linear);
                        transform.DOScaleY(transform.localScale.y + 0.045f, 0.1f)
                            .OnComplete(
                                async () =>
                                {
                                    ShowChestDialog();
                                    isClicked = false;
                                });
                    });
            }
        }

        public void SetOpened()
        {
            _dataService.SetLong(AppConstants.CHEST_STATE + _type + ChestSlot, (int)ChestState.OPENED);
            SetupTimer();
            ChangeState(false);
        }
    }

    public enum ChestType
    {
        NONE = 0,
        MINI = 1,
        MEDIUM = 2,
        MEGA = 3,
        GIGA = 4,
        DAILY = 5,
    }

    public enum ChestState
    {
        NONE = 0,
        WAIT = 1,
        READY = 2,
        OPENED = 3,
        WAIT_ON_LOCATION = 4
    }
}