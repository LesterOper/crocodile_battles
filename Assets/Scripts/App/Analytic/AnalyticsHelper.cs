using System;
using System.Collections.Generic;
using App.Cards;
using App.DAO;
using App.PrototypeBattle;
using CodeBase.Infrastructure.Services;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Analytics;
using VoxelBusters.CoreLibrary;
using Zenject;

namespace App.Analytic
{
    public class AnalyticsHelper : IAnalytics
    {
        private Amplitude amplitude;
        private Firebase.FirebaseApp firebase;
        [Inject]private IDataService _dataService;
        private TimeSpan _timeSinceFirstPlay;
        
        public AnalyticsHelper()
        {
            InitAnalytics();
        }
        
        private void InitAnalytics()
        {
            amplitude = Amplitude.getInstance();
            amplitude.setServerUrl("https://api2.amplitude.com");
            amplitude.logging = true;
            amplitude.trackSessionEvents(true);
            amplitude.init("166799c9e172c4418e2ef6dcbd50043b");
            
            /*Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            });*/
            
            /*if (_dataService.GetLong("FirstGameStart") == 0)
            {
                //PlayerPrefs.DeleteAll();
                PlayerPrefs.SetString("FirstGameTime", DateTime.Now.ToString());
                _dataService.SetLong("FirstGameStart",1);
            }
            _timeSinceFirstPlay = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("FirstGameTime"));*/
            PlayerPrefs.DeleteAll();
        }

        private List<BattleCardInfo> SortCards(List<BattleCardInfo> cardInfos)
        {
            cardInfos.Sort(delegate(BattleCardInfo x, BattleCardInfo y)
            {
                if (x.WeaponType == WeaponType.NONE && y.WeaponType == WeaponType.NONE) return 0;
                else if (x.WeaponType == WeaponType.NONE) return -1;
                else if (y.WeaponType == WeaponType.NONE) return 1;
                else return x.WeaponType.CompareTo(y.WeaponType);
            });
            return cardInfos;
        }
        public void SendStartBattleEvent(string amplitudeEvent,AmplitudePlacementProperty prop, BathData data)
        {
#if UNITY_ANDROID||UNITY_IOS && !UNITY_EDITOR
            
            amplitude.logEvent(amplitudeEvent.ToString(), new Dictionary<string, object>()
            {
                {AmplitudeCommonProperties.session_count.ToString(), AnalyticsSessionInfo.sessionCount},
                {AmplitudeCommonProperties.day_count.ToString(), _timeSinceFirstPlay.Days+1},
                {AmplitudePlacement.placement.ToString(), prop},
                {AmplitudePlacement.bath_id.ToString(), data.CrocodileBathType}
            });
            FirebaseAnalytics.LogEvent(amplitudeEvent.ToString(), new Parameter[]
            {
                new Parameter(AmplitudeCommonProperties.session_count.ToString(), AnalyticsSessionInfo.sessionCount),
                new Parameter(AmplitudeCommonProperties.day_count.ToString(), _timeSinceFirstPlay.Days+1),
                new Parameter(AmplitudePlacement.placement.ToString(), prop.ToString()),
                new Parameter(AmplitudePlacement.bath_id.ToString(), data.CrocodileBathType.ToString())
            });
#endif
        }
        
        public void SendStartBossBattleEvent(string amplitudeEvent, BathData data)
        {
#if UNITY_ANDROID||UNITY_IOS && !UNITY_EDITOR
            amplitude.logEvent(amplitudeEvent.ToString(), new Dictionary<string, object>()
            {
                {AmplitudeCommonProperties.session_count.ToString(), AnalyticsSessionInfo.sessionCount},
                {AmplitudeCommonProperties.day_count.ToString(), _timeSinceFirstPlay.Days+1},
                {AmplitudePlacement.bath_id.ToString(), data.CrocodileBathType}

            });
            
            FirebaseAnalytics.LogEvent(amplitudeEvent.ToString(), new Parameter[]
            {
                new Parameter(AmplitudeCommonProperties.session_count.ToString(), AnalyticsSessionInfo.sessionCount),
                new Parameter(AmplitudeCommonProperties.day_count.ToString(), _timeSinceFirstPlay.Days+1),
                new Parameter(AmplitudePlacement.bath_id.ToString(), data.CrocodileBathType.ToString())
            });
#endif
        }
        

        public void SendEvent(string amplitudeEvent, Dictionary<string, object> properties)
        {
#if UNITY_ANDROID||UNITY_IOS && !UNITY_EDITOR
            Dictionary<string, object> commonProps = new Dictionary<string, object>()
            {
                {AmplitudeCommonProperties.session_count.ToString(), AnalyticsSessionInfo.sessionCount},
                {AmplitudeCommonProperties.day_count.ToString(), _timeSinceFirstPlay.Days+1}
            };
            foreach (var key in properties.Keys)
            {
                commonProps.Add(key, properties[key]);
            }

            List<Parameter> fireBaseParameters = new List<Parameter>();

            foreach (var key in commonProps.Keys)
            {
                fireBaseParameters.Add(new Parameter(key, commonProps[key].ToString()));
            }

            amplitude.logEvent(amplitudeEvent, commonProps);
            FirebaseAnalytics.LogEvent(amplitudeEvent, fireBaseParameters.ToArray());
#endif
        }

        public void SendEvent(string amplitudeEvent)
        {
#if UNITY_ANDROID||UNITY_IOS && !UNITY_EDITOR
            Dictionary<string, object> commonProps = new Dictionary<string, object>()
            {
                {AmplitudeCommonProperties.session_count.ToString(), AnalyticsSessionInfo.sessionCount},
                {AmplitudeCommonProperties.day_count.ToString(), _timeSinceFirstPlay.Days+1}
            };

            List<Parameter> fireBaseParameters = new List<Parameter>();

            foreach (var key in commonProps.Keys)
            {
                fireBaseParameters.Add(new Parameter(key, commonProps[key].ToString()));
            }

            amplitude.logEvent(amplitudeEvent, commonProps);
            FirebaseAnalytics.LogEvent(amplitudeEvent, fireBaseParameters.ToArray());
#endif
        }
    }

    public interface IAnalytics : IService
    {

        void SendStartBattleEvent(string amplitudeEvent, AmplitudePlacementProperty prop, BathData data);
        void SendStartBossBattleEvent(string amplitudeEvent, BathData data);
        void SendEvent(string amplitudeEvent, Dictionary<string, object> properties);
        void SendEvent(string amplitudeEvent);

    }

    public class AmplitudeEvent
    {

        #region General

        public const string game_start = "game_start";
        public const string auto_fight_start = "auto-fight_start";
        public const string auto_fight_end = "auto-fight_end";
        public const string game_end = "game_end";
        public const string chest_start = "chest_start";
        public const string chest_collect = "chest_collect";
        public const string auto_equip = "auto-equip";
        public const string equipment_merge = "equipment_merge";
        public const string game_boss_start = "game_boss_start";
        public const string game_boss_end = "game_boss_end";
        public const string booster_use = "booster_use";
        public const string battle_pass_step_end = "battle-pass_step_end";
        public const string battle_pass_step_collect = "battle-pass_step_collect";
        public const string daily_complete = "daily_complete";
        public const string daily_collect = "daily_collect";
        public const string daily_all_collect = "daily-all_collect";
        public const string achievement_complete = "acjievement_complete";
        public const string achievement_collect = "achievement_collect";
        
    
        #endregion

        #region Ad

        public const string ad_reward_show = "ad_reward_show";
        public const string ad_reward_shown = "ad_reward_shown";
        public const string ad_reward_cancel = "ad_reward_cancel";
        public const string ad_interstitial_show = "ad_interstitial_show";
        public const string ad_interstitial_shown = "ad_interstitial_shown";
    
        #endregion

        #region UI_screens

        public const string ui_chests_show = "ui_chests_show";
        public const string ui_settings_show = "ui_settings_show";
        public const string ui_merge_show = "ui_merge_show";
        public const string ui_result_show = "ui_result_show";
        public const string ui_result_collect = "ui_result_collect";
        public const string ui_result_close = "ui_result_close";
        public const string ui_chest_timer_show = "ui_chest-timer_show";
        public const string ui_change_bath_show = "ui_change_bath_show";
        public const string ui_change_weapon_show = "ui_change_weapon_show";
        public const string ui_shop_show = "ui_shop_show";
        public const string ui_battle_pass_show = "ui_battle-pass_show";
        public const string ui_daily_show = "ui_daily_show";

        #endregion

        #region SHOP

        public const string shop_item_buy = "shop_item_buy";
        public const string shop_item_buy_complete = "shop_item_buy_complete";
        public const string shop_item_buy_cancel = "shop_item_buy_cancel";
        public const string shop_item_buy_fail = "shop_item_buy_fail";


        #endregion
    }

    public class AmplitudeTutorialEvent
    {
        public const string tutor_game_start = "tutor_game_start";
        public const string tutor_game_dialog_1_start = "tutor_game_dialog-1_start";
        public const string tutor_game_dialog_1_complete = "tutor_game_dialog-1_complete";
        public const string tutor_game_weapon_1_start = "tutor_game_weapon-1_start";
        public const string tutor_game_weapon_1_complete = "tutor_game_weapon-1_complete";
        public const string tutor_game_mana_start = "tutor_game_mana_start";
        public const string tutor_game_mana_complete = "tutor_game_mana_complete";
        public const string tutor_game_round_1_start = "tutor_game_round-1_start";
        public const string tutor_game_round_1_complete = "tutor_game_round-1_complete";
        public const string tutor_game_dialog_2_start = "tutor_game_dialog-2_start";
        public const string tutor_game_dialog_2_complete = "tutor_game_dialog-2_complete";
        public const string tutor_game_weapon_2_start = "tutor_game_weapon-2_start";
        public const string tutor_game_weapon_2_complete = "tutor_game_weapon-2_complete";
        public const string tutor_game_dialog_3_start = "tutor_game_dialog-3_start";
        public const string tutor_game_dialog_3_complete = "tutor_game_dialog-3_complete";
        public const string tutor_game_weapon_3_start = "tutor_game_weapon-3_start";
        public const string tutor_game_weapon_3_complete = "tutor_game_weapon-3_complete";
        public const string tutor_game_round_2_start = "tutor_game_round-2_start";
        public const string tutor_game_round_2_complete = "tutor_game_round-2_complete";
        public const string tutor_game_dialog_4_start = "tutor_game_dialog-4_start";
        public const string tutor_game_dialog_4_complete = "tutor_game_dialog-4_complete";
        public const string tutor_game_weapon_4_start = "tutor_game_weapon-4_start";
        public const string tutor_game_weapon_4_complete = "tutor_game_weapon-4_complete";
        public const string tutor_game_dialog_5_start = "tutor_game_dialog-5_start";
        public const string tutor_game_dialog_5_complete = "tutor_game_dialog-5_complete";
        public const string tutor_game_weapon_5_start = "tutor_game_weapon-5_start";
        public const string tutor_game_weapon_5_complete = "tutor_game_weapon-5_complete";
        public const string tutor_game_round_3_start = "tutor_game_round-3_start";
        public const string tutor_game_round_3_complete = "tutor_game_round-3_complete";
        public const string tutor_game_dialog_6_start = "tutor_game_dialog-6_start";
        public const string tutor_game_dialog_6_complete = "tutor_game_dialog-6_complete";
        public const string tutor_game_weapon_6_start = "tutor_game_weapon-6_start";
        public const string tutor_game_weapon_6_complete = "tutor_game_weapon-6_complete";
        public const string tutor_game_round_4_start = "tutor_game_round-4_start";
        public const string tutor_game_round_4_complete = "tutor_game_round-4_complete";
        public const string tutor_game_dialog_7_start = "tutor_game_dialog-7_start";
        public const string tutor_game_dialog_7_complete = "tutor_game_dialog-7_complete";
        public const string tutor_game_result_start = "tutor_game_result_start";
        public const string tutor_game_result_complete = "tutor_game_result_complete";
        public const string tutor_game_complete = "tutor_game_complete";
    
    
        public const string tutor_chest_start = "tutor_chest_start";
        public const string tutor_chest_dialog_1_start = "tutor_chest_dialog-1_start";
        public const string tutor_chest_dialog_1_complete = "tutor_chest_dialog-1_complete";
        public const string tutor_chest_dialog_2_start = "tutor_chest_dialog-2_start";
        public const string tutor_chest_dialog_2_complete = "tutor_chest_dialog-2_complete";
        public const string tutor_chest_open_start = "tutor_chest_open_start";
        public const string tutor_chest_open_complete = "tutor_chest_open_complete";
        public const string tutor_chest_complete = "tutor_chest_complete";
    
    
        public const string tutor_weapon_start = "tutor_weapon_start";
        public const string tutor_weapon_button_start = "tutor_weapon_button_start";
        public const string tutor_weapon_button_complete = "tutor_weapon_button_complete";
        public const string tutor_weapon_dialog_start = "tutor_weapon_dialog_start";
        public const string tutor_weapon_dialog_complete = "tutor_weapon_dialog_complete";
        public const string tutor_weapon_change_start = "tutor_weapon_change_start";
        public const string tutor_weapon_change_complete = "tutor_weapon_change_complete";
        public const string tutor_weapon_auto_start = "tutor_weapon_auto_start";
        public const string tutor_weapon_auto_complete = "tutor_weapon_auto_complete";
        public const string tutor_weapon_complete = "tutor_weapon_complete";
        
        public const string tutor_bath_start = "tutor_bath_start";
        public const string tutor_bath_button_start = "tutor_bath_button_start";
        public const string tutor_bath_button_complete = "tutor_bath_button_complete";
        public const string tutor_bath_dialog_start = "tutor_bath_dialog_start";
        public const string tutor_bath_dialog_complete = "tutor_bath_dialog_complete";
        public const string tutor_bath_change_start = "tutor_bath_change_start";
        public const string tutor_bath_change_complete = "tutor_bath_change_complete";
        public const string tutor_bath_complete = "tutor_bath_complete";
        
        public const string tutor_game_button_start = "tutor_game_button_start";
        public const string tutor_game_button_complete = "tutor_game_button_complete";
        
        public const string tutor_new_chest_start = "tutor_new_chest_start";
        public const string tutor_new_chest_dialog_start = "tutor_new_chest_dialog_start";
        public const string tutor_new_chest_dialog_complete = "tutor_new_chest_dialog_complete";
        public const string tutor_new_chest_pointer_start = "tutor_new_chest_pointer_start";
        public const string tutor_new_chest_pointer_complete = "tutor_new_chest_pointer_complete";
        public const string tutor_new_chest_open_start = "tutor_new_chest_open_start";
        public const string tutor_new_chest_open_complete = "tutor_new_chest_open_complete";
        public const string tutor_new_chest_complete = "tutor_new_chest_complete";


        public const string tutor_merge_start = "tutor_merge_start";
        public const string tutor_merge_dialog_1_start = "tutor_merge_dialog-1_start";
        public const string tutor_merge_dialog_1_complete = "tutor_merge_dialog-1_complete";
        public const string tutor_merge_button_start = "tutor_merge_button_start";
        public const string tutor_merge_button_complete = "tutor_merge_button_complete";
        public const string tutor_merge_dialog_2_start = "tutor_merge_dialog-2_start";
        public const string tutor_merge_dialog_2_complete = "tutor_merge_dialog-2_complete";
        public const string tutor_merge_card_1_start = "tutor_merge_card-1_start";
        public const string tutor_merge_card_1_complete = "tutor_merge_card-1_complete";
        public const string tutor_merge_card_2_start = "tutor_merge_card-2_start";
        public const string tutor_merge_card_2_complete = "tutor_merge_card-2_complete";
        public const string tutor_merge_complete = "tutor_merge_complete";
    
    
        public const string tutor_boss_start = "tutor_boss_start";
        public const string tutor_boss_dialog_start = "tutor_boss_dialog_start";
        public const string tutor_boss_dialog_complete = "tutor_boss_dialog_complete";
        public const string tutor_boss_button_start = "tutor_boss_button_start";
        public const string tutor_boss_button_complete = "tutor_boss_button_complete";
        public const string tutor_boss_complete = "tutor_boss_complete";
    }


    public enum AmplitudeCommonProperties
    {
        None = 0,
        day_count = 1,
        session_count = 2,
    }
    
    public enum AmplitudePlacement
    {
        NONE = 0,
        placement = 1,
        bath_id = 2,
        result = 3,
        type = 4,
        id = 5,
        lvl = 6,
        source = 7
    }

    public enum AmplitudePlacementProperty
    {
        NONE = 0,
        add_card = 1,
        free_chest = 2,
        show_enemy_cards = 3,
        change_enemy = 4,
        chest_timer = 5,
        icon = 6,
        shade = 7,
        basic = 8,
        big = 9,
        special = 10,
        Ok = 11,
        battle = 12,
        back = 13,
        win = 14,
        loose = 15,
        leave = 16,
        garage = 17,
        main = 18,
        usual = 19,
        boss = 20,
        result_screen = 21,
        draw = 22,
        home
    }
}