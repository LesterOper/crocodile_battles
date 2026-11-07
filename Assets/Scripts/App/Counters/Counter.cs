using System;
using App.Signals;
using Library;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Counters
{
    public class Counter : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private TextMeshProUGUI counter;
        [SerializeField] private CounterType _counterType;


        [Header("Debug")] 
        [SerializeField] private Button addMoney;

        private CounterUtility _utility;
        private SignalBus _signalBus;
            [Inject]
        public void Construct(CounterUtility counterUtility, SignalBus signalBus)
        {
            _utility = counterUtility;
            _signalBus = signalBus;
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<ChangeMoneySignal>(UpdateView);
        }

        private void Start()
        {
            //_utility = new CounterUtility();
            _signalBus.Subscribe<ChangeMoneySignal>(UpdateView);
            UpdateView();        
            if(_counterType != CounterType.BATTLES_WIN && _counterType != CounterType.BATTLES_LOOSE)
                addMoney.onClick.AddListener(() =>
                {
                    _utility.AppendMoney(_counterType, 5);
                });
        }

        private void UpdateView()
        {
            if (_counterType == CounterType.BATTLES_WIN)
                counter.SetText(((long)_utility.GetMoney(CounterType.BATTLES_WIN)+(long)_utility.GetMoney(CounterType.BOSS_BATTLES_WIN)).ToString());
            else
                counter.SetText(((long)_utility.GetMoney(_counterType)).ToString());
        }
    }
}