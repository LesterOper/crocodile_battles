using System;
using System.Collections.Generic;
using System.Linq;
/*using CodeBase.ApplicationLibrary.Analytics;
using CodeBase.ApplicationLibrary.Common;
using CodeBase.Infrastructure.Merge;
using CodeBase.Infrastructure.PremPurchaseDialog;*/
using MRGS;
using MRGS.Base;
using UnityEngine;

namespace GameServices.Mrgs
{
    /*public class MrgsBankController : MRGSBankDelegateEx
    {
        private const int _productsLoadFrequency = 5000;
        private bool restoring = false;

        public bool IsAvailable => MRGSBank.Instance.IsBankAvailable();
        public bool IsProductsLoaded { get; private set; }
        
        private readonly MrgsController _mrgsController;
        //private PremiumPurchaseDictionary premiumPurchaseDictionary;
        private List<MRGSBankProduct> _bankProducts = new List<MRGSBankProduct>();
        /*private string[] _consumablesProducts = new string[] {
            "crystals_1",
            "crystals_2",
            "crystals_3",
            "crystals_4",
            "crystals_5",
            "crystals_6",
            "skip_ads_small",
            "skip_ads_big",
        };
        private string[] _nonConsumablesProducts = new string[] {
            "no_ads1",
            "extra_merge_bar",
            "vip_pass",
            "prem_location",
        };#1#

        /*public string GetProductPrice(PremiumPurchaseType purchaseType)
        {
            string sku = premiumPurchaseDictionary.GetPremiumPurchaseSetup(purchaseType).GetSku();
            if (_bankProducts.Count > 0) return _bankProducts.FirstOrDefault(product => product.Sku == sku)?.Price;
            return "loading...";
        }#1#

        /*public MrgsBankController(MrgsController mrgsController, PremiumPurchaseDictionary premiumPurchaseDictionary)
        {
            _mrgsController = mrgsController;
            this.premiumPurchaseDictionary = premiumPurchaseDictionary;
            MRGSBank.getInstance().DelegateExtended = this;
            MRGSBank.getInstance().autoRestorePurchase(false);
            DownloadProductsAsync();
        }#1#

        /*private async void DownloadProductsAsync()
        {
            while (IsAvailable == false)
            {
                await Task.Delay(_productsLoadFrequency);
            }
            
            MRGSBankProductsRequest productsRequest = new MRGSBankProductsRequest();
            productsRequest.Add(_consumablesProducts, MRGSBankProductType.Consumable);
            productsRequest.Add(_nonConsumablesProducts, MRGSBankProductType.NonConsumable);
            MRGSBank.getInstance().requestProductsInfo(productsRequest);
        }#1#
        
        
        /*public bool TrySendBuyingRequest(PremiumPurchaseType purchaseType)
        {
            #if UNITY_EDITOR 
                PremIumPurchaseUtils.ApplyPurchase(purchaseType);
                //return true;
            #endif
            
            if (IsAvailable == false)
            {
                Debug.Log("Bank is not available.");
                return false;
            }
            MRGSBankPurchaseRequest purchaseRequest = MRGSBankPurchaseRequest.RequestWithProductIdentifier(premiumPurchaseDictionary.GetPremiumPurchaseSetup(purchaseType).GetSku());
            purchaseRequest.iOS.SimulatesAskToBuyInSandbox = false;
            MRGSBank.getInstance().purchaseProduct(purchaseRequest);
            Debug.Log("[MrgsBankController] Purchase request is sent");
            return true;
        }#1#
        
        public void RestorePurchase(Action OnRestoringFinished)
        {
            restoring = true;
            MRGSBank.Instance.RestorePurchases();
        }
        
        private void Restored() => restoring = false;


        public bool TryGetProductPrice(string sku, out double price)
        {
            price = 0;
            if (IsProductsLoaded == false) return false;
            
            Debug.Log("Products is not loaded.");
            if (_bankProducts.Count(p => p.Sku.Equals(sku)) <= 0) return false;
            
            var product = _bankProducts.First(p => p.Sku.Equals(sku));
            if (double.TryParse(product.Price, out price) == true) return true;
            
            Debug.Log("Price parse failed");
            return false;
        }
        
        #region MRGSBankDelegateEx implementation
        
        /*public async void onReceiveProductsResponce(MRGSBankProductsResponce productsResponce)
        {   
            if (productsResponce.RequestError != null)
            {
                await Task.Delay(2000);
                DownloadProductsAsync();
                return;
            }

            MRGSLog.getInstance().addLog($"onReceiveProductsResponce at Thread: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            MRGSLog.getInstance().addLog($"onReceiveProductsResponce: {productsResponce}");
            _bankProducts = productsResponce.ValidProducts;
            IsProductsLoaded = true;
            
            foreach (var invalidProductId in productsResponce.ValidProducts)
            {
                Debug.Log("ValidProduct: " + invalidProductId);
            }
            foreach (var invalidProductId in productsResponce.InvalidProductsIds)
            {
                Debug.LogError("InvalidProduct " + invalidProductId);
            }
        }

        public async void onReceiveProductsError(MRGSBankProductsResponce productsResponce)
        {
            if (productsResponce.RequestError != null)
            {
                await Task.Delay(2000);
                DownloadProductsAsync();
                return;
            }
            MRGSLog.getInstance().addLog(string.Format("onReceiveProductsError at Thread: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
            MRGSLog.getInstance().addLog("onReceiveProductsError: " + productsResponce);
        }

        public void onReceiveSucessfullPurchase(MRGSBankPurchaseResult purchase)
        {
            MRGSBank.getInstance().notifyMRGSAboutConsume(purchase.Product.Sku, purchase.Transaction.TransactionIdentifier);
            PremiumPurchaseType purchaseType = ProjectContext.Instance.Container.Resolve<PremiumPurchaseDictionary>().GetPremiumPurchaseSetup(purchase.Product.Sku).GetPremiumPurchaseType();
            PremIumPurchaseUtils.ApplyPurchase(purchaseType);
            MRGSLog.getInstance().addLog($"onReceiveSucessfullPurchase at Thread: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            MRGSLog.getInstance().addLog("onReceiveSucessfullPurchase: " + purchase);
            
            if(!restoring)
                MetricsHub.Instance.SendEvent(MetricsEvent.SHOW_ITEM_BUY, new Dictionary<string, object>()
                {
                    {"price_type", PriceType.REAL_MONEY},
                    {"price", purchase.Product.Price},
                    {"reward_id", purchaseType}
                });
        }

        public void onReceiveFailedPurchase(MRGSBankPurchaseResult purchase)
        {
            MRGSLog.getInstance().addLog(string.Format("onReceiveFailedPurchase at Thread: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
            MRGSLog.getInstance().addLog("onReceiveFailedPurchase: " + purchase);
        }

        public void onReceivePendingPurchase(MRGSBankPurchaseResult purchase)
        {
            MRGSLog.getInstance().addLog(string.Format("onReceivePendingPurchase at Thread: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
            MRGSLog.getInstance().addLog("onReceivePendingPurchase: " + purchase);
        }

        public void onReceiveCancelledPurchase(MRGSBankPurchaseResult purchase)
        {
            MRGSLog.getInstance().addLog(string.Format("onReceiveCancelledPurchase at Thread: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
            MRGSLog.getInstance().addLog("onReceiveCancelledPurchase: " + purchase);
        }

        public void onTransactionsRestoreCompleted()
        {
            MRGSLog.getInstance().addLog(string.Format("onTransactionsRestoreCompleted at Thread: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
            MRGSLog.getInstance().addLog("onTransactionsRestoreCompleted");
            ProjectContext.Instance.Container.Resolve<IDelayedCallbackManager>().Invoke(2f, Restored);
        }
        #1#
        
        #endregion


        public void onReceiveProductsResponce(MRGSBankProductsResponse productsResponce) { }

        public void onReceiveProductsError(MRGSBankProductsResponse productsResponce) { }

        public void onReceiveSucessfullPurchase(MRGSBankPurchaseResult purchase) { }

        public void onReceiveFailedPurchase(MRGSBankPurchaseResult purchase) { }

        public void onReceivePendingPurchase(MRGSBankPurchaseResult purchase) { }

        public void onReceiveCancelledPurchase(MRGSBankPurchaseResult purchase) { }

        public void onTransactionsRestoreCompleted() { }*/
    //}
}