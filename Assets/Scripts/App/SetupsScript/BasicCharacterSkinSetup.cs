using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace App.SetupsScript
{
    [CreateAssetMenu(fileName = "basic_character_skin_setup", menuName = "Setup/BasicCharacters")]
    public class BasicCharacterSkinSetup : ScriptableObject
    {
        [SerializeField] private List<Skin> characters;

        public Skin GetCharacterByCharacterType(CharacterType character) =>
            characters.Find(characterLocal => characterLocal.CharacterType == character);
    }

    [Serializable]
    public class Skin
    {
        [SerializeField] public CharacterType CharacterType;
        [SerializeField] public SkeletonDataAsset skin;
    }

    public enum CharacterType
    {
        NONE = 0,
        BASIC_1 = 1,
        BASIC_2 = 2,
        BASIC_3 = 3,
        BASIC_4 = 4,
        BASIC_5 = 5,
        BASIC_6 = 6,
        BASIC_7 = 7,
        BASIC_8 = 8,
        BASIC_9 = 9,
        BASIC_10 = 16,
    
        BOSS_1 = 10,
        BOSS_2 = 11,
        BOSS_3 = 12,
        BOSS_4 = 13,
        BOSS_5 = 14,
        BOSS_6 = 15,
    }
}