using System;
using System.Collections;
using System.Collections.Generic;
using App;
using App.Achievements;
using App.Analytic;
using App.Audio;
using App.CodeBase.Infrastructure.States;
using App.DAO;
using App.Signals;
using App.Tutorial;
using BattleStates.States;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.States;
using DG.Tweening;
using Effects;
using Library;
using Library.CanvasNamespace;
using Library.Collections;
using Library.Common;
using Library.Dialogs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Battle
{
    public class BattleScene : MonoBehaviour
    {
        [SerializeField] private GameObject _firstNonBossArena = null;
        [SerializeField] private GameObject _secondNonBossArena = null;
        [SerializeField] private GameObject _firstBossArena;
        [SerializeField] private GameObject _secondBossArena;
        [SerializeField] private Button Back;
        [SerializeField] private Button Ads;
        [SerializeField] private LogoScreen _logoScreen;
        [SerializeField] private GameObject AdsText;
        [SerializeField] private GameObject enemyBlocker;
        [SerializeField] private List<AudioClip> laughSFX;
        private AudioSource _audioSource;
        private MusicPlayer _musicPlayer;
        private IDataService _dataService;
        private DialogsContainer _container;
        private EnemyCardHandler _enemyCardHandler;
        private SignalBus _signalBus;
        private GameStateMachine _gameStateMachine;
        private IGameInfoService _gameInfoService;
        private IAnalytics _analytics;

        [Inject]
        public void Construct(DialogsContainer container, MusicPlayer musicPlayer, 
            SignalBus signalBus, GameStateMachine gameStateMachine, 
            IGameInfoService gameInfoService, IAnalytics analytics, IDataService dataService)
        {
            _container = container;
            _musicPlayer = musicPlayer;
            _signalBus = signalBus;
            _gameStateMachine = gameStateMachine;
            _gameInfoService = gameInfoService;
            _analytics = analytics;
            _dataService = dataService;
        }
        
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<ActivateBoosterSignal>(ActivateFreezeBooster);
        }

        private void ActivateFreezeBooster(ActivateBoosterSignal signal)
        {
            if(signal.BoosterType != BoosterType.FREEZE)
                return;
            enemyBlocker.SetActive(!signal.IsUsed);
        }

        private void CreateLocation(GameObject location)
        {
            Instantiate(location, this.transform);
        }

        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _signalBus.Subscribe<ActivateBoosterSignal>(ActivateFreezeBooster);
            if (_dataService.GetExtensionUnlock(Extension.FIGHT))
            {
                _logoScreen.WithoutMove(0f);
            }

            if (_gameInfoService.IsBossFight())
            {
                if(UnityEngine.Random.Range(0f,1f)>0.5)
                    CreateLocation(_firstBossArena);
                else
                    CreateLocation(_secondBossArena);
            }
            else
            {
                if(UnityEngine.Random.Range(0f,1f)>0.5)
                    CreateLocation(_firstNonBossArena);
                else
                    CreateLocation(_secondNonBossArena);
            }
            
            _enemyCardHandler = FindObjectOfType<EnemyCardHandler>();
            if (!_dataService.GetTutorialStepComplete(TutorialStepType.BATTLE_TUTORIAL_END))
            {
                Ads.gameObject.SetActive(false);
                AdsText.SetActive(false);
            }
            
            SetupButton(Back, ReturnToMainLocation);
            SetupButton(Ads, PlayAds);
            //DOVirtual.DelayedCall(2f, () =>
            //{
                _musicPlayer.PlayMusic("music_fight_loop_01", AudioMixerController.MasterMixerGroup.MUSIC, MusicPlayer.Channel.Music, 0.45f);
                _musicPlayer.PlayMusic("amb_fight_loop_01", AudioMixerController.MasterMixerGroup.SFXCONST, MusicPlayer.Channel.Ambient, 0.1f);
            //});
            
            StartCoroutine(PlayCrocoIntroSound());
        }

        private IEnumerator PlayCrocoIntroSound()
        {
            yield return new WaitForSeconds(1f);
            _audioSource.PlayOneShot(laughSFX.Random(), 0.7f);
            yield return new WaitForSeconds(1f);
            _audioSource.PlayOneShot(laughSFX.Random(), 0.8f);

        }

        private void PlayAds() => FindObjectOfType<AdsHelper>().PlayVideo(() =>
        {
            _dataService.SetLastInterTicks();
            _gameInfoService.IsEnemyCardsOpenByAds = true;
            _enemyCardHandler.InvokeShowCardsByAds();
            Ads.interactable = false;
            Ads.image.DOFade(0, 1f).OnComplete(() => Ads.gameObject.SetActive(false));
            AdsText.gameObject.SetActive(false);
        }, AmplitudePlacementProperty.show_enemy_cards);

        private void ReturnToMainLocation()
        {
            if (BattleViewController.roundIndex > 1)
            {
                _container.ShowExitBattleDialog();
            }
            else
            {
                BattleViewController.roundIndex = 1;
                _signalBus.Fire<ClearSubscribersAfterFightSignal>();
                _gameStateMachine
                    .Enter<LoadLevelState, string>("Main");
                _analytics.SendEvent(AmplitudeEvent.game_end,
                    new Dictionary<string, object>()
                    {{AmplitudePlacement.result.ToString(), AmplitudePlacementProperty.leave}});
            }
            
            if (_gameInfoService.IsBossFight())
                _analytics.SendEvent(AmplitudeEvent.game_boss_end, 
                        new Dictionary<string, object>(){{AmplitudePlacement.result.ToString(), AmplitudePlacementProperty.leave}});
        }

        private void SetupButton(Button targetButton, UnityAction action)
        {
            if (targetButton != null) targetButton.onClick.AddListener(action);
        }
    }
}