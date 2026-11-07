using System.Collections;
using App;
using App.Analytic;
using App.Audio;
using App.Crocodile;
using App.DAO;
using App.PrototypeBattle;
using App.Signals;
using App.Tutorial;
using Battle;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Effects;
using I2.Loc;
using Library;
using Library.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BattleStates.States
{
    public class BattleViewController : MonoBehaviour, IBattleViewController
    {
        [SerializeField] private TextMeshProUGUI StartBattle;
        [SerializeField] private TextMeshProUGUI round;
        [SerializeField] private MovingAnimationCurvesContainer moverCurveContainer;
        [SerializeField] private CameraEffect _effect;
        [SerializeField] private Button readyButton;
        public RandomBattleController BattleController { get; set;}
        public static int roundIndex = 1;
        private Image autoBattleButtonImage;
        private IDataService _dataService;
        private SignalBus _signalBus;
        private IGameInfoService _gameInfoService;
        private SoundPlayer _soundPlayer;
        public int RoundCount { get; set; }

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService, IGameInfoService gameInfoService, SoundPlayer soundPlayer)
        {
            _signalBus = signalBus;
            _dataService = dataService;
            _gameInfoService = gameInfoService;
            _soundPlayer = soundPlayer;
        }
        
        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<StartNewRounsSignal>(GoToFight);
            readyButton.onClick.RemoveListener(GoToFight);
        }

        private void Start()
        {
            readyButton.onClick.AddListener(GoToFight);
            PlayerPrefs.SetInt(AppConstants.IS_AUTO_MODE_ON, 0);
            roundIndex = 1;
            SetInitViewState();
            round.DOFade(0.75f, 0f);
            StartBattle.DOFade(0.2f, 1.5f)
                .OnComplete(() =>
                {
                    StartBattle.DOFade(1, 2.2f);
                })
                .SetEase(Ease.InSine)
                .SetDelay(0.2f)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void GoToFight()
        {
            _soundPlayer.PlaySound("button_start_round_01", AudioMixerController.MasterMixerGroup.SFX, 0.3f);
            TutorialStepType tutorialStepType = _dataService.GetTutorialCurrentStep();
            _signalBus.Fire<WeaponEquippedSignal>();
            _signalBus.Fire<GoToFightSignal>();
            if (tutorialStepType == TutorialStepType.READY_BUTTON_HELP ||
                tutorialStepType == TutorialStepType.READY_BUTTON_HELP_2 ||
                tutorialStepType == TutorialStepType.READY_BUTTON_HELP_3 ||
                tutorialStepType == TutorialStepType.READY_BUTTON_HELP_4)
            {
                _signalBus.Fire<CompleteTutorialSignal>();
            }
        }

        private void SetInitViewState()
        {
            _signalBus.Subscribe<StartNewRounsSignal>(GoToFight);
            round.text = LocalizationManager.GetTranslation("ROUND") + " " + roundIndex;
            round.text = LocalizationManager.GetTranslation("CHOOSE_CARD");
        }

        public void ChangeReadyButtonState(bool active)
        {
            readyButton.interactable = active;
        }

        public void IncreaseRoundCounter()
        {
            round.text = LocalizationManager.GetTranslation("ROUND") + " " + (roundIndex++);
            RoundCount = roundIndex-1;
        }
            
        public void SetPreparingText() => round.text = LocalizationManager.GetTranslation("CHOOSE_CARD");

        public AnimationCurve Curve(AnimationType type) => moverCurveContainer.GetAnimationCurve(type);

    }
    
}