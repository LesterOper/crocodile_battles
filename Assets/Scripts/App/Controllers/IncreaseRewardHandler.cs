using System;
using App.Analytic;
using App.DAO;
using App.SetupsScript;
using Library.Common;
using Library.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Controllers
{
    public class IncreaseRewardHandler : MonoBehaviour
    {
        [SerializeField] private Button x2Reward;
        [SerializeField] private CountdownTimer timer;
        [SerializeField] private TextMeshProUGUI x2RewardText;
        [SerializeField] private Image x2Icon;
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite notActive;
        [SerializeField] private Image adReadyIcon;
        [Inject] private IDataService _dataService;
        [Inject] private BattleSetup _battleSetup;
        private AdsHelper _adsHelper;
        private void Start()
        {
            x2Reward.onClick.AddListener(() =>
            {
                FindObjectOfType<AdsHelper>().PlayVideo(() =>
                    {
                        _dataService.SetX2Reward(DateTime.UtcNow.Ticks + TimeSpan.TicksPerMinute * _battleSetup.X2BonusDuration);
                        StartTimer();
                    },
                    AmplitudePlacementProperty.battle);
                
            });
            
            StartTimer();
        }

        private void StartTimer()
        {
            long time = _dataService.GetX2Reward();
            long leftTicks = time - DateTime.UtcNow.Ticks;
            if (leftTicks > 0)
            {
                adReadyIcon.gameObject.SetActive(false);
                ResetX2RewardView(true);
                timer.StartCountdown(DateTime.UtcNow.Ticks, leftTicks);
                timer.OnCountdownFinish += () => ResetX2RewardView(false);
            }
            else
            {
                ResetX2RewardView(false);
            }
        }

        private void ResetX2RewardView(bool active)
        {
            timer.gameObject.SetActive(active);
            x2Reward.interactable = !active;
            x2Icon.sprite = active ? activeSprite : notActive;
            _dataService.SetX2RewardState(active);
        } 
    }
}