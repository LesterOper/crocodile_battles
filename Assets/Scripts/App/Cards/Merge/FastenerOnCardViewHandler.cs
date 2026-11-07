using App.Dialogs.Merge;
using App.PrototypeBattle;
using Library.Utils;
using UnityEngine;
using UnityEngine.UI;

public class FastenerOnCardViewHandler : MonoBehaviour
{
    [SerializeField] private Image fastener;
    [SerializeField] private Sprite attack;
    [SerializeField] private Sprite defence;
    [SerializeField] private Sprite closeAttack;
    [SerializeField] private Sprite heal;
    [SerializeField] private Sprite defenceClose;

    public void Setup(WeaponType type)
    {
        if (type == WeaponType.UMBRELLA)
        {
            SetFastener(defence);
            return;
        }

        if (type == WeaponType.SNAKE_TUBE)
        {
            SetFastener(defenceClose);
            return;
        }

        FilterType filterType = type.GetAttribute<WeaponData>().FilterType;
        switch (filterType)
        {
            case FilterType.LONG_DISTANCE : fastener.sprite = attack; break;
            
            case FilterType.CLOSE_DISTANCE : fastener.sprite = closeAttack; break;
            case FilterType.ONE_ACTION:
            {
                if (type == WeaponType.MINA_BOMB || type == WeaponType.BOMB_ILLNESS)
                {
                    fastener.sprite = attack;
                    break;
                }

                fastener.sprite = heal;
                break;
            }
        }
    }

    private void SetFastener(Sprite icon)
    {
        fastener.sprite = icon;
    }

    public void SetBathFastener() => fastener.sprite = heal;
}