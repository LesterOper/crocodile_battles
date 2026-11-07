using System;
using System.IO;
using System.Linq;
using App.Achievements;
using App.Cards;
using App.Chests;
using App.Counters;
using App.DAO;
using App.Dialogs.BattlePass;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using Library.Common.Purchases;
using Library.Utils;
using UnityEngine;
using Zenject;

namespace Core.Cheats
{
    /// <summary>
    /// Module for creating in-game cheats. Before any usage Initialize must be called.
    /// Supports following controls:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             Label
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Button
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Toggle
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Slider (integer, float)
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Button with input field (integer, float, string)
    ///         </description>
    ///     </item>
    /// </list>
    /// Allows to split controls by tabs and sections (folds). See usage example in <see cref="Cheats.InitDemo"/> source.
    /// </summary>
    public class Cheats : MonoBehaviour
    {
        internal static bool IsEnabled;

        internal static CheatsWindow window;

        private const int WidthPerChar = 30;
        private static IDataService _dataService;
        private static SignalBus _signalBus;
        private static CounterUtility _counterUtility;
        private static BattlePassUtility _battlePassUtility;
        private static PurchaseController _purchaseController;

        /// <summary>
        /// Cheats was shown. See also <see cref="Show"/> and <see cref="Hide"/>.
        /// </summary>
        public static bool IsShown => window.IsShown;

        /// <summary>
        /// Was cheats initialized?
        /// </summary>
        public static bool IsInitialized => window != null;
        
        public static void Initialize(RectTransform container, bool enabled)
        {
            if (IsInitialized) return;
            
            IsEnabled = enabled;

            var windowPrefab = Resources.Load<GameObject>(Path.Combine("UI", "Cheats", "CheatsWindow"));
            window = Instantiate(windowPrefab).GetComponent<CheatsWindow>();
            window.transform.SetParent(container);
            window.Hide();
        }

        public static void DeInitialize()
        {
            if (!IsInitialized) return;
            
            Destroy(window);

            window = null;
        }

        /// <summary>
        /// Show cheats window.
        /// </summary>
        public static void Show()
        {
            if (!IsEnabled) return;
            
            window.Show();
        }

        /// <summary>
        /// Hide cheats window.
        /// </summary>
        public static void Hide() => window.Hide();

        /// <summary>
        /// Return <see cref="CheatsSection"/> object (create new or fetch cached) for adding controls to it. Sections are
        /// split by tabs for handy navigation.
        /// </summary>
        /// <param name="header">Section header</param>
        /// <param name="tab">Cheats tab section belongs to</param>
        /// <returns>Newly created or cached cheats section with specified <paramref name="header"/> and <paramref name="tab"/></returns>
        public static CheatsSection GetSection(string header, string tab = null) => window.GetSection(header, tab);

        private static void AddConcreteWeapon(WeaponType type, int level)
        {
            if(level < 1 || level > 10)
                return;
            _dataService.GetCardCollectionData().Collection.Add(new CardInfo()
            {
                WeaponType = type,
                type = type.GetAttribute<WeaponData>().ActionType,
                Level = level
            });
            _dataService.UpdateCardCollectionData();
        }

        private static void AddConcreateBath(BathType type, int level)
        {
            if(level < 1 || level > 9)
                return;
            _dataService.GetBathCollection().BathDatas.Add( new BathInCollectionData()
            {
                CrocodileBathType = type,
                Level = level
            });
            _dataService.UpdateBathCollection();
        }

        private static void AddConcreateChest(ChestType chestType, int amount)
        {
            if(amount == 0)
                return;
            _dataService.GetChestCollectionData().ChestCollection.FirstOrDefault(chest => chest.type == chestType)
                .chestAmount += amount;
            _dataService.UpdateChestCollectionData();
            _signalBus.Fire<UpdateChestSlotsSignal>();
        }

        private static void AddMoney(CounterType counterType, int amount)
        {
            _counterUtility.AppendMoney(counterType, amount);
            if (counterType == CounterType.BATTLES_WIN)
            {
                _battlePassUtility.AppendBattlePassStepProgress((int)_battlePassUtility
                    .GetBattlePassStepProgress((int)_battlePassUtility.GetBattlePassCurrentStep())+amount);
            }
        }

        private static void OpenAllWeapons()
        {
            foreach (WeaponType weaponType in Enum.GetValues(typeof(WeaponType)))
            {
                if(weaponType == WeaponType.NONE)
                    continue;
                if (_dataService.GetCardCollectionData().Collection.FirstOrDefault(weapon => weapon.WeaponType == weaponType) ==
                    null)
                {
                    _dataService.GetCardCollectionData().Collection.Add(new CardInfo()
                    {
                        WeaponType = weaponType, type = weaponType.GetAttribute<WeaponData>().ActionType,
                        Level = 1
                    });
                }
            }
            _dataService.UpdateCardCollectionData();
        }

        private static void OpenAllBath()
        {
            foreach (BathType bathType in Enum.GetValues(typeof(BathType)))
            {
                if(bathType == BathType.NONE)
                    continue;
                if (_dataService.GetBathCollection().BathDatas.FirstOrDefault(bath => bath.CrocodileBathType == bathType) ==
                    null)
                {
                    _dataService.GetBathCollection().BathDatas.Add(new BathInCollectionData()
                    {
                        CrocodileBathType = bathType,
                        Level = 1
                    });
                }
            }
            _dataService.UpdateBathCollection();
        }
        
        public static void InitCrocoCheats(IDataService dataService, SignalBus signalBus, CounterUtility counterUtility, 
            BattlePassUtility battlePassUtility, PurchaseController purchaseController)
        {
            _dataService = dataService;
            _signalBus = signalBus;
            _counterUtility = counterUtility;
            _battlePassUtility = battlePassUtility;
            _purchaseController = purchaseController;
            const string cardTabName = "AddCards";
            const string chestsTabName = "Chests";
            const string moneyTabName = "Money";
            const string achieves = "Achieves";

            GetSection("AddAllCards", cardTabName)
                .AddButton("AddAllWeapons", OpenAllWeapons)
                .AddButton("AddAllBaths", OpenAllBath);
            GetSection("AddConcreteWeapon", cardTabName)
                .AddButtonInt("AddBrush", v => AddConcreteWeapon(WeaponType.BRUSH, v))
                .AddLineBreaker()
                .AddButtonInt("AddPaddle", v => AddConcreteWeapon(WeaponType.PADDLE, v))
                .AddLineBreaker()
                .AddButtonInt("AddBlaster", v => AddConcreteWeapon(WeaponType.BLASTER, v))
                .AddLineBreaker()
                .AddButtonInt("AddFountain", v => AddConcreteWeapon(WeaponType.WATER_FOUNTAIN, v))
                .AddLineBreaker()
                .AddButtonInt("AddRifle", v => AddConcreteWeapon(WeaponType.RIFLE, v))
                .AddLineBreaker()
                .AddButtonInt("AddBow", v => AddConcreteWeapon(WeaponType.BOW, v))
                .AddLineBreaker()
                .AddButtonInt("AddBottleMachineGun", v => AddConcreteWeapon(WeaponType.BOTTLE_MACHINE_GUN, v))
                .AddLineBreaker()
                .AddButtonInt("AddBottleGun", v => AddConcreteWeapon(WeaponType.BOTTLE_GUN, v))
                .AddLineBreaker()
                .AddButtonInt("AddCatapult", v => AddConcreteWeapon(WeaponType.CATAPULT, v))
                .AddLineBreaker()
                .AddButtonInt("AddTestTubeGun", v => AddConcreteWeapon(WeaponType.TEST_TUBE_GUN, v))
                .AddLineBreaker()
                .AddButtonInt("AddMop", v => AddConcreteWeapon(WeaponType.MOP, v))
                .AddLineBreaker()
                .AddButtonInt("AddUmbrella", v => AddConcreteWeapon(WeaponType.UMBRELLA, v))
                .AddLineBreaker()
                .AddButtonInt("AddBobber", v => AddConcreteWeapon(WeaponType.BOBBER, v))
                .AddLineBreaker()
                .AddButtonInt("AddLifeBuoy", v => AddConcreteWeapon(WeaponType.LIFEBUOY, v))
                .AddLineBreaker()
                .AddButtonInt("AddFloatBelt", v => AddConcreteWeapon(WeaponType.FLOAT_BELT, v))
                .AddLineBreaker()
                .AddButtonInt("AddBomb", v => AddConcreteWeapon(WeaponType.BOMB_ILLNESS, v))
                .AddLineBreaker()
                .AddButtonInt("AddShower", v => AddConcreteWeapon(WeaponType.SHOWER, v))
                .AddLineBreaker()
                .AddButtonInt("AddMina", v => AddConcreteWeapon(WeaponType.MINA_BOMB, v));
            GetSection("AddConcreateBath", cardTabName)
                .AddButtonInt("AddBath_1", v => AddConcreateBath(BathType.BATH_TYPE_1, v))
                .AddLineBreaker()
                .AddButtonInt("AddBath_2", v => AddConcreateBath(BathType.BATH_TYPE_2, v))
                .AddLineBreaker()
                .AddButtonInt("AddBath_3", v => AddConcreateBath(BathType.BATH_TYPE_3, v))
                .AddLineBreaker()
                .AddButtonInt("AddBath_4", v => AddConcreateBath(BathType.BATH_TYPE_4, v))
                .AddLineBreaker()
                .AddButtonInt("AddBath_5", v => AddConcreateBath(BathType.BATH_TYPE_5, v))
                .AddLineBreaker()
                .AddButtonInt("AddBath_6", v => AddConcreateBath(BathType.BATH_TYPE_6, v))
                .AddLineBreaker()
                .AddButtonInt("AddBath_7", v => AddConcreateBath(BathType.BATH_TYPE_7, v))
                .AddLineBreaker()
                .AddButtonInt("AddBath_8", v => AddConcreateBath(BathType.BATH_TYPE_8, v))
                .AddLineBreaker()
                .AddButtonInt("AddBath_9", v => AddConcreateBath(BathType.BATH_TYPE_9, v))
                .AddLineBreaker()
                .AddButtonInt("AddBath_10", v => AddConcreateBath(BathType.BATH_TYPE_10, v))
                .AddLineBreaker();

            GetSection("AddChests", chestsTabName)
                .AddButtonInt("AddSmallChest", v => AddConcreateChest(ChestType.MINI, v))
                .AddLineBreaker()
                .AddButtonInt("AddMediumChest", v => AddConcreateChest(ChestType.MEDIUM, v))
                .AddLineBreaker()
                .AddButtonInt("AddMegaChest", v => AddConcreateChest(ChestType.MEGA, v))
                .AddLineBreaker()
                .AddButtonInt("AddGigaChest", v => AddConcreateChest(ChestType.GIGA, v))
                .AddLineBreaker();

            GetSection("AddMoney", moneyTabName)
                .AddButtonInt("AddDefaultMoney", v => AddMoney(CounterType.DEFAULT_MONEY, v))
                .AddLineBreaker()
                .AddButtonInt("AdWins", v => AddMoney(CounterType.BATTLES_WIN, v));
            
            GetSection("AchievesCheats", achieves)
                .AddButton("CompleteDailyAchieves", CompleteAchieves);

            GetSection("Other", "Other")
                .AddButton("ResetPurchases", _purchaseController.ResetPurchases)
                .AddButton("ResetBattlePassPrizes", ResetBattlePassPrizes);
            /*.AddButton("AddWeapon", () => { })
            .AddButtonInt("Level", i => { })
            .AddLineBreaker()
            .AddButton("AddBath", () => { })
            .AddButtonInt("Level", i => { });*/
        }

        private static void ResetBattlePassPrizes()
        {
            for (int i = 0; i < 40; i++)
            {
                _battlePassUtility.SetPrizeClaimed(i, false);
                _battlePassUtility.SetPremPrizeClaimed(i, false);
            }
        }

        private static void CompleteAchieves()
        {
            _signalBus.Fire(new BattleEndSignal(){IsBoss = false, IsWin = true});
            _signalBus.Fire(new ChestTakenSignal(){});
        }
        
        /// <summary>
        /// Initialize demo tabs with control examples. Use as reference for real in-game cheats.
        /// </summary>
        public static void InitDemo()
        {
            const string buttonsTabName = "Demo Buttons";
            const string labelsTabName = "Demo Labels";
            const string togglesTabName = "Demo Toggles";
            const string valueChangeTabName = "Demo Value Change Shortcut";

            void DoNothing() {}

            void DoNothingTyped<TType>(TType arg) where TType : struct {}

            GetSection("Simple Buttons", buttonsTabName)
                .AddButton("Button 1", DoNothing)
                .AddButton("Button 2", DoNothing)
                .AddButton("Button 3", DoNothing)
                .AddButton("Button 4", DoNothing)
                .AddButton("Button 5", DoNothing)
                ;

            GetSection("Another buttons", buttonsTabName)
                .AddButton("Button 1", DoNothing)
                .AddButton("Button 2", DoNothing)
                .AddButton("Button 3", DoNothing)
                .AddButton("Button 4", DoNothing)
                .AddButton("Button 5", DoNothing)
                .AddButton("Button 6", DoNothing)
                ;

            GetSection("And again", buttonsTabName)
                .AddButton("Button 1", DoNothing)
                .AddButton("Button 2", DoNothing)
                .AddButton("Button 3", DoNothing)
                .AddButton("Button 4", DoNothing)
                .AddButton("Button 5", DoNothing)
                .AddButton("Button 6 not so long", DoNothing)
                .AddButton("Button 7 that is very long", DoNothing)
                .AddButton("Button 8 it's so fucking big! Oh my god", DoNothing)
                ;

            GetSection("Line by line", buttonsTabName)
                .AddButton("Button 1", DoNothing)
                .AddLineBreaker()
                .AddButton("Button 2", DoNothing)
                .AddLineBreaker()
                .AddButton("Button 3", DoNothing)
                .AddLineBreaker()
                .AddButton("Button 4", DoNothing)
                ;

            GetSection("Labels", labelsTabName)
                .AddLabel("Label 1")
                .AddLabel("Label 2")
                .AddLabel("Label 3")
                .AddLabel("Label 4")
                .AddLabel("Label 5")
                ;

            GetSection("With bounds", labelsTabName)
                .AddLabel("Label 1", true)
                .AddLabel("Label 2", true)
                .AddLabel("Label 3", true)
                .AddLabel("Label 4 not so long", true)
                .AddLabel("Label 5 very very long label", true)
                ;

            GetSection("Dynamic", labelsTabName)
                .AddDynamicLabel(() => $"Frame count {UnityEngine.Time.frameCount}")
                .AddLineBreaker()
                .AddDynamicLabel(() => $"Time passed {UnityEngine.Time.realtimeSinceStartup:0.00}")
                ;

            GetSection("Toggles", togglesTabName)
                .AddToggle("Toggle", DoNothingTyped, () => true, true)
                .AddToggle("Long toggle", DoNothingTyped, () => true, true)
                .AddToggle("Very long toggle", DoNothingTyped, () => true, true)
                .AddToggle("Very very long toggle", DoNothingTyped, () => true, true)
                ;

            GetSection("Button + Label + Toggle", "Demo Misc")
                .AddButton("Some button", DoNothing)
                .AddLabel("some label")
                .AddToggle("some toggle", DoNothingTyped)
                ;

            GetSection("Sliders", "Demo Sliders")
                .AddSlider("Volume", DoNothingTyped, 1, 100, 50)
                .AddSlider("SFX", DoNothingTyped, 1, 100, 25, true)
                ;

            GetSection("Input fields", "Demo Buttons with Input")
                .AddButtonInt("Integer", v => Debug.Log(v))
                .AddButtonFloat("Float", v => Debug.Log(v))
                .AddButtonString("String", Debug.Log)
                ;

            GetSection("Integer, long, float", valueChangeTabName)
                .AddValueChangeShortcut(
                    "Integer", 0, DoNothingTyped, 1, 10, 100, 1000)
                .AddValueChangeShortcut(
                    "Long", (long) 100, DoNothingTyped, 1, 100, 1000000, 1000000000)
                .AddValueChangeShortcut(
                    "Float", 0f, DoNothingTyped, .001f, .05f, .25f, .5f, 1.5f)
                ;

            GetSection("Many options", valueChangeTabName)
                .AddValueChangeShortcut(
                    "Integer",
                    0,
                    DoNothingTyped,
                    1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000);
        }

        internal static float CalcTextWidth(string text) => CalcTextWidth(text.Length);

        internal static float CalcTextWidth(int length) => length * WidthPerChar;
    }
    
}