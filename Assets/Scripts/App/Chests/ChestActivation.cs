using System;
using System.Collections.Generic;
using App.Analytic;
using App.Audio;
using App.DAO;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Common;
using Library.Dialogs;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace App.Chests
{
    public class ChestActivation : MonoBehaviour, IPointerClickHandler
    {
        private const string CLICKED_SLOT = "clicked_slot";
        
        
        [SerializeField] private SpriteRenderer _renderer;
        private DialogsContainer _dialogsContainer;
        private IDataService _dataService;
        private SoundPlayer _soundPlayer;
        private Sequence sequence;
        private ChestSlot _slot;
        private bool isClicked;
        private SignalBus _signalBus;
        private IAnalytics _analytics;
        public bool IsSlotEmpty { get; set; }

        private void OnEnable()
        {
            _dataService.SetLong(CLICKED_SLOT, (int)_slot);
            HasChestAnimation();
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<HasChestSignal>(HasChestAnimation);
            _signalBus.TryUnsubscribe<OpenChestsForTutor>(OpenChestContainerForTutor);
        }

        [Inject]
        public void Construct(DialogsContainer  dialogsContainer, SoundPlayer soundPlayer, SignalBus signalBus, IDataService dataService, IAnalytics analytics)
        {
            _dialogsContainer = dialogsContainer;
            _soundPlayer = soundPlayer;
            _signalBus = signalBus;
            _dataService = dataService;
            _analytics = analytics;
        }
        
        private void Awake()
        {
            //_dataService = AllServices.Container.Single<IDataService>();
            _slot = GetComponentInParent<ChestLocationController>().ChestSlot;
        }

        private void Start()
        {
            _signalBus.Subscribe<HasChestSignal>(HasChestAnimation);
            _signalBus.Subscribe<OpenChestsForTutor>(OpenChestContainerForTutor);
            HasChestAnimation();
        }

        private void HasChestAnimation()
        {
            if (_dataService.GetChestCollectionData().ChestCollection.Exists(chest => chest.chestAmount > 0))
            {
                
                if (sequence == null)
                {
                    sequence = DOTween.Sequence();
                    sequence.Append(_renderer.DOFade(0.5f, 0.35f));
                    sequence.Append(_renderer.DOFade(1, 0.35f));
                    sequence.SetLoops(-1, LoopType.Restart);
                }
            } 
            else
            {
                if (sequence != null && sequence.IsPlaying())
                {
                    sequence.Kill();
                    _renderer.DOFade(1, 0.2f);
                }
            }
        }

        private GameObject ShowChestsContainer()
        {
            _analytics.SendEvent(AmplitudeEvent.ui_chests_show, new Dictionary<string, object>(){{AmplitudePlacement.placement.ToString(), AmplitudePlacementProperty.shade}});
            return _dialogsContainer.ShowChestsContainerDialog();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            _soundPlayer.PlaySound("SFX_ButtonClickBase", AudioMixerController.MasterMixerGroup.SFX, 0.1f);
            _dataService.SetLong(CLICKED_SLOT, (int)_slot);
            Jump(ShowChestsContainer());
        }

        private void OpenChestContainerForTutor()
        {
            if (_slot == ChestSlot.SLOT_1 && _dataService.GetTutorialCurrentStep() == TutorialStepType.OPEN_CHEST_POINTER_HELP)
            {
                _soundPlayer.PlaySound("SFX_ButtonClickBase", AudioMixerController.MasterMixerGroup.SFX, 0.1f);
                _dataService.SetLong(CLICKED_SLOT, 1);
                Jump(ShowChestsContainer());
            }
        }

        private void Jump(GameObject onComplete)
        {
            if (!isClicked)
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
                                    isClicked = false;
                                });
                    });
            }
        }
    }
}