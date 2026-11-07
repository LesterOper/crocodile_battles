using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace App.SetupsScript
{
    [CreateAssetMenu(fileName = "name_generator", menuName = "Setup/Names")]
    public class NameGenerator : ScriptableObject
    {
        [SerializeField] private String[] firstNames = null;
        [SerializeField] private String[] secondNames = null;

        public String GetRandomName()
        {
            string firstName = firstNames[Random.Range(0, firstNames.Length)];
            string secondName = "";
            if (Random.Range(0, 11) > 5)
            {
                secondName = secondNames[Random.Range(0, firstNames.Length)];
            }
            string number = "";
            if (Random.Range(0, 11) > 5)
            {
                number = Random.Range(1, 100).ToString();
            }

            return $"{firstName}{secondName}{number}";
        }
    }
}
