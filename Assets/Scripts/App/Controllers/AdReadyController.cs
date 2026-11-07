using DG.Tweening;
using Library.Common;
using UnityEngine;
using UnityEngine.UI;

namespace App.Controllers
{
    public class AdReadyController : MonoBehaviour
    {
        [SerializeField]private Image load;
        [SerializeField] private Button adsButton;
        [SerializeField] private Collider2D coll;
        [SerializeField] private GameObject adReady;
        [SerializeField] private bool isNeedAdReadyIcon;
        private Tween _tween;

        private AdsHelper _adsHelper;
        private void Awake()
        {
            _adsHelper = FindObjectOfType<AdsHelper>();
        }

        private void OnEnable()
        {
            OnRewardStateChanged(_adsHelper.HasVideo());
            _adsHelper.OnVideoStateChanged += OnRewardStateChanged;
        }

        private void OnDisable()
        {
            _adsHelper.OnVideoStateChanged -= OnRewardStateChanged;
        }

        private void OnRewardStateChanged(bool availalable)
        {
            load.gameObject.SetActive(!availalable);
            if(adsButton != null)
                adsButton.interactable = availalable;
            if (coll != null)
                coll.enabled = availalable;
            
            if(isNeedAdReadyIcon)
                adReady.SetActive(availalable);
        }
    }
}