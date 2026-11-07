using System.Collections.Generic;
using System.Linq;
using App.PremPurchases;
using UnityEngine;

[CreateAssetMenu(fileName = "prem_purchase_view_setup", menuName = "Setup/PremPurchaseViewSetup")]
public class PremPurchasesInfoSetup : ScriptableObject
{
    [SerializeField] private PremPurchaseInfo[] _premPurchasesData;

    public List<PremPurchaseInfo> GetAll()
    {
        return _premPurchasesData.ToList();
    }

    public PremPurchaseInfo Get(PremPurchaseType purchaseType)
    {
        return _premPurchasesData.FirstOrDefault(purchase => purchase.Type == purchaseType);
    }

    public List<PremPurchaseInfo> Get(IEnumerable<PremPurchaseType> purchases)
    {
        var result = new List<PremPurchaseInfo>();
        foreach (var purchase in purchases)
        {
            result.Add(Get(purchase));
        }

        return result;
    }
}

[System.Serializable]
public class PremPurchaseInfo
{
    [SerializeField] private PremPurchaseType _type;
    [SerializeField] private int amount;
    [SerializeField] private Sprite _smallIcon;

    [SerializeField] private string _nameKey;
    [SerializeField] private string _descriptionKey;
    public int Amount => amount;
    public Sprite SmallIcon => _smallIcon;
    public PremPurchaseType Type => _type;
    public string NameKey => _nameKey;
    public string Descriptions => _descriptionKey;
}

[System.Serializable]
public class PremPurchaseDescriptionItem
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string textKey;
    [SerializeField] private long count;
    
    public Sprite Icon => icon;
    public string TextKey => textKey;
    public long Count => count;
}