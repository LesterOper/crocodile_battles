using App.DAO;
using App.PrototypeBattle;
using App.Signals;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.Merge
{
    public class MergePlace : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Merge _merge;
        private Card card;
        private IDataService _data;
        private ICardInfo _info;
        private bool _isFree = true;
        private bool _isBathMerge;
        private float _lastTimeClick;
        private Image _cardPlaceImage;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService)
        {
            _signalBus = signalBus;
            _data = dataService;
        }

        private void Start()
        {
            _cardPlaceImage = GetComponent<Image>();
        }

        public void Setup(Card cardLocal, ICardInfo cardInfo)
        {
            _isFree = false;
            _isBathMerge = cardInfo.IsBathCard();
            _info = cardInfo;
            SetCardProperties(cardLocal);
            SetupMergeWeapons();
            card.transform.localScale = Vector3.one;
            _cardPlaceImage.DOColor(Color.clear, 0.25f);

            _signalBus.Fire(new MergeFilterChangedSignal()
            {
                CardInfo = _info,
                FilterType = _info.IsBathCard() ?
                    FilterType.BATH :
                    GetFilterType(_info)
            });
        }

        private FilterType GetFilterType(ICardInfo cardInfo)
        {
            if ((WeaponType) cardInfo.GetCardType() == WeaponType.MINA_BOMB ||
                (WeaponType) cardInfo.GetCardType() == WeaponType.BOMB_ILLNESS)
                return FilterType.LONG_DISTANCE;
            else
                return ((WeaponType) cardInfo.GetCardType()).GetAttribute<WeaponData>().FilterType;
        }

        private void SetCardProperties(Card card)
        {
            this.card = card;
            this.card.transform.SetParent(transform);
            RectTransform rect = this.card.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            this.card.CardStateHandlerProperty.SetCardPosition(Vector2.zero);
        }

        private void SetupMergeWeapons()
        {
            _merge.SetupMerge(_info);
            _merge.CheckMerge();
        }
    
        public void ClearMergePlace()
        {
            Destroy(card.gameObject);
            _isFree = true;
            _merge.ResetMerge();
            _cardPlaceImage.DOColor(Color.white, 0.25f);
            _info = null;
        }

        public void SetFree()
        {
            _isFree = true;
            card = null;
            _cardPlaceImage.DOColor(Color.white, 0.25f);
        }

        public bool IsFree() => _isFree;

        public ICardInfo GetCardInfo() => _info;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if(IsFree()) return;
            float currentTimeClick = eventData.clickTime;

            if (Mathf.Abs(currentTimeClick - _lastTimeClick) < .75f)
            {
                ClearMergePlace();
                _signalBus.Fire(new MergeFilterChangedSignal(){FilterType = FilterType.ALL});
            }

            _lastTimeClick = currentTimeClick;
        }
    }
}