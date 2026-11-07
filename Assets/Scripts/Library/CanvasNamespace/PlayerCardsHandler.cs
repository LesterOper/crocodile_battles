using System.Collections.Generic;
using App;
using App.Cards;
using App.DAO;
using App.PrototypeBattle;
using App.Signals;
using App.SetupsScript;
using App.Tutorial;
using App.Weapon.WeaponBases;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Random = System.Random;

namespace Library.CanvasNamespace
{
    public class PlayerCardsHandler : AbstractCardsHandler
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private TextMeshProUGUI currentDeckCardCounter;
        [SerializeField] private TextMeshProUGUI maxCardCount;
        [SerializeField] private RectTransform cardBlocker;
        private WeaponEquipingHandler _weaponEquipingHandler;
        private List<ICardInfo> cards;
        private List<ICardInfo> usedCards = new List<ICardInfo>();
        private int usedCardCount = 0;
        private SignalBus _signalBus;
        //private IDataService _dataService;
        //private IGameInfoService _gameInfoService;
        private List<ICardInfo> _cardsInFold = new List<ICardInfo>();
        [Inject]
        public void Construct(SignalBus signalBus/*, IDataService dataService, IGameInfoService gameInfoService*/)
        {
            _signalBus = signalBus;
            //_dataService = dataService;
            //_gameInfoService = gameInfoService;
        }

        private void Activate()
        {
            base.InitializeCards();
        }
        
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<ManaChangedSignal>(UpdateCardsUsing);
            _signalBus.TryUnsubscribe<RemoveCardFromListSignal>(RemoveCardFromList);
            _signalBus.TryUnsubscribe<FoldCardSignal>(FoldCard);
        }
        
        private void RemoveCardFromList(RemoveCardFromListSignal signal)
        {
            var cardToRemove = signal.Card;        
            Cards.Remove(cardToRemove);
        }

        private void UpdateCardsUsing(ManaChangedSignal signal)
        {
            if(!signal.isPlayer)
                return;
            int playerMana = signal.Mana;
            mana = playerMana;
            foreach (var card in Cards)
            {
                card.CardStateHandlerProperty.UpdateCardUsing(playerMana);
            }
        }
        
        private void Start()
        {
            _signalBus.Subscribe<ManaChangedSignal>(UpdateCardsUsing);
            _signalBus.Subscribe<RemoveCardFromListSignal>(RemoveCardFromList);
            _signalBus.Subscribe<FoldCardSignal>(FoldCard);
            if (SceneManager.GetActiveScene().name == "Main")
            {
                base.Setup(true);
                base.InitializeCards();
            }
           
            if(content != null)
                content.sizeDelta = new Vector2(GetCardsCount() * (1800 / 15), 200);
        }

        private void FoldCard(FoldCardSignal signal)
        {
            _cardsInFold.Add(signal.CardInfo);
        }
        
        
        private int GetCardsCount()
        {
            if (Cards.Count == 0)
                return 4;
            
            return Cards.Count;
        }

        public override void Setup(bool isPlayer)
        {
            base.Setup(isPlayer);
            //InitializeCards();
        }

        protected override void InitializeCards()
        {
            cards = new List<ICardInfo>();
            var cardsBuf = _dataService.GetBattleCardInfoData().CardInfos;
            foreach (var card in cardsBuf)
            {
                cards.Add(new CardInfo() {ID = card.ID, type = card.type, Level = card.Level, WeaponType = card.WeaponType});
            }

            ChooseFromDeck();
            maxCardCount.text = "/" + DeckUtils.CardLimit;
            CustomEventManager.ActivateCardsEvent.AddListener(SetDraggingEnabled);
            Cards.ForEach(local => local.Show(0));
        }

        public override void AddNextCard()
        {
            /*if(cards.Count <= 0)
                UpdateDeckCounters();*/
            //if (cards.Count > 0)
            //{
                List<Card> unusedCard = Cards.FindAll(local => !local.CardStateHandlerProperty.IsUsed());
                for(var i=0; i<4-unusedCard.Count; i++)
                {
                    if (cards.Count > 0)
                    {
                        //if(cards.Count <=0) break;
                        TakeCard();
                    }
                    else
                    {
                        UpdateDeck();
                        TakeCard();
                    }
                    
                }
            //}
        }

        private void TakeCard()
        {
            WeaponType weaponType = WeaponType.NONE;
            ICardInfo local = cards[0];
            weaponType = (WeaponType)local.GetCardType();
            GameObject cardPrefab = CardView;
            GameObject cardObject = Instantiate(cardPrefab, CardParent);
            cardObject.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            cardObject.SetActive(true);
            Card weaponCard = cardObject.GetComponent<Card>();
            WeaponView view = _setup.GetWeaponView(weaponType);
            weaponCard.Setup( local);
            weaponCard.WeaponCardHandlerProperty.SetBattleDrag(view, local, _weaponEquipingHandler);
            Cards.Add(weaponCard);
            cards.Remove(local);
            UpdateDeckCounters();
            usedCards.Add(local);
            weaponCard.Show(0.3f);
        }

        private void UpdateDeck()
        {
            var foldedCards = Shuffle(_cardsInFold); 
            cards.AddRange(foldedCards);
            usedCards.RemoveAll(card => _cardsInFold.Contains(card)); //= Shuffle(usedCards);
            //cards.AddRange(usedCards);
            //usedCards.Clear();
            _cardsInFold.Clear();
        }

        private void UpdateDeckCounters()
        {
            maxCardCount.text = "/" + _dataService.GetBattleCardInfoData().CardInfos.Count.ToString();

            if (cards.Count > 0)
                currentDeckCardCounter.text = (cards.Count).ToString();
            else 
            {
                currentDeckCardCounter.text = cards.Count.ToString();
            }
        }
        
        private void ChooseFromDeck()
        {
            List<ICardInfo> localCards = new List<ICardInfo>();
            var cardsBuf = _dataService.GetBattleCardInfoData().CardInfos;
            foreach (var card in cardsBuf)
            {
                localCards.Add(new CardInfo() {ID = card.ID, type = card.type, Level = card.Level, WeaponType = card.WeaponType});
            }
            if(_dataService.GetTutorialStepComplete(TutorialStepType.BATTLE_TUTORIAL_END))
                localCards = Shuffle(localCards);
            int index = 0;
            foreach (var local in localCards)
            {
                WeaponType weaponType = (WeaponType) local.GetCardType();
                GameObject cardPrefab =CardView;
                GameObject cardObject = Instantiate(cardPrefab, CardParent);
                cardObject.SetActive(true);

                cardObject.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                Card weaponCard = cardObject.GetComponent<Card>();
                WeaponView view = _setup.GetWeaponView(weaponType);
                weaponCard.Setup( local); 
                weaponCard.WeaponCardHandlerProperty.SetBattleDrag(view, local, _weaponEquipingHandler);
                Cards.Add(weaponCard);
                usedCards.Add(local);
                index++;
                if (index > 3)
                {
                    if (cards.Count > 4)
                        currentDeckCardCounter.text = (cards.Count - 4).ToString();
                    else currentDeckCardCounter.text = 0.ToString();
                    break;
                }
            }

            foreach (var card in usedCards)
            {
                localCards.Remove(card);
            }
            
            //localCards.RemoveRange(0, cardsBuf.Count < 4 ? cardsBuf.Count : 4);
            cards = new List<ICardInfo>(localCards);
        }

        private List<ICardInfo> Shuffle(List<ICardInfo> cardsLocal)
        {
            int n = cardsLocal.Count;  
            while (n > 1) {  
                n--;  
                int k = new Random().Next(n + 1);  
                var value = cardsLocal[k];  
                cardsLocal[k] = cardsLocal[n];  
                cardsLocal[n] = value;  
            }

            return cardsLocal;
        }

        public void SetDraggingEnabled(bool state)
        {
            foreach (var cardObject in Cards)
            {
                cardObject.ChangeDragState(state); //SetDraggingEnabled(state);
            }
        }
        private void SetupCardStater()
        {
            //foreach (var Card in Cards) Card.SetupCardStater(_weaponEquipingHandler);
        }

        public override void SetCardUsedGraphic(WeaponType type)
        {
            Card card = Cards.Find(local => (WeaponType)local.GetCardInfo().GetCardType() == type);
            if (card != null)
            {
                card.gameObject.SetActive(false);
                card.CardStateHandlerProperty.SetCardUsed(true);
                CardsWasUsed();
            }
        }

        
        
        public override void SetWeaponEquippingHandler(WeaponEquipingHandler weaponEquipingHandler)
        {
            _weaponEquipingHandler = weaponEquipingHandler;
            SetupCardStater();
            InitializeCards();
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }

        public override void CardsWasUsed()
        {
            usedCardCount++;
            if (usedCardCount >= cards.Count)
                _gameInfoService.DeckIsEmpty = true;
        }
        
    }
}
