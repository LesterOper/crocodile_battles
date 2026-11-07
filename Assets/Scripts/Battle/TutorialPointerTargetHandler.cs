using System;
using System.Collections.Generic;
using App;
using App.DAO;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using Library;
using UnityEngine;
using Zenject;

public class TutorialPointerTargetHandler : MonoBehaviour
{
    [SerializeField] private List<TutorialPointerTarget> tutorialTargets;
    private SignalBus _signalBus;
    private IDataService _dataService;

        [Inject]
    public void Construct(SignalBus signalBus, IDataService dataService)
    {
        _signalBus = signalBus;
        _dataService = dataService;
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<TutorialSendTargetSignal>(SendTutorialTarget);
    }

    private void Start()
    {
        _signalBus.Subscribe<TutorialSendTargetSignal>(SendTutorialTarget);
    }

    private void SendTutorialTarget()
    {
        TutorialStepType stepType = _dataService.GetTutorialCurrentStep();

        Transform target = tutorialTargets.Find(local => local.StepType == stepType).Target;
        if (target == null) return;
        _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = target});
    }

    [Serializable]
    class TutorialPointerTarget
    {
        [SerializeField] private TutorialStepType stepType;
        [SerializeField] private Transform target;

        public TutorialStepType StepType => stepType;
        public Transform Target => target;
    }
}