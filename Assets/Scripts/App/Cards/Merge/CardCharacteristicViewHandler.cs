using App.PrototypeBattle;
using App.SetupsScript;
using App.Utils;
using UnityEngine;

public class CardCharacteristicViewHandler : MonoBehaviour
{
    [SerializeField] private Characteristic attack;
    [SerializeField] private Characteristic health;
    [SerializeField] private Characteristic mana;
    [SerializeField] private Characteristic lvl;
    [SerializeField] private Characteristic workDuration;
    [SerializeField] private Characteristic armor;
    private CardCharactaristicsUtils _utils;
    private ICardInfo _cardInfo;

    public void Setup(ICardInfo cardInfo, int workDurationValue = 0, int manaValue = 0)
    {
        _cardInfo = cardInfo;
        _utils = new CardCharactaristicsUtils();
        if(_cardInfo.IsBathCard()) SetupBath();
        else SetupWeapon(workDurationValue, manaValue);
    }

    private void SetupBath()
    {
        workDuration?.gameObject.SetActive(false);
        mana.gameObject.SetActive(false);
        attack.gameObject.SetActive(false);
        health.gameObject.SetActive(true);
        health.SetupCharacteristic(_utils.GetBathHPValueByLevel(_cardInfo.GetLevel(), (BathType)_cardInfo.GetCardType(), true).ToString());
        lvl?.gameObject.SetActive(true);
        lvl?.SetupCharacteristic(_cardInfo.GetLevel().ToString());
    }

    private void SetupWeapon(int workDurationValue, int manaValue)
    {
        workDuration?.SetupCharacteristic(workDurationValue.ToString());
        lvl?.gameObject.SetActive(true);
        lvl?.SetupCharacteristic(_cardInfo.GetLevel().ToString());
        mana.gameObject.SetActive(true);
        mana.SetupCharacteristic(manaValue.ToString());

        long attackValue = _utils.GetAttackValueByLevel(_cardInfo.GetLevel(), (WeaponType) _cardInfo.GetCardType());
        if(attackValue <= 0) attack.gameObject.SetActive(false);
        else
        {
            attack.SetupCharacteristic(attackValue.ToString());
            attack.gameObject.SetActive(true);
        }
        
        long healthValue = _utils.GetHealthPointValueByLevel(_cardInfo.GetLevel(), (WeaponType) _cardInfo.GetCardType());
        if(healthValue <= 0) health.gameObject.SetActive(false);
        else
        {
            health.SetupCharacteristic(healthValue.ToString());
            health.gameObject.SetActive(true);
        }

        long armorValue = _utils.GetArmorValueByLevel(_cardInfo.GetLevel(), (WeaponType) _cardInfo.GetCardType());
        if (armorValue <= 0) armor.gameObject.SetActive(false);
        else
        {
            armor.SetupCharacteristic(armorValue.ToString());
            armor.gameObject.SetActive(true);
        }
            
    }

    public void SetupManaCost(bool changeColor) => mana.SetColor(changeColor);
}