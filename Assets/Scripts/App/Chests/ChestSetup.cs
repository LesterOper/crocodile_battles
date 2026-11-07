using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Chests
{
    [CreateAssetMenu(fileName = "chest_setup", menuName = "Setup/ChestSetup")]
    public class ChestSetup : ScriptableObject
    {
        [SerializeField] private List<ChestView> ChestContainer;
        public GameObject GetChestObject(ChestType type) => Get(type)?.ChestObject;
        public ChestCharacteristic GetChestCharacteristic(ChestType type) => Get(type)?.ChestCharacteristic;
        private ChestView Get(ChestType type) => ChestContainer.FirstOrDefault(view => view.Type == type);
    }
}