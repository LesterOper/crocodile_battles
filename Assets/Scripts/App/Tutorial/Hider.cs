using App.Analytic;
using App.DAO;
using App.Signals;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;

namespace App.Tutorial
{
    public class Hider : MonoBehaviour, IPointerClickHandler
    {
        private IDataService _dataService;
        private bool Hide = false;
        private bool _clicked = false;
        private SignalBus _signalBus;
        private IAnalytics _analytics;

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService, IAnalytics analytics)
        {
            _signalBus = signalBus;
            _dataService = dataService;
            _analytics = analytics;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Hide)
            {
                Hide = true;
                if (_dataService.GetTutorialCurrentStep().GetAttribute<TutorialAttribute>().NeedDelayAfterClick && !_clicked)
                {
                    _clicked = true;
                    DOVirtual.DelayedCall(2, CompleteTutorialStep);
                }
                else if(!_clicked)
                {
                    _clicked = true;
                    CompleteTutorialStep();
                }
            }
        }

        private void CompleteTutorialStep()
        {
            _signalBus.Fire<CompleteTutorialSignal>();
            _signalBus.Fire<BattleTutorialSignal>();
            TutorialBroadcast();
        }

        private void TutorialBroadcast()
        {
            TutorialStepType tutorialStepType = _dataService.GetTutorialCurrentStep();
            switch (tutorialStepType)
            {
                case TutorialStepType.CLOSE_WEAPON_COLLECTION_DIALOG_POINTER_HELP:
                {
                    _signalBus.Fire<AutoBuildTutorSignal>();
                    _signalBus.Fire<TutorialSendTargetSignal>();
                    break;
                }
                case TutorialStepType.READY_BUTTON_HELP :
                {
                    Hide = false;
                    _signalBus.Fire<TutorialViewPointerSignal>();
                    break;
                }
                case TutorialStepType.OPEN_CHEST_ON_LOCATION:
                {
                    _analytics.SendEvent(AmplitudeTutorialEvent.tutor_new_chest_complete);
                    _signalBus.Fire<ChestOnLocationTutorSignal>();
                    _dataService.SetExtensionUnlock(Extension.BATTLE_PASS, true);
                    _dataService.SetExtensionUnlock(Extension.ACHIEVEMENTS, true);
                    _dataService.SetExtensionUnlock(Extension.PREM_SHOP, true);
                    break;
                }
                
                case TutorialStepType.OPEN_CHEST_POINTER_HELP:
                {
                    _signalBus.Fire<OpenChestsForTutor>();
                    break;
                }
                
                case TutorialStepType.TAKE_CHEST_POINTER_HELP:
                {
                    _signalBus.Fire<TutorialSendTargetSignal>();
                    break;
                }
                
                case TutorialStepType.AUTO_BUILD_POINTER_HELP:
                {
                    if (SceneManager.GetActiveScene().name == "Main")
                        _signalBus.Fire<TutorialSendTargetSignal>();
                    break;
                }
                
                case TutorialStepType.OPEN_CHEST:
                {
                    _signalBus.Fire<ChestTutorialSignal>();
                    break;
                }

                case TutorialStepType.BOSS_POINTER:
                {
                    _signalBus.Fire<TutorialSendTargetSignal>();
                    break;
                }
                
                case TutorialStepType.FIRST_BATH_POINTER:
                {
                    _signalBus.Fire<TutorialSendTargetSignal>();
                    break;
                }
                
                case TutorialStepType.FIRST_WEAPON_POINTER :
                {
                    if (SceneManager.GetActiveScene().name == "Main")
                    {
                        _signalBus.Fire<TutorialSendTargetSignal>();
                    }
                    break;
                }
                
                case TutorialStepType.AFTER_WIN:
                {
                    if (SceneManager.GetActiveScene().name == "Main")
                    {
                        _signalBus.Fire<CompleteTutorialSignal>();
                        _signalBus.Fire<ShowTutorialSignal>();
                    }
                    
                    break;
                }
                
                case TutorialStepType.FIRST_CHEST:
                {
                    if (SceneManager.GetActiveScene().name == "Main")
                    {
                        _signalBus.Fire<ShowTutorialSignal>();
                    }
                    break;
                }

                case TutorialStepType.MERGE_POINTER_1:
                {
                    _signalBus.Fire(new TutorialSendTargetSignal(){FreeMergePlace = true});
                    break;
                }

                case TutorialStepType.MERGE_OPEN_DIALOG:
                {
                    _signalBus.Fire<TutorialSendTargetSignal>();
                    break;
                }
            }
        }
    }
}
