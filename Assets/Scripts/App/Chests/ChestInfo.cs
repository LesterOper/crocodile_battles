using System;
using UnityEngine;

namespace App.Chests
{
    [Serializable]
    public class ChestView
    {
        public ChestType Type;
        public GameObject ChestObject;
        public ChestCharacteristic ChestCharacteristic;
    }

    [Serializable]
    public class ChestCharacteristic
    {
        public string chestName;
        public Sprite chestIcon;
        public int trophyCount;
        public int trophyCost;
        public int readyTimeMinutes;
        public int pointsToGet;
    }
}