using App.DAO;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using App.Weapon.WeaponBases;
using CodeBase.Infrastructure.Services;
using Library;
using UnityEngine;
using Zenject;

namespace App.Dialogs.BathCollection
{
    public class BathPreview : MonoBehaviour
    {
        private BathSetup _bathSetup;
        [SerializeField] private Card card;
        [SerializeField] private Transform cardParent;
        private Card createdCard;
        private GameObject character;
        private IDataService _service;
        private BathFastenersHandler _handler;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService, BathSetup bathSetup)
        {
            _signalBus = signalBus;
            _bathSetup = bathSetup;
            _service = dataService;
        }
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<RecreateCharacterOnMainSceneSignal>(RecreateBath);
        }
        
        private void Start()
        {
            CreateCharacterOnApplicationStarted();
            CreateBathCard();
            _signalBus.Subscribe<RecreateCharacterOnMainSceneSignal>(RecreateBath);
        }

        private void CreateBath()
        {
            character = Instantiate(_bathSetup.GetBathByType(_service.GetBathData().CrocodileBathType).BathObject, transform);
            character.transform.localScale = Vector3.one*80;
            character.transform.localPosition = character.transform.localPosition - new Vector3(0, 85, 0);
            Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            foreach (var componentsInChild in character.GetComponentsInChildren<SpriteRenderer>())
            {
                componentsInChild.sortingLayerName = "Near Foreground";
            }
            character.GetComponentInChildren<MeshRenderer>().sortingLayerName = "Near Foreground";
        }
        
        private void RecreateBath()
        {
            if (_service.GetTutorialCurrentStep() ==
                TutorialStepType.FIRST_BATH_POINTER)
            {
                _signalBus.Fire<CompleteTutorialSignal>();
                _signalBus.Fire<TutorialSendTargetSignal>();
            }
            Destroy(transform.GetChild(0).gameObject);
            if (createdCard != null)
            {
                Destroy(createdCard.gameObject);
            }
            CreateBath();
            CreateBathCard();
        }

        private void CreateCharacterOnApplicationStarted()
        {
            CreateBath();
        }

        private void CreateBathCard()
        {
            //createdCard = Instantiate(card, cardParent);
            card.Setup(_service.GetBathData());
            card.SetDrag(false);
        }
    }
}