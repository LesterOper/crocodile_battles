using System;
using App;
using App.Crocodile.Implementations;
using App.DAO;
using App.SetupsScript;
using App.Signals;
using App.Weapon.WeaponBases;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using UnityEngine;
using Zenject;

namespace Effects
{
    public class MovePlayerMainScene : MonoBehaviour
    {
        [Inject] private BathSetup _bathSetup;
        private GameObject character;
        private IDataService _service;
        private BathFastenersHandler _handler;
        private CardCharactaristicsUtils utils;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService)
        {
            _signalBus = signalBus;
            _service = dataService;
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<RecreateCharacterOnMainSceneSignal>(RecreateBath);
        }

        private void Start()
        {
            _signalBus.Subscribe<RecreateCharacterOnMainSceneSignal>(RecreateBath);
            utils = new CardCharactaristicsUtils();
            CreateCharacterOnApplicationStarted();
        }

        private void CreateBath()
        {
            character = Instantiate(_bathSetup.GetBathByType(_service.GetBathData().CrocodileBathType).BathObject, transform);
            character.GetComponent<DamageHandler>().enabled = false;
            Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
            rb.mass = 4;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            foreach (var componentsInChild in character.GetComponentsInChildren<SpriteRenderer>())
            {
                componentsInChild.sortingLayerName = "Default";
            }

            character.GetComponentInChildren<MeshRenderer>().sortingLayerName = "Default";
            _signalBus.Fire(new HealthPointsChangeSignal()
            {
                IsPlayer = true, 
                CurrentHealthPoint = (float)utils.GetBathHPValueByLevel(_service.GetBathData().Level, _service.GetBathData().CrocodileBathType),
                HPChangedPercent = 100f
            });
        }
        
        private void HideCroco(Action callback)
        {
            transform.DOMoveX(-25f, 1.4f).OnComplete(callback.Invoke);
        }

        private void RecreateBath() =>
            HideCroco(() =>
            {
                Destroy(transform.GetChild(0).gameObject);
                CreateBath(); 
                Move();
            });

        private void CreateCharacterOnApplicationStarted()
        {
            _signalBus.Fire<OpenCurtainSignal>();
            CreateBath();
            Move();
        }

        public void Move(float delay = 0)
        {
            _signalBus.Fire(new MoveEffectSignal(){ShowEffect = false});
            transform.DOLocalMoveX(-12f, 2f).OnComplete(() =>
            {
                _signalBus.Fire(new MoveEffectSignal(){ShowEffect = true});
            }).SetDelay(delay);
        }
    }
}