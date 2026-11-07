using App.DAO;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace App.Chests
{
    public class DailyChestController : MonoBehaviour
    {
        private float _startPosY;
        private float _availablePosY =4.8f;
        [SerializeField] private AnimationCurve _curve;
        [Inject]private IDataService _dataService;

        void Start()
        {
            _startPosY = transform.position.y;
            if (Available())
            {
                MoveToScene();
            }
            else
            {
                MoveOut();
            }
        }
        
        private void MoveToScene()
        {
            transform.DOMoveY(10, 0);
            transform.DOMoveY(_availablePosY, 2f).SetEase(_curve);
        }

        private void MoveOut()
        {
            if(transform.position.y != _startPosY)
                transform.DOMoveY(_startPosY, 2f);
        }
    
        private bool Available()
        {
            return _dataService.GetTutorialStepComplete(TutorialStepType.MERGE);
        }
    }
}