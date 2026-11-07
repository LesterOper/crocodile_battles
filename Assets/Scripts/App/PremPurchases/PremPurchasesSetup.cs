using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.PremPurchases
{
    [CreateAssetMenu(fileName = "prem_purchase_setup", menuName = "Setup/PremPurchaseSetup")]
    public class PremPurchasesSetup : ScriptableObject
    {
        [SerializeField] private PremPurchase[] _premPurchasesData;
        public List<PremPurchase> GetAll()
        {
            return _premPurchasesData.ToList();
        }

        public PremPurchase Get(PremPurchaseType purchaseType)
        {
            return _premPurchasesData.FirstOrDefault(purchase => purchase.Type == purchaseType);
        }

        public PremPurchase Get(string sku)
        {
            return _premPurchasesData.First(item => item.Sku.Equals(sku));
        }

        public List<PremPurchase> Get(IEnumerable<PremPurchaseType> purchases)
        {
            var result = new List<PremPurchase>();
            foreach (var purchase in purchases)
            {
                result.Add(Get(purchase));
            }

            return result;
        }

    }

    [System.Serializable]
    public class PremPurchase
    {
        [SerializeField] private string _sku;
        [SerializeField] private PremPurchaseType _type;
        [SerializeField] private PurchasePriceType _priceType;
        [SerializeField] private long _price;
        [SerializeField] private bool _hasCooldown;
        [SerializeField] private float _cooldown;
        [SerializeField] private bool _giveX2AmountForFirstTime;

        public string Sku => _sku;
        public PremPurchaseType Type => _type;
        public PurchasePriceType PriceType => _priceType;
        public long Price => _price;
        public bool HasCooldown => _hasCooldown;
        public float Cooldown => _cooldown;
        public bool GiveX2ForFirstTime => _giveX2AmountForFirstTime;
    }

    public enum PurchasePriceType
    {
        BaseMoney = 0,
        RealMoney = 1,
        AD = 2,
    }
    
    public enum PremPurchaseType
    {
        NONE =0,
        FREE_MONEY = 1,
        SMALL_MONEY_PACK = 2,
        MEDIUM_MONEY_PACK = 3,
        BIG_MONEY_PACK = 4,
        HUGE_MONEY_PACK = 5,
        FANTASTIC_PACK = 6,
        NO_ADS = 7,
        CROCO_KIT = 8,
        CATOR_KIT = 9,
        CAYMAN_KIT = 10,
        MINI_CHEST = 11,
        MEDIUM_CHEST = 12,
        MEGA_GHEST = 13,
        GIGA_CHEST = 14,
        CARD_1 = 15,
        CARD_2 = 16,
        CARD_3 = 17,
        BATTLE_PASS = 18
            
    
    }
}