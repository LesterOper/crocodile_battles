using System;
using System.Collections;
using System.Collections.Generic;
using App.Analytic;
using App.Cards;
using App.DAO;
using App.Signals;
using CodeBase.Infrastructure.Services;
using Library;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Controllers
{
    public class AutoBuildController : MonoBehaviour
    {
        private Button autoBuild;
        private IDataService _dataService;
        private SignalBus _signalBus;
        private IAnalytics _analytics;

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService, IAnalytics analytics)
        {
            _signalBus = signalBus;
            _dataService = dataService;
            _analytics = analytics;
        }
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<AutoBuildTutorSignal>(AutoBuild);
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<AutoBuildTutorSignal>(AutoBuild);
        }

        private void Start()
        {
            autoBuild = GetComponent<Button>();
            autoBuild.onClick.AddListener(() =>
            {
                _analytics.SendEvent(AmplitudeEvent.auto_equip);
                AutoBuild();
            });
        }

        private void AutoBuild()
        {
            List<BattleCardInfo> cardInfos = _dataService.GetBattleCardInfoData().CardInfos;
            foreach (var card in cardInfos)
            {
                _dataService.GetCardCollectionData().Collection.Add(new CardInfo() {ID = card.ID, type = card.type, Level = card.Level, WeaponType = card.WeaponType});
            }
            _dataService.GetBattleCardInfoData().CardInfos.Clear();
            AutoBuildWeapons();
            _signalBus.Fire<UpdateItemsInListSignal>();
            _dataService.UpdateCardCollectionData();
            _dataService.UpdateBattleCardInfoData();
        }

        private void AutoBuildWeapons()
        {
            List<CardInfo> weapons = new List<CardInfo>(_dataService.GetCardCollectionData().Collection);
            CardInfo cardInfo = null;
            if (weapons.Count < 50)
            {
                _dataService.GetBattleCardInfoData().CardInfos.Clear();
                foreach (var weap in weapons)
                {
                    _dataService.GetBattleCardInfoData().CardInfos.Add(new BattleCardInfo(weap));    
                }
                _dataService.GetCardCollectionData().Collection.Clear();
            }
            else
            {
                for (int i = 0; i < 50; i++)
                {
                    var card = weapons[i];
                    cardInfo = card;
                    if(cardInfo == null) continue;
                    _dataService.GetCardCollectionData().Collection.Remove(cardInfo);
                    BattleCardInfo battleCardInfo = new BattleCardInfo(cardInfo);
                    _dataService.GetBattleCardInfoData().CardInfos.Add(battleCardInfo);
                }
            }
            _dataService.UpdateBattleCardInfoData();
            _dataService.UpdateCardCollectionData();
        }
    }
}
