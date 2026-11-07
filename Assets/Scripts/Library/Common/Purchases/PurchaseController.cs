using System;
using System.Collections.Generic;
using System.Linq;
using App.Analytic;
using App.Counters;
using App.DAO;
using App.Dialogs.BattlePass;
using App.PremPurchases;
using CodeBase.Infrastructure;
using Library.Dialogs;
using VoxelBusters.EssentialKit;
using Zenject;

namespace Library.Common.Purchases
{
    public class PurchaseController
    {
        public event Action<Purchase> PurchaseBought;
        private readonly Dictionary<PremPurchaseType, Purchase> _purchases;
        private readonly PurchaseTransactionController _transactionController;

        [Inject]
        public PurchaseController(PurchaseTransactionController transactionController, PremPurchasesSetup purchaseSetup, 
            PremPurchasesInfoSetup purchaseInfoSetup, IDataService dataService, IAnalytics analytics, GameBootstrapper bootstrapper, 
            DialogsContainer dialogsContainer, CounterUtility counterUtility, BattlePassUtility battlePassUtility)
        {
            _purchases = new Dictionary<PremPurchaseType, Purchase>();
            _transactionController = transactionController;
            foreach (PremPurchase purchase in purchaseSetup.GetAll())
            {
                Purchase purchaseInstance = new Purchase(purchase, purchaseInfoSetup.Get(purchase.Type), dataService,
                    transactionController, analytics, bootstrapper.AdsHelper, dialogsContainer, counterUtility, battlePassUtility);
                _purchases.Add(purchase.Type, purchaseInstance);
                purchaseInstance.Bought += OnPurchaseBought;
            }
            
            transactionController.PurchasesRestored += OnPurchasesRestored;
        }

        public Purchase GetPurchase(PremPurchaseType type) => 
            _purchases[type];

        public void ResetPurchases()
        {
            foreach (Purchase purchase in _purchases.Values)
            {
                purchase.Reset();
            }
        }
        
        private void OnPurchaseBought(Purchase obj) => 
            PurchaseBought?.Invoke(obj);

        private Purchase GetPurchaseBySku(string sku)
        {
            return _purchases.First(p => sku.Equals(p.Value.Sku)).Value;
        }

        private void OnPurchasesRestored(IBillingTransaction[] transactions)
        {
            _transactionController.PurchasesRestored -= OnPurchasesRestored;
            foreach (IBillingTransaction transaction in transactions)
            {
                GetPurchaseBySku(transaction.Payment.ProductId).Restore();
            }
        }
    }
}