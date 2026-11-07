using System;
using App.PrototypeBattle;
using App.Signals;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.Merge
{
    public class FilterButtonHandler : MonoBehaviour
    {
        [SerializeField] private Button filterButton;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite active, nonActive;
        [SerializeField] private FilterType _filterType;
        [SerializeField] private Image newCardsIcon;
        private FilterType currentActive;
        private IGameInfoService _gameInfoService;
        private Tween _tween;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus, IGameInfoService gameInfoService)
        {
            _signalBus = signalBus;
            _gameInfoService = gameInfoService;
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<NewCardsSignal>(NewCards);
        }

        private void Start()
        {
            _signalBus.Subscribe<NewCardsSignal>(NewCards);
            NewCards();
        }

        private void NewCards()
        {
            if (currentActive != _filterType)
            {
                if (_filterType == FilterType.BATH)
                {
                    if (_gameInfoService.GetNewBathsCardInfo().Count > 0)
                    {
                        newCardsIcon.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (_gameInfoService.GetNewCardsInfo()
                        .Exists(card => ((WeaponType)card.GetCardType()).GetAttribute<WeaponData>().FilterType == _filterType))
                    {
                        newCardsIcon.gameObject.SetActive(true);
                    }
                    else
                    {
                        newCardsIcon.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void AddListener(Action action) =>
            filterButton.onClick.AddListener(()=>
            {
                action.Invoke();
                ChangeSprite(true);
            });

        public void ChangeSprite(bool isActive) => icon.sprite = isActive ? active : nonActive;

        public void Interactable(bool interact) => filterButton.interactable = interact;

    }


    internal enum TaskFilterType
    {
        NONE= 0,
        DAILY =1,
        ETERNAL =2
    }
}
