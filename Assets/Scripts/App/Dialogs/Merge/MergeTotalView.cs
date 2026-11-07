using System.Collections.Generic;
using App.Achievements;
using App.Cards;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.Merge
{
    public class MergeTotalView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Card _cardViewBasic;
        [SerializeField] private MergeDialog mergeDialog;
        [SerializeField] private GameObject bottomText;
        [SerializeField] private GameObject effectNormal;
        [SerializeField] private TextMeshProUGUI middleText;
        [SerializeField] private TextMeshProUGUI middleMergedText;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button panelButton;
        [SerializeField] private GameObject back;
        [SerializeField] private Material spriteDefault;
        [SerializeField] private Merge _merge;
        [SerializeField] private CanvasGroup resultViews;
        private RectTransform rect;
        private WeaponView _view;
        private Card mergedCard;
        private IDataService _service;
        private IGameInfoService _gameInfoService;
        private Image currentMergeResult;
        private bool isBath;
        private bool isAnimFinish;
        private CanvasGroup _canvasGroup;
        private Vector2 _startPos;
        private Vector3 _startScale;
        private SignalBus _signalBus;
        private WeaponSetup _weaponSetup;
        private BathSetup _bathSetup;
        [Inject]
        public void Construct(SignalBus signalBus, WeaponSetup weaponSetup, BathSetup bathSetup, IDataService dataService, IGameInfoService gameInfoService)
        {
            _signalBus = signalBus;
            _weaponSetup = weaponSetup;
            _bathSetup = bathSetup;
            _gameInfoService = gameInfoService;
            _service = dataService;
        }
        private void Start()
        {
            rect = GetComponent<RectTransform>();
            panelButton.onClick.AddListener(EndMerge);
        }

        public void SetCurrentResultView(Image resultView) => currentMergeResult = resultView;
    
        public void SetMaterial(Material material) => currentMergeResult.material = material;

        public void SetupMerge(ICardInfo cardInfo, bool inUse)
        {
            bool isBathMerge = cardInfo.IsBathCard();
            
            if (isBathMerge)
            {
                BathData card = new BathData(cardInfo);
                card.SetLevel(card.GetLevel() + 1);
                _signalBus.Fire(new TakeCardSignal(){CardLevel = card.Level, FromMegre = true, WeaponType = WeaponType.NONE, BathType = card.CrocodileBathType});
                
                if (inUse)
                {
                    _service.GetBathData().SetData(card);
                    _service.UpdateBathData();
                }
                else
                {
                    _service.GetBathCollection().BathDatas.Add(new BathInCollectionData(card));
                    _service.UpdateBathCollection();
                }
                _gameInfoService.SetNewCardsInfo(_signalBus,new List<ICardInfo>(){card});
                SetupView(card);
            }
            else
            {
                CardInfo card = new CardInfo(cardInfo);
                card.SetLevel(card.GetLevel() + 1);
                _signalBus.Fire(new TakeCardSignal(){CardLevel = card.Level, FromMegre = true, WeaponType = card.WeaponType, BathType = BathType.NONE});
                _service.GetCardCollectionData().Collection.Add(card);
                _service.UpdateCardCollectionData();
                _gameInfoService.SetNewCardsInfo(_signalBus,new List<ICardInfo>(){card});
                SetupView(card);
            }
            
           
        }
        
    
        private void SetupView(ICardInfo cardInfo)
        {
            CanvasGroup canvasGroup;
            mergedCard = Instantiate(_cardViewBasic, transform);
            mergedCard.gameObject.SetActive(true);
            canvasGroup = mergedCard.GetComponent<CanvasGroup>();
            canvasGroup.DOFade(0, 0);
            mergedCard.GetComponent<RectTransform>()
                .DOAnchorPos(new Vector2(0, -35), 0);
            mergedCard.gameObject.GetComponent<DragMerge>().enabled = false;
            mergedCard.Setup( cardInfo);
            cardInfo = null;
            Animation(canvasGroup);
        }

        private void Animation(CanvasGroup canvasGroup)
        {
            _signalBus.Fire<NewCardsSignal>();
            _canvasGroup = canvasGroup;
            isAnimFinish = false;
            canvasGroup.interactable = false;
            panel.SetActive(true); 
            _startPos = rect.anchoredPosition;
            _startScale = rect.localScale;
            rect.DORotate(new Vector3(0, 360, 720), 0.6f, RotateMode.FastBeyond360);
            rect.DOScale(2, 0.6f);
            back.transform.DOScale(transform.localScale * 1f, 0.15f);
            rect.DOAnchorPos(new Vector2(-4.6f,8.9f), 0.6f).OnComplete(() =>
            {
                _signalBus.Fire(new AddItemInListSignal(){IsAdd = true});
                isAnimFinish = true;
                FinishAnim(_canvasGroup);
                _canvasGroup.DOFade(1, 0.2f);
                bottomText.SetActive(false);
            });
        }

        private void FinishAnim( CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = true;
        }

        public void OnPointerClick(PointerEventData eventData) => EndMerge();

        private void EndMerge()
        {
            if(mergedCard == null) return;
            if (!isAnimFinish) return;
            mergedCard.transform.SetParent(transform.parent);
            
            middleText.DOFade(0, 0.2f);
            middleMergedText.DOFade(1, 0.2f);
            rect.DOScale(_startScale, 0.2f).SetDelay(0f).OnComplete(() =>
            {
                panel.SetActive(false);
            });
            effectNormal.SetActive(false);
            resultViews.DOFade(0, 0.1f).OnComplete(() =>
            {
                rect.DOAnchorPos(_startPos, 0.1f).SetDelay(0)
                    .OnComplete(() => resultViews.DOFade(1, 0.1f));
            });
            MergedCardAnimation().OnComplete(CompleteMerge);
        }

        public void CompleteMerge()
        {
            _merge.ResetMerge();
            bottomText.SetActive(true);
            middleText.DOFade(1, 0.2f);
            middleMergedText.DOFade(0, 0.2f);
            back.transform.DOScale(1, 0.3f);
            SetMaterial(spriteDefault);
            if (mergedCard != null)
            {
                Destroy(mergedCard.gameObject);
                mergedCard = null;
            }
        }
    
        private Sequence MergedCardAnimation()
        {
            GameObject cardObject = mergedCard.gameObject;
            //mergedCard.SetDeActiveTexts();
            
            Vector3 targetPos = new Vector3(-7.5f, -0.5f, 0);
            Sequence sequence = DOTween.Sequence();
        
            sequence.Append(cardObject.transform.DOScale(cardObject.transform.localScale * 0.8f, 0.2f));
            sequence.Append(cardObject.transform.DOScale(1.3f, 0.1f));
            sequence.Join(mergeDialog.lastElem.GetComponent<CanvasGroup>().DOFade(0, 0.0f));
            sequence.Join(cardObject.transform.DOMove(targetPos, 0.3f));
            sequence.Join(cardObject.transform.DOScale(Vector2.zero, 0.3f).SetDelay(0.07f));
            sequence.Append(mergeDialog.lastElem.GetComponent<CanvasGroup>().DOFade(1, 0.07f));
            return sequence;
        }
    }
}