using System.Collections.Generic;
using App.Achievements;
using App.Analytic;
using App.Cards;
using App.Counters;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.Merge
{
    public class Merge : MonoBehaviour
    {
        [SerializeField] private DotsAnim dotsLeft;
        [SerializeField] private DotsAnim dotsRight;
        [SerializeField] private DotsAnim dotsMiddle;
        [SerializeField] private MergePlace leftPlace;
        [SerializeField] private MergePlace rightPlace;
        [SerializeField] private MergeTotalView _mergeTotalView;
        [SerializeField] private Material mergeStarted;
        [SerializeField] private Button merge;
        [SerializeField] private Image mergeImage;
        [SerializeField] private Image mergeButtonImage;
        [SerializeField] private Sprite activeImageSprite;
        [SerializeField] private Sprite activeImageButtonSprite;
        [SerializeField] private CanvasGroup mergeButtonCanvasGroup;
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private Sprite nonActiveImageSprite;
        [SerializeField] private Sprite nonActiveImageButtonSprite;
        private WeaponView _view;
        private ICardInfo _cardInfo;
        private BathView _bathView;
        private ICardInfo _bathData;
        private bool mergeBath;
        private int positionLeftCard;
        private int positionRightCard;
        private IDataService _dataService;
        private CounterUtility _utility;
        private MergeUtils _utils;
        private long costValue;
        private Vector2 scale;
        private Tween scalling;
        private IAnalytics _analytics;
        private SignalBus _signalBus;
        private MergeCostSetup _mergeCostSetup;
        [Inject]
        public void Construct(SignalBus signalBus, IAnalytics analytics, IDataService dataService, 
            CounterUtility counterUtility, MergeCostSetup mergeCostSetup)
        {
            _signalBus = signalBus;
            _analytics = analytics;
            _dataService = dataService;
            _utility = counterUtility;
            _mergeCostSetup = mergeCostSetup;
        }
        private void Start()
        {
            _utils = new MergeUtils(_dataService);
            scale = merge.transform.localScale;
            merge.onClick.AddListener(MergeProcess);
            scale = transform.localScale;
        }

        public void SetupMerge(ICardInfo bathData)
        {
            mergeBath = bathData.IsBathCard();
            _cardInfo = bathData;

            /*if (cardInfo.IsBathCard())
            {
                _cardInfo = new BathInCollectionData()
                {
                    CrocodileBathType = (BathType) cardInfo.GetCardType(), Level = cardInfo.GetLevel(),
                    ID = _cardInfo.GetId(), InUseForBattle = _cardInfo.IsInUseForBattle()
                };
            }
            else
            {
                _cardInfo = cardInfo;
            }*/
            
            
        }
    
        public void CheckMerge()
        {
            if(!leftPlace.IsFree() || !rightPlace.IsFree())
                _signalBus.Fire(new UnlockFilterButtonsSignal(){Unlock = false});
            if (!leftPlace.IsFree() && !rightPlace.IsFree())
            {
                DotsFirstlyAnim();
            }
        }

        private void SetupMergeButton()
        {
            merge.gameObject.SetActive(true);
            merge.transform.localScale = Vector3.one;
            scalling.Kill();
            costValue = _mergeCostSetup.GetMergeCostByLevel(_cardInfo.GetLevel() + 1);
            if (_utility.GetMoney(CounterType.DEFAULT_MONEY) >= costValue)
            {
                mergeButtonImage.sprite = activeImageButtonSprite;
                scalling = merge.transform.DOScale(merge.transform.localScale * 1.1f, 0.2f).SetLoops(-1, LoopType.Yoyo);
                merge.interactable = true;
            }
            else
            {
                mergeButtonImage.sprite = nonActiveImageButtonSprite;
                merge.interactable = false;
            }
            mergeButtonCanvasGroup.DOFade(1, 0.2f);
            cost.text = costValue.ToString();
            mergeImage.sprite = activeImageSprite;
        }
    
        private void DotsFirstlyAnim()
        {
            dotsLeft.ActivateCoroutine(SetupMergeButton);
            dotsRight.ActivateCoroutine(null);
        }
    
        private void MergeProcess()
        {
            if(_mergeTotalView.transform.childCount > 2)
                _mergeTotalView.CompleteMerge();
            _mergeTotalView.SetMaterial(mergeStarted);
            mergeButtonCanvasGroup.DOFade(0, 0.2f).OnComplete(() =>
            {
                scalling.Kill();
                merge.transform.localScale = scale;
                merge.gameObject.SetActive(false);
            });
            
            _utility.AppendMoney(CounterType.DEFAULT_MONEY, -costValue);
            dotsMiddle.ActivateCoroutine(() =>
            {
                ICardInfo leftCardInfo = leftPlace.GetCardInfo();
                ICardInfo rightCardInfo = rightPlace.GetCardInfo();
                leftPlace.ClearMergePlace();
                rightPlace.ClearMergePlace();

                if (!mergeBath)
                {
                    if (leftCardInfo.IsInUseForBattle())
                    {
                        _dataService.GetBattleCardInfoData().CardInfos.Remove((BattleCardInfo)leftCardInfo);
                    }
                    else
                    {
                        _dataService.GetCardCollectionData().Collection.Remove((CardInfo)leftCardInfo);
                    }

                    if (rightCardInfo.IsInUseForBattle())
                    {
                        _dataService.GetBattleCardInfoData().CardInfos.Remove((BattleCardInfo)rightCardInfo);
                    }
                    else
                    {
                        _dataService.GetCardCollectionData().Collection.Remove((CardInfo) rightCardInfo);
                    }
                    _dataService.UpdateBattleCardInfoData();
                    _dataService.UpdateCardCollectionData();
                }
                else
                {
                    if (leftCardInfo.IsInUseForBattle())
                    {
                        BathData bathData = new BathData()
                        {
                            Level = leftCardInfo.GetLevel() + 1,
                            CrocodileBathType = (BathType) leftCardInfo.GetCardType(), InUseForBattle = true
                        };
                        _dataService.GetBathData().SetData(bathData);
                        /*_dataService.GetBathData().Level++;
                        _dataService.GetBathData().CrocodileBathType = (BathType)leftCardInfo.GetCardType();*/
                        _dataService.UpdateBathData();
                        _dataService.UpdateBathCollection();
                        _signalBus.Fire<RecreateCharacterOnMainSceneSignal>();
                    }
                    else
                    {
                        var removedCard = _dataService.GetBathCollection().BathDatas
                            .Find(bath => bath.ID == leftCardInfo.GetId());
                        _dataService.GetBathCollection().BathDatas.Remove( removedCard /*new BathInCollectionData(leftCardInfo)*/);
                        _dataService.UpdateBathData();
                        _dataService.UpdateBathCollection();
                    }

                    if (rightCardInfo.IsInUseForBattle())
                    {
                        BathData bathData = new BathData()
                        {
                            Level = rightCardInfo.GetLevel() + 1,
                            CrocodileBathType = (BathType) leftCardInfo.GetCardType(), InUseForBattle = true
                        };
                        _dataService.GetBathData().SetData(bathData);
                        _dataService.UpdateBathData();
                        _dataService.UpdateBathCollection();
                        _signalBus.Fire<RecreateCharacterOnMainSceneSignal>();
                    }
                    else
                    {
                        var removedCard = _dataService.GetBathCollection().BathDatas
                            .Find(bath => bath.ID == rightCardInfo.GetId());
                        _dataService.GetBathCollection().BathDatas.Remove(removedCard /*new BathInCollectionData(rightCardInfo)*/);
                        _dataService.UpdateBathData();
                        _dataService.UpdateBathCollection();
                    }

                    
                }

                _analytics.SendEvent(AmplitudeEvent.equipment_merge, 
                    new Dictionary<string, object>(){
                        {AmplitudePlacement.id.ToString(), _cardInfo.GetCardType()}, 
                        {AmplitudePlacement.lvl.ToString(), _cardInfo.GetLevel()+1}
                    });
                
                _mergeTotalView.SetupMerge(_cardInfo, false);
                _signalBus.Fire<UpdateItemsInListSignal>();
                ResetMerge();
            });
            merge.interactable = false;
            mergeButtonImage.sprite = nonActiveImageButtonSprite;
            mergeImage.sprite = nonActiveImageSprite;
        }

        public void ResetMerge()
        {
            if (leftPlace.IsFree() && rightPlace.IsFree())
                _signalBus.Fire(new UnlockFilterButtonsSignal(){Unlock = true});
            dotsRight.FadeOut();
            dotsLeft.FadeOut();
            dotsMiddle.FadeOut();
            dotsRight.FadeIn();
            dotsLeft.FadeIn();
            dotsMiddle.FadeIn();
            mergeImage.sprite = nonActiveImageSprite;
            mergeButtonCanvasGroup.DOFade(0, 0.2f).OnComplete(() =>
            {
                scalling.Kill();
                merge.transform.localScale = scale;
                merge.gameObject.SetActive(false);
            });
        }
    }

    public class MergeUtils
    {
        private readonly IDataService _dataService;
        public MergeUtils(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void ClearExactFastener(BattleCardInfo cardInfo)
        {
            _dataService.GetBattleCardInfoData().CardInfos.Remove(cardInfo);
            _dataService.UpdateBattleCardInfoData();
        }
    }
}