using System;
using App.DAO;
using App.Signals;
using CodeBase.Infrastructure.Services;
using Library;
using SimpleSQL;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Tutorial
{
    public class TutorialButton : MonoBehaviour
    {
        [SerializeField] private Extension _extension;
        private Button btn;
        private IDataService _dataService;
        public bool _interactable = true;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService)
        {
            _signalBus = signalBus;
            _dataService = dataService;
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<ButtonInteractableInTutor>(SwitchButtonEnable);
        }

        private void OnDestroy()
        {
            _signalBus.TryUnsubscribe<TutorialButtonSignal>(Activator);
        }

        private void OnDisable()
        {
            //if (gameObject.activeInHierarchy)
            //{
                _signalBus.TryUnsubscribe<ButtonInteractableInTutor>(SwitchButtonEnable);
            //}
        }

        private void SwitchButtonEnable()
        {
            _interactable = !_interactable;
            btn.interactable = _interactable;
        }
        
        private void Awake()
        {
            btn = GetComponent<Button>();
        }

        private void Start()
        {
            _signalBus.Subscribe<TutorialButtonSignal>(Activator);
            Activator();
        }

        private void Activator()
        {
            if(_extension == Extension.NONE)
                return;
            if (_dataService.GetExtensionUnlock(_extension))
            {
                gameObject.SetActive(true);
                if(btn != null) btn.interactable = true;
            }
            else
            {
                gameObject.SetActive(false);
                if(btn != null) btn.interactable = false;
            }
        }
    }

    public enum Extension
    {
        NONE = 0,
        FIGHT = 1,
        BACK_TO_MAIN = 2,
        MERGE = 3,
        TACTICS = 4,
        FILTERS = 5,
        MERGE_TUTOR_END = 6,
        AUTO_BUILD_BUTTON = 7,
        WEAPON_COLLECTION_BUTTON = 8,
        BATH_COLLECTION_BUTTON = 9,
        BATTLE_PASS = 10,
        ACHIEVEMENTS = 11,
        X2_REWARD = 12,
        PREM_SHOP = 13
    }
}