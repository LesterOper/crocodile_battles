using System;
using App.Analytic;
using App.CodeBase.Infrastructure.States;
using App.DAO;
using App.SetupsScript;
using App.Tutorial;
using CodeBase.Infrastructure.States;
using DG.Tweening;
using I2.Loc;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.BattlePass
{
    public class BattlePassBossPrizeView : MonoBehaviour
    {
        private const string BattleScene = "BattleScene";

        [SerializeField] private Transform toast;
        [SerializeField] private TextMeshProUGUI toastText;
        [SerializeField] private Button bossFight;
        [SerializeField] private TextMeshProUGUI needBattlesAmount;
        [SerializeField] private Image closeIcon;
        [SerializeField] private Image claimedIcon;
        [Inject] private IAnalytics _analytics;
        [Inject] private IGameInfoService _gameInfoService;
        [Inject] private GameStateMachine _gameStateMachine;
        [Inject] private IDataService _dataService;
        [Inject] private DialogsContainer _dialogHelper;
        private BathType _boss;
        public Action<bool> OnPrizeOpen;
        [SerializeField] private Sprite lockedSprite;
        [SerializeField] private Sprite openedSprite;


        public void Setup(PrizeItems prizeItems)
        {
            _boss = prizeItems.BossBathType;
            bossFight.onClick.AddListener(TransferToBossFight);
            OnPrizeOpen = ChangeOpenState;
            ChangeClaimedState(_boss < _dataService.GetCurrentBoss());
        }


        private void OnDisable()
        {
            bossFight.onClick.RemoveAllListeners();
        }

        private void ChangeClaimedState(bool claimed)
        { 
            //closeIcon.gameObject.SetActive(!claimed);
            bossFight.interactable = !claimed;
            claimedIcon.gameObject.SetActive(claimed);
            claimedIcon.DOFade(claimed ? 1 : 0, 0);
        }

        public void ChangeOpenState(bool active)
        {
            closeIcon.gameObject.SetActive(!active);
            bossFight.interactable = active;
            bossFight.GetComponent<Image>().sprite = active ? openedSprite : lockedSprite; 
            if (_boss < _dataService.GetCurrentBoss())
            {
                bossFight.interactable = false;
            }
        }
        
        private void TransferToBossFight()
        {
            _analytics.SendStartBossBattleEvent(AmplitudeEvent.game_boss_start, _dataService.GetBathData());


            if (_dataService.GetTutorialCurrentStep() == TutorialStepType.BOSS_BUTTON_POINTER)
            {
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_boss_button_complete);
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_boss_complete);
            }
                  
            var boostersDialog = _dialogHelper.ShowBoostersDialog();
            boostersDialog.SetFightButton(() =>
            {
                if ((int)_dataService.GetCurrentBoss() <= 16)
                {
                    _gameInfoService.SetIsBossFight(true);
                    LoadBattleScene();
                }
            });
        }
        
        private void LoadBattleScene()
        {
            if (ToastShow())
            {
                toast.DOKill();
                toastText.DOKill();
                toast.gameObject.SetActive(true);
                toast.DOScale(1.2f, 2f);
                toastText.DOFade(0, 1.2f).SetDelay(3f).OnComplete(() =>
                {
                    toast.gameObject.SetActive(false);
                    toast.localScale = Vector3.zero;
                    toastText.DOFade(1, 0);
                });
                return;
            }
            _gameStateMachine.Enter<LoadLevelState, string>(BattleScene);
        }

        private bool ToastShow()
        {
            var cards = _dataService.GetBattleCardInfoData().CardInfos;

            if (cards.Count < 5)
            {
                toastText.text = LocalizationManager.GetTranslation("TOAST_WITHOUT_WEAPONS");
                return true;
            }

            return false;
        }
    }
}