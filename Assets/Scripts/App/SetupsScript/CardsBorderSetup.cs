using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.SetupsScript
{
    [CreateAssetMenu(fileName = "CardsBorderSetup", menuName = "Setup/CardsBorderSetup")]
    public class CardsBorderSetup : ScriptableObject
    {
        [SerializeField] private List<CardsBorder> bathBorders = new List<CardsBorder>();
        [SerializeField] private List<CardsBorder> basicCardBorders = new List<CardsBorder>();
        
        public Sprite GetBathCardBorderByLevel(int level) =>
            bathBorders.FirstOrDefault(border => border.level == level)?.border;
        public Sprite GetBasicCardBorderByLevel(int level) =>
            basicCardBorders.FirstOrDefault(border => border.level == level)?.border;
    
    }

    [Serializable]
    public class CardsBorder
    {
        public int level;
        public Sprite border;
    }
}