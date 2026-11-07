using System;
using System.Collections.Generic;
using System.Linq;
using App.Analytic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using Zenject;

namespace Library.Common.Purchases
{
    [DisallowMultipleComponent]
    public class PurchaseTransactionController : MonoBehaviour
    {
        public event Action<IBillingTransaction[]> PurchasesRestored
        {
            add
            {
                _purchaseRestored += value;
                if(_isPurchaseRestored) value?.Invoke(_restoredTransactions);
            }
            remove => _purchaseRestored -= value;
        }

        private Action<IBillingTransaction[]> _purchaseRestored;
        private Action OnCompletedPurchaseAction;
        private bool _isPurchaseRestored;
        private IBillingTransaction[] _restoredTransactions;
        private IBillingProduct[] _products;

        [Inject] private IAnalytics _analytics;

        public void MakePurchase(string sku, Action completePurchase)
        {
            Debug.Log($"[PremPurchaseHelper] MakePurchase: {sku}");
            if (BillingServices.IsAvailable() == false)
            {
                Debug.LogError("Billing is not available.");
                return;
            }

            OnCompletedPurchaseAction = completePurchase;
            if (_products == null) LoadProducts();

            IBillingProduct product = _products.FirstOrDefault(tmpProduct => sku.Equals(tmpProduct.Id));

            if (product == null)
                Debug.LogError($"No BillingProduct found for Purchase {sku}.");
            Debug.Log($"[PremPurchaseHelper] MakePurchase: {product}");
            if (BillingServices.IsProductPurchased(product) == false)
            {
                if (BillingServices.CanMakePayments())
                    BillingServices.BuyProduct(sku);
            }
            else
                Debug.Log($"{sku} is already purchased.");
        }

        public bool IsPurchaseConsumable(string sku)
        {
            BillingProductDefinition product = BillingServices.ProductDefinitions.FirstOrDefault(tmpProduct => sku.Equals(tmpProduct.Id));
            if (product == null) return true;
            return product.ProductType == BillingProductType.Consumable;
        }

        public string GetPriceString(string sku)
        {
            if (_products == null) return string.Empty;
            var product = _products.FirstOrDefault(p => sku.Equals(p.Id));
            return product == null ? string.Empty : product.LocalizedPrice;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void OnEnable()
        {
            if (!BillingServices.IsAvailable()) return;
            // register for events
            BillingServices.OnInitializeStoreComplete   += OnInitializeStoreComplete;
            BillingServices.OnTransactionStateChange    += OnTransactionStateChange;
            BillingServices.OnRestorePurchasesComplete  += OnRestorePurchasesComplete;
            
            //BillingServices.InitializeStore();
            //BillingServices.RestorePurchases();
        }

        public void InitPurchaseService()
        {
            BillingServices.InitializeStore();
            BillingServices.RestorePurchases();
        }

        private void OnDisable()
        {
            if (!BillingServices.IsAvailable()) return;
            BillingServices.OnInitializeStoreComplete   -= OnInitializeStoreComplete;
            BillingServices.OnTransactionStateChange    -= OnTransactionStateChange;
            BillingServices.OnRestorePurchasesComplete  -= OnRestorePurchasesComplete;
        }

        private void LoadProducts()
        {
            if (BillingServices.IsAvailable())
            {
                Debug.Log("[PremPurchaseHelper] LoadProducts: c=" + BillingServices.Products.Length);
                BillingServices.InitializeStore((BillingServices.ProductDefinitions));
                _products = BillingServices.Products;
            }
        }

        private void OnInitializeStoreComplete(BillingServicesInitializeStoreResult result, Error error)
        {
            if (error == null)
            {
                // update UI
                // show console messages
                var     products    = result.Products;
                Debug.Log("Store initialized successfully.");
                Debug.Log("Total products fetched: " + products.Length);
                Debug.Log("Below are the available products:");
                for (int iter = 0; iter < products.Length; iter++)
                {
                    var     product = products[iter];
                    Debug.Log(string.Format("[{0}]: {1}", iter, product));
                }
            }
            else
            {
                Debug.Log("Store initialization failed with error. Error: " + error);
            }

            var     invalidIds  = result.InvalidProductIds;
            Debug.Log("Total invalid products: " + invalidIds.Length);
            if (invalidIds.Length > 0)
            {
                Debug.Log("Here are the invalid product ids:");
                for (int iter = 0; iter < invalidIds.Length; iter++)
                {
                    Debug.Log($"[{iter}]: {invalidIds[iter]}");
                }
            }
        }

        private void OnTransactionStateChange(BillingServicesTransactionStateChangeResult result)
        {
            var     transactions    = result.Transactions;
            for (int iter = 0; iter < transactions.Length; iter++)
            {
                var     transaction = transactions[iter];
                switch (transaction.TransactionState)
                {
                    case BillingTransactionState.Purchased:
                        if (transaction.ReceiptVerificationState == BillingReceiptVerificationState.Success)
                        {
                            OnCompletedPurchaseAction?.Invoke();
                            OnCompletedPurchaseAction = null;
                        }
                        Debug.Log(string.Format("Buy product with id:{0} finished successfully.", transaction.Payment.ProductId));
                        break;
                    
                    case BillingTransactionState.Failed:
                        if (transaction.Error.Code == 0) //??
                        {
                            _analytics.SendEvent(AmplitudeEvent.shop_item_buy_cancel, new Dictionary<string, object>(){{"item_id", transaction.Payment.ProductId}});
                        }
                        else
                        {
                            _analytics.SendEvent(AmplitudeEvent.shop_item_buy_fail, new Dictionary<string, object>(){
                                {"item_id", transaction.Payment.ProductId},
                                {"error_code", transaction.Error.Code},
                                {"error_description", transaction.Error.Description}});
                        }
                        Debug.Log(string.Format("Buy product with id:{0} failed with error. Error: {1}", transaction.Payment.ProductId, transaction.Error));
                        OnCompletedPurchaseAction = null;
                        break;
                }
            }
        }

        private void OnRestorePurchasesComplete(BillingServicesRestorePurchasesResult result, Error error)
        {
            if (error == null)
            {
                var     transactions    = result.Transactions;
                Debug.Log("Request to restore purchases finished successfully.");
                Debug.Log("Total restored products: " + transactions.Length);
                for (int iter = 0; iter < transactions.Length; iter++)
                {
                    var     transaction = transactions[iter];
                    Debug.Log(string.Format("[{0}]: {1}", iter, transaction.Payment.ProductId));
                }

                _isPurchaseRestored = true;
                _restoredTransactions = result.Transactions;
                _purchaseRestored?.Invoke(result.Transactions);
            }
            else
            {
                Debug.Log("Request to restore purchases failed with error. Error: " +  error);
            }
        }
    }
}