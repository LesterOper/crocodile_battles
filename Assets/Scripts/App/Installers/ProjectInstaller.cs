using App.Achievements;
using App.Analytic;
using App.Audio;
using App.CodeBase.Infrastructure.States;
using App.Counters;
using App.DAO;
using App.Dialogs.BattlePass;
using App.Dialogs.EndGameDialog;
using App.PremPurchases;
using App.Signals;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Factory;
using Library.Common;
using Library.Common.Purchases;
using Library.L10n;
using MRGS.Base;
using UnityEngine;
using Zenject;

namespace App.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameObject mixerController;
        [SerializeField] private GameObject musicPlayer;
        [SerializeField] private GameObject soundPlayer;
        [SerializeField] private GameBootstrapper bootstrapper;
        [SerializeField] private LocaleHelper localeHelper;
        [SerializeField] private PurchaseTransactionController purchaseHelper;
        [SerializeField] private PremPurchasesSetup _purchasesSetup;
        [SerializeField] private PremPurchasesInfoSetup _purchasesInfo;
        
        public override void InstallBindings()
        {
            BindSignals();
            BindServices();
            BindBootstrapper();
            BindMixerPlayer();
            BindHelpers();
        }

        private void BindHelpers()
        {
            LocaleHelper local = Container.InstantiatePrefabForComponent<LocaleHelper>(localeHelper);
            Container.Bind<LocaleHelper>().FromInstance(local).AsSingle();
            PurchaseTransactionController purchase = Container.InstantiatePrefabForComponent<PurchaseTransactionController>(purchaseHelper);
            Container.Bind<PurchaseTransactionController>().FromInstance(purchase).AsSingle();
            Container.BindInstance(_purchasesInfo);
            Container.BindInstance(_purchasesSetup);
        }

        private void BindSignals()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<TakeCardSignal>();
            Container.DeclareSignal<BattleEndSignal>();
            Container.DeclareSignal<ChestOpenedSignal>();
            Container.DeclareSignal<ChestTakenSignal>();
            Container.DeclareSignal<WeaponEquippedSignal>();
            Container.DeclareSignal<HealthPointsChangeSignal>();
            Container.DeclareSignal<AttackChangedSignal>();
            Container.DeclareSignal<GameOverSignal>();
            Container.DeclareSignal<BathEffectSignal>();
            Container.DeclareSignal<MoveEffectSignal>();
            Container.DeclareSignal<ShowEndBattleViewSignal>();
            Container.DeclareSignal<MergeFilterChangedSignal>();
            Container.DeclareSignal<ActivateChestSignal>();
            Container.DeclareSignal<UpdateEndGameDialogSignal>();
            Container.DeclareSignal<ChestCollectedSignal>();
            Container.DeclareSignal<StartNewRounsSignal>();
            Container.DeclareSignal<RecreateCharacterOnMainSceneSignal>();
            Container.DeclareSignal<UpdateItemsInListSignal>();
            Container.DeclareSignal<AddItemInListSignal>();
            Container.DeclareSignal<ClearSubscribersAfterFightSignal>();
            Container.DeclareSignal<OpenCurtainSignal>();
            Container.DeclareSignal<UpdateChestSlotsSignal>();
            Container.DeclareSignal<ShowTutorialSignal>();
            Container.DeclareSignal<CompleteTutorialSignal>();
            Container.DeclareSignal<BattleTutorialSignal>();
            Container.DeclareSignal<TutorialPointerSignal>();
            Container.DeclareSignal<FadeTutorPointerSignal>();
            Container.DeclareSignal<ChestTutorialSignal>();
            Container.DeclareSignal<TutorialSendTargetSignal>();
            Container.DeclareSignal<TutorialButtonSignal>();
            Container.DeclareSignal<SetTutorialStepSignal>();
            Container.DeclareSignal<NameChangedSignal>();
            Container.DeclareSignal<HasChestSignal>();
            Container.DeclareSignal<NewCardsSignal>();
            Container.DeclareSignal<ResetChestProgressSignal>();
            Container.DeclareSignal<UnlockFilterButtonsSignal>();
            Container.DeclareSignal<ManaChangedSignal>();
            Container.DeclareSignal<GoToFightSignal>();
            Container.DeclareSignal<RemoveCardFromListSignal>();
            Container.DeclareSignal<TutorialViewPointerSignal>();
            Container.DeclareSignal<ChestNotificationSignal>();
            Container.DeclareSignal<ButtonInteractableInTutor>();
            Container.DeclareSignal<OpenChestsForTutor>();
            Container.DeclareSignal<UpdateBathSignal>();
            Container.DeclareSignal<ChestOnLocationTutorSignal>();
            Container.DeclareSignal<DebufDamageSignal>();
            Container.DeclareSignal<AutoBuildTutorSignal>();
            Container.DeclareSignal<DailyTasksCompleteSignal>();
            Container.DeclareSignal<ArmorPointsChangedSignal>();
            Container.DeclareSignal<SwitchArmorSignal>();
            Container.DeclareSignal<BattlePassProgressSignal>();
            Container.DeclareSignal<BattlePassNotificationSignal>();
            Container.DeclareSignal<ChangeMoneySignal>();
            Container.DeclareSignal<LoadBattleSceneSignal>();
            Container.DeclareSignal<BoosterAmountChanged>();
            Container.DeclareSignal<ActivateBoosterSignal>();
            Container.DeclareSignal<FoldCardSignal>();
            Container.DeclareSignal<MergeAvailableSignal>();
            Container.DeclareSignal<AchieveSignal>();
            Container.DeclareSignal<DailyTaskReadyToClaim>();
            Container.DeclareSignal<UpdateDailyTasks>();
            Container.DeclareSignal<UpdateDailyTaskListSignal>();
        }

        private void BindBootstrapper()
        {
            GameBootstrapper gameBootstrapper = Container.InstantiatePrefabForComponent<GameBootstrapper>(bootstrapper);
            Container.Bind<GameBootstrapper>().FromInstance(gameBootstrapper).AsSingle();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>().FromMethod(_ => Container.Resolve<GameBootstrapper>().StateMachine).AsSingle();
            Container.Bind<IDataService>().To<SqlDataService>().FromInstance(new SqlDataService()).AsSingle();
            //Container.Bind<IMRGSService>().To<MrgsController>().FromInstance(new MrgsController()).AsSingle();
            Container.Bind<CounterUtility>().FromNew().AsSingle();
            Container.Bind<IGameInfoService>().To<GameInfoService>().FromInstance(new GameInfoService()).AsSingle();
            Container.Bind<IGameFactory>().To<GameFactory>().FromInstance(new GameFactory()).AsSingle();
            Container.Inject(Container.Resolve<IGameFactory>());
            Container.Bind<IAnalytics>().To<AnalyticsHelper>().FromInstance(new AnalyticsHelper()).AsSingle();
            Container.Bind<IAchievementObserver>().To<AchievementsObserver>().FromInstance(new AchievementsObserver()).AsSingle();
            Container.Bind<BattlePassUtility>().FromNew().AsSingle();
        }
        
        private void BindMixerPlayer()
        {
            AudioMixerController mixerControllerLocal = Container.InstantiatePrefabForComponent<AudioMixerController>(mixerController);
            Container.Bind<AudioMixerController>().FromInstance(mixerControllerLocal).AsSingle();
            
            MusicPlayer musicPlayerLocal = Container.InstantiatePrefabForComponent<MusicPlayer>(musicPlayer);
            Container.Bind<MusicPlayer>().FromInstance(musicPlayerLocal).AsSingle();

            SoundPlayer soundPlayerLocal = Container.InstantiatePrefabForComponent<SoundPlayer>(soundPlayer);
            Container.Bind<SoundPlayer>().FromInstance(soundPlayerLocal).AsSingle();
        }
    }
}
