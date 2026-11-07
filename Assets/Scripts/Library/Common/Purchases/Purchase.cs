using System;
using System.Collections.Generic;
using App.Analytic;
using App.Chests;
using App.Counters;
using App.DAO;
using App.Dialogs.BattlePass;
using App.PremPurchases;
using Library.Dialogs;
using UnityEngine;

namespace Library.Common.Purchases
{
    public class Purchase
    {
        private const string PURCHASE_COUNT_PREF = "PremPurchaseUtils_purchase_count";
        private const string LAST_BUYING_TIME_PREF = "PremPurchaseUtils_last_purchase_at";
        public event Action<Purchase> Bought;
        
        private readonly PremPurchase _purchaseSetup;
        private readonly PremPurchaseInfo _purchaseInfoSetup;
        private readonly IDataService _dataService;
        private readonly PurchaseTransactionController _transactionController;
        private readonly IAnalytics _analytics;
        private readonly AdsHelper _adsHelper;
        private readonly DialogsContainer _dialogsContainer;
        private readonly CounterUtility _counterUtility;
        private readonly BattlePassUtility _battlePassUtility;

        public string Sku => _purchaseSetup.Sku;
        public string NameKey => _purchaseInfoSetup.NameKey;
        public string Descriptions => _purchaseInfoSetup.Descriptions;
        public long Amount => _purchaseInfoSetup.Amount;
        public Sprite SmallIcon => _purchaseInfoSetup.SmallIcon;
        public bool IsX2Amount => AmountMultiplier == 2;
        private DateTime LastBuyingTime
        {
            get => _dataService.GetDateTime($"{LAST_BUYING_TIME_PREF}{_purchaseSetup.Type}");
            set => _dataService.SetDateTime($"{LAST_BUYING_TIME_PREF}{_purchaseSetup.Type}", value);
        }
        private long PurchasedCount
        {
            get => _dataService.GetLong($"{PURCHASE_COUNT_PREF}{_purchaseSetup.Type}");
            set => _dataService.SetLong($"{PURCHASE_COUNT_PREF}{_purchaseSetup.Type}", value);
        }
        private int AmountMultiplier => 
            _purchaseSetup.GiveX2ForFirstTime && PurchasedCount == 0 ? 2 : 1;

        
        public Purchase(PremPurchase purchaseSetup, PremPurchaseInfo purchaseInfoSetup, IDataService dataService,
            PurchaseTransactionController transactionController, IAnalytics analytics, AdsHelper adsHelper, 
            DialogsContainer dialogsContainer, CounterUtility counterUtility, BattlePassUtility battlePassUtility)
        {
            _purchaseSetup = purchaseSetup;
            _purchaseInfoSetup = purchaseInfoSetup;
            _dataService = dataService;
            _transactionController = transactionController;
            _analytics = analytics;
            _adsHelper = adsHelper;
            _dialogsContainer = dialogsContainer;
            _counterUtility = counterUtility;
            _battlePassUtility = battlePassUtility;
        }

        public void Buy(Action callback)
        {
            callback = OnPurchaseCompleted + callback;
            _analytics.SendEvent(AmplitudeEvent.shop_item_buy, new Dictionary<string, object>(){{"item_id", _purchaseSetup.Sku}});
            Debug.Log($"[Purchase] Buy purchase request {_purchaseSetup.Type}");
            switch (_purchaseSetup.PriceType)
            {
                case PurchasePriceType.BaseMoney: OnPurchaseCompleted(); break;
                case PurchasePriceType.RealMoney: _transactionController.MakePurchase(_purchaseSetup.Sku, callback); break;
                case PurchasePriceType.AD: callback.Invoke(); break;
            }
        }

        public void Restore() => ApplyPurchase();

        public string GetPriceText()
        {
            switch (_purchaseSetup.PriceType)
            {
                case PurchasePriceType.BaseMoney:
                    return _purchaseSetup.Price.ToString();
                case PurchasePriceType.RealMoney:
                    string priceString = _transactionController.GetPriceString(_purchaseSetup.Sku);
                    return string.IsNullOrEmpty(priceString) ? $"{(float)_purchaseSetup.Price / 100}$" : priceString;
                case PurchasePriceType.AD:
                    return "FREE";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public PurchaseStatus GetStatus()
        {
            PurchaseStatus status = PurchaseStatus.PurchaseLimitReached;
            if (IsPurchaseBought() == false || IsPurchaseConsumable())
            {
                status = PurchaseStatus.Available;
            }

            if (_purchaseSetup.HasCooldown && GetRemainingCooldownTime().TotalSeconds > 0)
            {
                status = PurchaseStatus.OnCooldown;
            }
            
            return status;
        }

        public TimeSpan GetRemainingCooldownTime() => 
            LastBuyingTime + TimeSpan.FromSeconds(_purchaseSetup.Cooldown) - DateTime.UtcNow;

        public void Reset()
        {
            LastBuyingTime = default;
            PurchasedCount = default;
        }

        private bool IsPurchaseConsumable() =>
            _transactionController.IsPurchaseConsumable(_purchaseSetup.Sku);

        private bool IsPurchaseBought() => PurchasedCount > 0;

        private void OnPurchaseCompleted()
        {
            Debug.Log($"[Purchase] PurchaseCompleted {_purchaseSetup.Type}");
            _analytics.SendEvent(AmplitudeEvent.shop_item_buy_complete, new Dictionary<string, object>{{"item_id", _purchaseSetup.Sku}});
            ApplyPurchase();
            LastBuyingTime = DateTime.UtcNow;
            PurchasedCount++;
            Bought?.Invoke(this);
        }

        private void ApplyPurchase()
        {
            switch (_purchaseSetup.Type)
            {
                case PremPurchaseType.FREE_MONEY:
                case PremPurchaseType.SMALL_MONEY_PACK:
                case PremPurchaseType.MEDIUM_MONEY_PACK:
                case PremPurchaseType.BIG_MONEY_PACK:
                case PremPurchaseType.HUGE_MONEY_PACK:
                case PremPurchaseType.FANTASTIC_PACK:
                    TakeMoney(_purchaseInfoSetup);
                    break;
                case PremPurchaseType.CATOR_KIT:
                case PremPurchaseType.CROCO_KIT:
                case PremPurchaseType.CAYMAN_KIT:
                    TakeBoosters(_purchaseInfoSetup);
                    break;
                case PremPurchaseType.NO_ADS:
                    _dataService.SetLong($"{PURCHASE_COUNT_PREF}{_purchaseSetup.Type}", 1);
                    RemoveAd();
                    break;
                case PremPurchaseType.MINI_CHEST:
                    TakeChest(_purchaseSetup, _purchaseInfoSetup, ChestType.MINI);
                    break;
                case PremPurchaseType.MEDIUM_CHEST:
                    TakeChest(_purchaseSetup, _purchaseInfoSetup, ChestType.MEDIUM);
                    break;
                case PremPurchaseType.MEGA_GHEST:
                    TakeChest(_purchaseSetup, _purchaseInfoSetup, ChestType.MEGA);
                    break;
                case PremPurchaseType.GIGA_CHEST:
                    TakeChest(_purchaseSetup, _purchaseInfoSetup, ChestType.GIGA);
                    break;
                case PremPurchaseType.BATTLE_PASS:
                    TakeBattlePass();
                    break;
            }
        }

        #region PurchaseApplyingMethods

        private void TakeMoney(PremPurchaseInfo purchaseInfo)
        {
            var dialog = _dialogsContainer.ShowPurchasingNotification();
            dialog.Setup(purchaseInfo.SmallIcon, purchaseInfo.Amount * AmountMultiplier);
            _counterUtility.AppendMoney(CounterType.DEFAULT_MONEY, purchaseInfo.Amount * AmountMultiplier);
        }

    private void TakeBoosters(PremPurchaseInfo purchaseInfo)
        {
            _dialogsContainer.ShowPurchasingNotification().Setup(purchaseInfo.SmallIcon, purchaseInfo.Amount * AmountMultiplier);
            foreach (BoosterInfo booster in _dataService.GetBoosterCollectionData().BoosterCollection)
            {
                booster.BoosterAmount += purchaseInfo.Amount * AmountMultiplier;
            }
            _dataService.UpdateBoosterCollectionData();
        }

        private void TakeChest(PremPurchase purchase, PremPurchaseInfo purchaseInfo, ChestType chestType)
        {
            if(_counterUtility.GetMoney(CounterType.DEFAULT_MONEY) < purchase.Price)
                return;
            _counterUtility.AppendMoney(CounterType.DEFAULT_MONEY, -purchase.Price);
            PurchasedDialog dialog = _dialogsContainer.ShowPurchasingNotification();
            dialog.Setup(purchaseInfo.SmallIcon, purchaseInfo.Amount);
            dialog.SetupButton(() => OpenChest(chestType));
        }

        private void OpenChest(ChestType chestType)
        {
            var dialog = _dialogsContainer.ShowTakeCardsDialog();
            dialog.Setup(chestType, false);
        }

        private void RemoveAd() => 
            _adsHelper.IsNoAdsBought = true;

        private void TakeBattlePass() => 
            _battlePassUtility.ActivateBattlePass();

        #endregion

        public enum PurchaseStatus
        {
            None = 0,
            Available = 1,
            PurchaseLimitReached = 2,
            OnCooldown = 3,
        }
    }
}