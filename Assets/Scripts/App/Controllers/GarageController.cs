using System.Collections.Generic;
using App.Analytic;
using App.Audio;
using App.Cards;
using App.Chests;
using App.Counters;
using App.DAO;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Effects;
using Library;
using Library.CanvasNamespace;
using Library.Common;
using Library.Dialogs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Controllers
{
    public class GarageController :  AbstractPanelController
    {
        [SerializeField] private ChestHandler _handler;
        [SerializeField] private GameObject bathNewIcon;
        [SerializeField] private GameObject weaponsNewIcon;
        [SerializeField] private Transform fightButton;
        [SerializeField] private Transform takeChestTutorTarget;
        [SerializeField] private Transform weaponsCollectionForTutorial;
        [SerializeField] private Transform bathCollectionForTutorial;
        [SerializeField] private Transform mergeForTutorial;
        [SerializeField] private Button mergeButton;
        [SerializeField] private Button bathCollectionButton;
        [SerializeField] private Button weaponCollectionButton;
        [SerializeField] private Transform bossFightButton;
        private IGameInfoService _gameInfoService;
        private IDataService _dataService;
        private IAnalytics _analytics;
        private DialogsContainer _dialogsContainer;
        private MusicPlayer _musicPlayer;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(DialogsContainer dialogsContainer,  MusicPlayer musicPlayer, SignalBus signalBus, 
            IDataService dataService, IAnalytics analytics, IGameInfoService gameInfoService)
        {
            _dialogsContainer = dialogsContainer;
            _musicPlayer = musicPlayer;
            _signalBus = signalBus;
            _analytics = analytics;
            _dataService = dataService;
            _gameInfoService = gameInfoService;
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<ChestTutorialSignal>(ShowChestTutor);
            _signalBus.TryUnsubscribe<NewCardsSignal>(NewCards);
        }

        private void ShowBathCollectionDialog() => _dialogsContainer.ShowBathCollectionDialog();

        private void ShowWeaponCollectionDialog() => _dialogsContainer.ShowWeaponCollectionDialog();

        private void Start()
        {
            _signalBus.Subscribe<ChestTutorialSignal>(ShowChestTutor);
            _signalBus.Subscribe<TutorialSendTargetSignal>(SendTargetToTutorial);
            _signalBus.Subscribe<NewCardsSignal>(NewCards);
            if (PlayerPrefs.HasKey("music_volume"))
                AudioListener.volume = PlayerPrefs.GetFloat("music_volume");
            else AudioListener.volume = 1;
            //DOVirtual.DelayedCall(6,
                //() =>
               // {
                    _musicPlayer.PlayMusic("music_menu_loop_01", AudioMixerController.MasterMixerGroup.MUSIC,
                        MusicPlayer.Channel.Music, 0.45f);
                    _musicPlayer.PlayMusic("amb_menu_loop_01", AudioMixerController.MasterMixerGroup.SFXCONST, MusicPlayer.Channel.Ambient, 0.1f);
              //  });
            TutorialResume();
            NewCards();
            
            SetupButton(mergeButton,  ShowMergeDialog);
            SetupButton(weaponCollectionButton, ShowWeaponCollectionDialog);
            SetupButton(bathCollectionButton, ShowBathCollectionDialog);
            
        }

        private void NewCards()
        {
            bathNewIcon.gameObject.SetActive(_gameInfoService.GetNewBathsCardInfo().Count > 0);
            weaponsNewIcon.gameObject.SetActive(_gameInfoService.GetNewCardsInfo().Count > 0);
        }
        
        private void  ShowMergeDialog()
        {
           _analytics
                .SendEvent(AmplitudeEvent.ui_merge_show);
            _dialogsContainer.ShowMergeDialog();
        }
        
        private void TutorialResume()
        {
            TutorialStepType stepType = _dataService.GetTutorialCurrentStep();

            if (_dataService.GetChestCollectionData().ChestCollection.Exists(chest => chest.chestAmount > 0) &&
                !_dataService.GetTutorialStepComplete(TutorialStepType.OPEN_CHEST_POINTER_HELP))
            {
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_new_chest_start);
                _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.TAKE_CHEST_HELP});
                _signalBus.Fire<ShowTutorialSignal>();
                return;
            }

            if (!_dataService.GetTutorialStepComplete(TutorialStepType.OPEN_CHEST)
                && _dataService.GetTutorialCurrentStep() != TutorialStepType.BATTLE_TUTORIAL_END)
            {
                _dataService.GetCardCollectionData().Collection.Clear();
                _dataService.GetBathCollection().BathDatas.Clear();
                _dataService.UpdateBathCollection();
                _dataService.UpdateCardCollectionData();
                _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.AFTER_WIN});
                _signalBus.Fire<ShowTutorialSignal>();
                return;
            }
            
            if (!_dataService.GetTutorialStepComplete(TutorialStepType.AUTO_BUILD_POINTER_HELP) 
                && _dataService.GetTutorialCurrentStep() != TutorialStepType.BATTLE_TUTORIAL_END)
            {
                _dataService.SetExtensionUnlock(Extension.AUTO_BUILD_BUTTON, false);
                List<BattleCardInfo> cardInfos = _dataService.GetBattleCardInfoData().CardInfos;
                foreach (var card in cardInfos)
                {
                    _dataService.GetCardCollectionData().Collection.Add(new CardInfo() {ID = card.ID, type = card.type, Level = card.Level, WeaponType = card.WeaponType});
                }
                
                _dataService.GetBattleCardInfoData().CardInfos.Clear();
                _dataService.UpdateCardCollectionData();
                _dataService.UpdateBattleCardInfoData();
                _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.MOVE_TO_GARAGE});
                _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = weaponsCollectionForTutorial});
                return;
            }

            if (!_dataService.GetTutorialStepComplete(TutorialStepType.FIRST_BATH_POINTER) 
                && _dataService.GetTutorialCurrentStep() != TutorialStepType.BATTLE_TUTORIAL_END)
            {
                _dataService.SetExtensionUnlock(Extension.BATH_COLLECTION_BUTTON, true);
                _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.MOVE_TO_BATH_COLLECTION_DIALOG});
                _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = bathCollectionForTutorial});
                return;
            }

            if (!_dataService.GetTutorialStepComplete(TutorialStepType.RANDOM_BATTLE_POINTER_HELP) 
                && _dataService.GetTutorialCurrentStep() != TutorialStepType.BATTLE_TUTORIAL_END)
            {
                _dataService.SetExtensionUnlock(Extension.FIGHT, true);
                _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.RANDOM_BATTLE_POINTER_HELP});
                _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = fightButton});
                return;
            }
            
            if (stepType == TutorialStepType.NONE) return;
            switch (stepType)
            {
                case TutorialStepType.BATTLE_TUTORIAL_END:
                {
                    _analytics.SendEvent(AmplitudeTutorialEvent.tutor_chest_start);
                    _signalBus.Fire<CompleteTutorialSignal>();
                    _signalBus.Fire<ShowTutorialSignal>();
                    
                    break;
                }

                case TutorialStepType.FIRST_CHEST:
                {
                    _signalBus.Fire<ShowTutorialSignal>();
                    break;
                }

                case TutorialStepType.OPEN_CHEST:
                {
                    _signalBus.Fire<ChestTutorialSignal>();
                    break;
                }
                case TutorialStepType.MERGE_OPEN_DIALOG :
                {
                    _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.MERGE});
                    _signalBus.Fire<ShowTutorialSignal>();
                    break;
                }
                
                case TutorialStepType.MERGE_IN_DIALOG_HELP :
                {
                    _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.MERGE});
                    _signalBus.Fire<ShowTutorialSignal>();
                    break;
                }
                case TutorialStepType.MERGE_POINTER_1 :
                {
                    _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.MERGE});
                    _signalBus.Fire<ShowTutorialSignal>();
                    break;
                }
                case TutorialStepType.MERGE_POINTER_2 :
                {
                    _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.MERGE});
                    _signalBus.Fire<ShowTutorialSignal>();
                    break;
                }
            }
        }
    
        private void ShowChestTutor()
        {
            _dialogsContainer.ShowChestDialog().Setup(true, _handler, true);
        }

        private void SendTargetToTutorial()
        {
            TutorialStepType tutorialStepType = _dataService.GetTutorialCurrentStep();

            switch (tutorialStepType)
            {
                case TutorialStepType.MOVE_TO_BATH_COLLECTION_DIALOG:
                {
                    _analytics.SendEvent(AmplitudeTutorialEvent.tutor_bath_button_start);
                    _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = bathCollectionForTutorial});
                    break;
                }
                
                case TutorialStepType.MOVE_TO_GARAGE:
                {
                    _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = weaponsCollectionForTutorial});
                    break;
                }

                case TutorialStepType.TAKE_CHEST_POINTER_HELP:
                {
                    _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = takeChestTutorTarget});
                    break;
                }
                
                case TutorialStepType.RANDOM_BATTLE_POINTER_HELP:
                {
                    _dataService.SetExtensionUnlock(Extension.FIGHT, true);
                    _signalBus.Fire<TutorialButtonSignal>();
                    _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = fightButton});
                    break;
                }
                

                case TutorialStepType.MERGE_OPEN_DIALOG:
                {
                    
                    _signalBus.Fire<TutorialButtonSignal>();
                    _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = mergeForTutorial});
                    break;
                }

                case TutorialStepType.BOSS_POINTER:
                {
                    _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = bossFightButton});
                    break;
                }
            }
        }
    }
}