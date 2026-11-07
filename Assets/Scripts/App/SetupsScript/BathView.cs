using System;
using UnityEngine;

namespace App.SetupsScript
{
    [Serializable]
    public class BathView 
    {
        [SerializeField] private GameObject _bathObject;
        [SerializeField] private Sprite _bathIcon;
        [SerializeField] private BathType _bathType;
   
        public BathType BathType => _bathType;
        public GameObject BathObject => _bathObject;
    
        public Sprite BathIcon => _bathIcon;
        public Sprite GetSprite() => _bathIcon;
    }
}

