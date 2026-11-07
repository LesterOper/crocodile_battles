using App.SetupsScript;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EnemyCard : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Image backImg;
    private Card cardCurrent;
    private bool cardWillUse;
    

    public bool CardWillUse
    {
        get => cardWillUse;
        set => cardWillUse = value;
    }

    public void Init()
    {
        backImg.gameObject.SetActive(true);
        backImg.DOFade(1, 0.2f);
        cardCurrent = card;
    }
    
    public void Setup(ICardInfo cardInfo)
    {
        cardCurrent.Setup(cardInfo);
        cardWillUse = true;
    }

    public void Show()
    {
        card.gameObject.SetActive(true);
        backImg.gameObject.SetActive(false);
        backImg.DOFade(0, 0.2f);
    }

    public void Hide()
    {
        card.gameObject.SetActive(false);
        backImg.gameObject.SetActive(true);
        backImg.DOFade(1, 0.2f);
        cardWillUse = false;
    }
    public Card GetCard() => card;
}
