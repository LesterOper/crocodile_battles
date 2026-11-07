using App.PrototypeBattle;
using App.SetupsScript;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image cardIcon;
    [SerializeField] private Image cardIconBack;
    [SerializeField] private GameObject frontSide;
    [SerializeField] private Image backSide;
    [SerializeField] private Image border;
    [SerializeField] private Image lifeDuration;
    [SerializeField] private GameObject newCardIcon;
    [SerializeField] private UIParticle _particles;
    [SerializeField] private CardsBorderSetup cardsBorderSetup;
    [SerializeField] private FastenerOnCardViewHandler fastener;
    
    public void SetCardIcon(Sprite icon) => cardIcon.sprite = icon;
    public void SetBorder(int level, WeaponType weaponType)
    {
        border.sprite = cardsBorderSetup.GetBasicCardBorderByLevel(level);
        border.SetNativeSize();
        lifeDuration?.gameObject.SetActive(true);
        fastener.Setup(weaponType);
    }

    public void SetBathBorder(int level)
    {
        border.sprite = cardsBorderSetup.GetBathCardBorderByLevel(level);
        border.SetNativeSize();
        lifeDuration?.gameObject.SetActive(false);
        fastener.SetBathFastener();
    }

    public void ChangeCardState(bool activate)
    {
        frontSide.gameObject.SetActive(activate);
        backSide.gameObject.SetActive(!activate);
    }

    public void ActivateNewCardIcon(bool activate)
    {
        newCardIcon?.SetActive(activate);
    }

    public void ChangeStateOfParticles(bool activate)
    {
        _particles.gameObject.SetActive(activate);
        if(activate) _particles.Play();
        else _particles.Stop();
    }

    public void EnableIconBack(bool activate) => cardIconBack.gameObject.SetActive(activate);
}