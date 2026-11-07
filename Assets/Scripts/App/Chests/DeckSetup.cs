using System.Collections.Generic;
using System.Linq;
using App.PrototypeBattle;
using App.SetupsScript;
using UnityEngine;

namespace App.Chests
{
    [CreateAssetMenu(fileName = "deck_setup", menuName = "Setup/DeckSetup")]
    public class DeckSetup : ScriptableObject
    {
        [SerializeField] private List<Deck> decks;
        public List<Deck> GetDecks => decks;

        public Deck GetDeck(int winAmount)
        {
            var deck = decks.FirstOrDefault(deck => deck.GetPlayerWinAmount > winAmount && !deck.isBossDeck);
            if (deck == null)
                deck = decks[decks.Count - 1];
            return deck;
        }

        public Deck GetBossDeck(BathType bossType)
        {
            var deck = decks.FirstOrDefault(deck => deck.isBossDeck && deck.BossType == bossType);
            return deck;
        }
        
    }

    [System.Serializable]
    public class CardsLevelPropability
    {
        [Range(1,10)]
        [SerializeField] private int _cardLevel;
        [Range(0f,1f)]
        [SerializeField] private float _propability;

        public int GetCardLevel => _cardLevel;
        public float GetPropability => _propability;
    }

    [System.Serializable]
    public class CardsAmountPropability
    {
        [Range(1, 10)]
        [SerializeField] private int _cardsAmount;
        [Range(0f,1f)]
        [SerializeField] private float _propability;

        public int GetCardsAmount => _cardsAmount;
        public float GetPropability => _propability;
    }

    [System.Serializable]
    public class Deck
    {
        [Header("Is Boss Deck")] 
        [SerializeField] private bool _isBossDeck;
        [Header("Boss type")] 
        [SerializeField] private BathType _bathType;
        [Header("Cards in deck")]
        [SerializeField] private List<Card> _cards;
        [Header("Propabilities of cards amount played in round")]
        [SerializeField] private List<CardsAmountPropability> cardsAmountPropabilities;
        [Header("Propabilities of cards level for this deck")]
        [SerializeField] private List<CardsLevelPropability> cardsLevelPropabilities;
        [Header("Player win amount for this deck")]
        [SerializeField] private int playerWinAmount;
        public List<Card> GetCards => _cards;
        public int GetPlayerWinAmount => playerWinAmount;
        
        
        public int GetCardLevel()
        {
            float sum = cardsLevelPropabilities.Sum(level => level.GetPropability);
            float randomNum = Random.Range(0, sum);
            int level = Count(randomNum, cardsLevelPropabilities);
            return level;
            
            /*int index = 0;
            float minBorder = 0;
            float maxBorder = cardsLevelPropabilities[index].GetPropability;
            foreach (var set in cardsLevelPropabilities)
            {
                if (propability >= minBorder && propability < maxBorder)
                    return set.GetCardLevel;
                index++;
                minBorder = maxBorder;
                maxBorder += cardsAmountPropabilities[index].GetPropability;
            }
            return 1;*/
        }
        
        private int Count(float rand, List<CardsLevelPropability> list)
        {
            float sumBuf = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sumBuf += list[i].GetPropability;
                if (rand < sumBuf)
                    return list[i].GetCardLevel;
            }
            return 1;
        }
        
        public int GetCardsAmount(float propability)
        {
            int index = 0;
            float minBorder = 0;
            float maxBorder = cardsAmountPropabilities[index].GetPropability;
            foreach (var set in cardsAmountPropabilities)
            {
                if (propability >= minBorder && propability < maxBorder)
                    return set.GetCardsAmount;
                index++;
                minBorder = maxBorder;
                maxBorder += cardsAmountPropabilities[index].GetPropability;
            }
            return 1;
        }

        public bool isBossDeck => _isBossDeck;
        public BathType BossType => _bathType;
    }
    
    [System.Serializable]
    public class Card
    {
        [SerializeField] private WeaponType _weaponType;

        public WeaponType GetWeaponType => _weaponType;
    }
}