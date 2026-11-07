using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Achievements;
using App.Analytic;
using App.Audio;
using App.Cards;
using App.Chests;
using App.Counters;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using Battle;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using I2.Loc;
using Library;
using Library.Common;
using Library.Dialogs;
using Library.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.CoreLibrary;
using Zenject;

namespace App.Dialogs
{
    public class TakeCardsDialog : Dialog
    {
        [SerializeField] private Button take;
        [SerializeField] private Button skipButton;
        [SerializeField] private RectTransform parentForCards;
        private ChestSetup _chestSetup;
        [SerializeField] private Card prefab;
        [SerializeField] private RectTransform parentForOpenChest;
        [SerializeField] private TextMeshProUGUI _trophyCountText;
        [SerializeField] private TextMeshProUGUI _nameChestText;
        [SerializeField] private RectTransform _startCardPoint;
        [SerializeField] private MovementPath.MovementPath _path;
        [SerializeField] private RectTransform[] _finishPositions;
        [SerializeField] private Card _bathCard;
        [SerializeField] private ChestHandler[] _chestsHandlers;
        private ChestHandler _chestHandler;
        private List<Card> _cardObjects = new List<Card>();
        private Card _cardObject;
        private ChestCardsGenerator _cardsGenerator;
        private bool _timeOut = false;
        private Coroutine openedChest;
        private int index = 0;
        private int bathIndex = 0;
        private ChestAnimation _chestAnimation;
        private CounterUtility _utility;
        private bool skip;
        private bool _isTutorial;
        private SoundPlayer _soundPlayer;
        private BathSetup _bathSetup;
        private WeaponSetup _weaponSetup;
        private IAnalytics _analytics;
        private IGameInfoService _gameInfoService;
     

        [Inject]
        public void Construct(SignalBus signalBus, SoundPlayer soundPlayer, WeaponSetup weaponSetup, 
            ChestSetup chestSetup, IAnalytics analytics, IGameInfoService gameInfoService, CounterUtility utility)
        {
            _soundPlayer = soundPlayer;
            _weaponSetup = weaponSetup;
            _chestSetup = chestSetup;
            _analytics = analytics;
            _gameInfoService = gameInfoService;
            _utility = utility;
        }

        protected override void Start()
        {
            base.Start();
           
            _hideDialogButtonArea.onClick.AddListener(AreaClick);
        }

        public void SetChestSetup(ChestSetup chestSetup)
        {
            _chestSetup = chestSetup;
        }
        
        public void Setup(ChestType chestType, bool isTutorial)
        {
            ChestHandler chestHandler = _chestsHandlers.First(c => c.ChestType == chestType);
            _isTutorial = isTutorial;
            _chestHandler = chestHandler;
            _cardsGenerator =
                new ChestCardsGenerator(chestHandler, _chestSetup, _signalBus, _gameInfoService, _dataService, _analytics);
            GameObject chest = Instantiate(_chestSetup.GetChestObject(chestHandler.ChestType), parentForOpenChest);
            _chestAnimation = chest.GetComponent<ChestAnimation>();
            _nameChestText.text = LocalizationManager.GetTranslation(_chestSetup.GetChestCharacteristic(chestHandler.ChestType).chestName);
            _trophyCountText.text = SetTrophiesCountText(_chestSetup.GetChestCharacteristic(chestHandler.ChestType).trophyCount);
            gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            gameObject.transform.DOPunchScale(Vector3.one * 0.15f, 0.25f,1, 0);
            _chestAnimation.OpenCap();
            take.onClick.AddListener(() => { 
                take.interactable = false;
                Hide();
            });
            skipButton.onClick.AddListener(
                () =>
                {
                    skipButton.interactable = false;
                    _hideDialogButtonArea.onClick.RemoveAllListeners();
                    skip = true;
                    skipButton.onClick.RemoveAllListeners();
                });
            if (_dataService.GetTutorialCurrentStep() != TutorialStepType.OPEN_CHEST)
                _cardsGenerator.GenerateCardSet();
            else
                _cardsGenerator.GenerateStarterCardsPack();

            openedChest = StartCoroutine(CreateCards(chestHandler));
            _signalBus.Fire(new ChestOpenedSignal(){ChestType = chestHandler.ChestType});
            _analytics.SendEvent(AmplitudeEvent.chest_collect, 
                new Dictionary<string, object>(){{AmplitudePlacement.type.ToString(), chestHandler.ChestType}});
        }

        public override void Hide()
        {
            TutorialStepType stepType = _dataService.GetTutorialCurrentStep();
            if (stepType == TutorialStepType.OPEN_CHEST)
            {
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_chest_open_complete);
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_chest_complete);
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_weapon_start);
                _signalBus.Fire<CompleteTutorialSignal>();
                _signalBus.Fire<TutorialSendTargetSignal>();
                _utility.ClearExactMoney(CounterType.POINTS);
            }
            else if(stepType == TutorialStepType.MERGE)
                _signalBus.Fire<ShowTutorialSignal>();
            _signalBus.Fire<MergeAvailableSignal>();
            base.Hide();
        }

        private string SetTrophiesCountText(int counts)
        {
            string translate = counts > 4
                ? LocalizationManager.GetTranslation("TROPHIES_MORE__THAN_4")
                : LocalizationManager.GetTranslation("TROPHIES");
            return counts + " " + translate;
        }
        
        private void AreaClick()
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.enabled = false;
            skipButton.GetComponent<CanvasGroup>().DOFade(0, 0.2f)
                .OnComplete(() => { skipButton.gameObject.SetActive(false); });
             Hide();
        }
        
        private void SkipCardsAnimation()
        {
            _timeOut = true;
            StopCoroutine(openedChest);
            int cardCount = _isTutorial ? _chestSetup.GetChestCharacteristic(_chestHandler.ChestType).trophyCount + 1 : _chestSetup.GetChestCharacteristic(_chestHandler.ChestType).trophyCount;
            for (int i = index; i < cardCount; i++)
            {
                if (_cardsGenerator.CetGeneratedWeaponsCards().Count > i)
                {
                    Card card = CreateWeaponCard(_cardsGenerator.CetGeneratedWeaponsCards()[i],_finishPositions[i]);
                    card.CardStateHandlerProperty.ShowCardFront();
                }
                else
                {
                    Card bathCard = CreateBathCard(_cardsGenerator.GetGeneratedBathCards()[bathIndex],_finishPositions[i]);
                    bathCard.CardStateHandlerProperty.ShowCardFront();
                    bathIndex++;
                }
            }
            ShowButtonTake();
        }
        
        private void SetTimeOut()
        {
            _timeOut = true;
        }
        
        private void ShowButtonTake()
        {
            skipButton.enabled = false;
            skipButton.GetComponent<CanvasGroup>().DOFade(0, 0.2f)
                .OnComplete(() => { skipButton.gameObject.SetActive(false); });
            take.gameObject.SetActive(true);
            take.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(ResubscribeCloseArea);
        }
        
        private void ResubscribeCloseArea()
        {
            _hideDialogButtonArea.onClick.RemoveAllListeners();
            _hideDialogButtonArea.onClick.AddListener(Hide);
        }
    
        private Vector3[] GetPath()
        {
            Vector3[] path = new Vector3[_path.PathElements.Length];
            for (int i=0; i < _path.PathElements.Length; i++)
            {
                path[i] = _path.PathElements[i].position;
            }
            return path;
        }

        private Vector3 GetFinishPos()
        {
            Vector3[] path = new Vector3[_path.PathElements.Length];
            for (int i=0; i < _path.PathElements.Length; i++)
            {
                path[i] = _path.PathElements[i].position;
            }
            return path[_path.PathElements.Length-1];
        }
        
        private Card CreateBathCard(BathType type, int level)
        {
            Card card = Instantiate(_bathCard, _startCardPoint);
            card.Setup( new BathInCollectionData{CrocodileBathType = type, Level = level});
            card.GetComponentInChildren<WeaponStater>().enabled = false;
            return card;
        }

        private Card CreateBathCard(BathInCollectionData bathData, Transform parent)
        {
            Card card = Instantiate(_bathCard, parent);
            card.transform.DOMove(parent.position, 0);
            card.transform.DOScale(Vector3.one * 0.8f, 0);
            card.Setup(bathData);
            card.GetComponentInChildren<WeaponStater>().enabled = false;
            return card;
        }

        private Card CreateWeaponCard(WeaponType typeWeapon, int level)
        {
            _cardObject = Instantiate(prefab, _startCardPoint);
            _cardObject.transform.localScale = Vector3.one*0.1f;
            _cardObject.Setup( new CardInfo(){WeaponType = typeWeapon, type = typeWeapon.GetAttribute<WeaponData>().ActionType, Level =  level});
            _cardObject.gameObject.SetActive(true);
            _cardObjects.Add(_cardObject);
            _cardObject.GetComponentInChildren<WeaponStater>().enabled = false;
            return _cardObject;
        }

        private Card CreateWeaponCard(CardInfo cardInfo, Transform parent)
        {
            _cardObject = Instantiate(prefab, parent);
            if (!cardInfo.WeaponType.GetAttribute<WeaponData>().IsNeedFastener)
                _cardObject.transform.DOScale(Vector3.one * 1f, 0f);
            else _cardObject.transform.DOScale(Vector3.one * 0.8f, 0f);
            _cardObject.transform.DOMove(parent.position, 0f);
            _cardObject.Setup(cardInfo);
            _cardObject.gameObject.SetActive(true);
            _cardObjects.Add(_cardObject);
            _cardObject.GetComponentInChildren<WeaponStater>().enabled = false;
            return _cardObject;
        }

        private IEnumerator CreateCards(ChestHandler chestHandler)
        {
            int cardCount = _chestSetup.GetChestCharacteristic(chestHandler.ChestType).trophyCount;
            for (int i = 0; i < cardCount; i++)
            {
                _timeOut = false;
                index++;
                if (_cardsGenerator.CetGeneratedWeaponsCards().Count > i)
                {
                    Card card = CreateWeaponCard(_cardsGenerator.CetGeneratedWeaponsCards()[i].WeaponType ,_cardsGenerator.CetGeneratedWeaponsCards()[i].Level);
                    card.CardStateHandlerProperty.ShowBackside();
                    if (skip)
                    {
                        card.CardStateHandlerProperty.ShowCardFront();
                        FinishCardAnim(i, card, _cardsGenerator.CetGeneratedWeaponsCards()[i].Level).OnComplete(SkipCardsAnimation);
                        StopCoroutine(openedChest);
                        card.Rotate();
                        break;
                    }
                    else
                    {
                        Sequence sequence = AppearAnim(_cardObject.transform);
                        yield return new WaitUntil(() =>
                            (Input.GetMouseButtonDown(0) && !sequence.active) || _timeOut);
                    }
                    card.CardStateHandlerProperty.ShowCardFront();
                    _cardObject.CardStateHandlerProperty.ChangeActivationStatePArticle(false);
                    _cardObject.Rotate();
                    yield return new  WaitForSeconds(0.7f);
                    FinishCardAnim(i, card, _cardsGenerator.CetGeneratedWeaponsCards()[i].Level);
                }
                else if(_cardsGenerator.GetGeneratedBathCards().Count != 0)
                {
                    Card bathCard = CreateBathCard(_cardsGenerator.GetGeneratedBathCards()[bathIndex].CrocodileBathType ,_cardsGenerator.GetGeneratedBathCards()[bathIndex].Level);
                    bathCard.CardStateHandlerProperty.ShowBackside();
                    if (skip)
                    {
                        bathCard.CardStateHandlerProperty.ShowCardFront();
                        FinishCardAnim(i, bathCard, _cardsGenerator.GetGeneratedBathCards()[bathIndex].Level).OnComplete(SkipCardsAnimation);
                        StopCoroutine(openedChest);
                        bathCard.Rotate();
                        bathIndex++;
                        break;
                    }
                    else
                    {
                        Sequence sequence = AppearAnim(bathCard.transform);
                        yield return new WaitUntil(() =>
                            (Input.GetMouseButtonDown(0) && !sequence.active) || _timeOut);
                    }
                    bathCard.CardStateHandlerProperty.ShowCardFront();
                    bathCard.Rotate();
                    yield return new WaitForSeconds(0.7f);
                    FinishCardAnim(i, bathCard, _cardsGenerator.GetGeneratedBathCards()[bathIndex].Level);
                    bathIndex++;
                }
            }
            ShowButtonTake();
        }
        
        private Sequence AppearAnim(Transform card)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(card.DOMove(GetFinishPos(), 0.8f).SetEase(Ease.Linear))
                .Join(card.DOScale(Vector3.one * 2, 0.5f))
                .Join(card.DOPunchRotation(new Vector3(0f,180f,0f),0.8f,0,0));
            card.GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(() =>
            {
                Invoke(nameof(SetTimeOut),0.5f);
            });
            return sequence;
        }

        private Tween FinishCardAnim(int i, Card card, int level = 1)
        {
            card.CardStateHandlerProperty.ChangeActivationStatePArticle(false);
            card.transform.DOScale(Vector3.one * 0.8f, 0.3f);
            Tween tween = card.transform.DOMove(_finishPositions[i].transform.position, 0.3f);
            card.gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
            return tween;
        }
    }
}