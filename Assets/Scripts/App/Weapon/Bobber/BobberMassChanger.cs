using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Weapon.Bobber
{
    public class BobberMassChanger : MonoBehaviour
    {
        [SerializeField] private float massOnMainScene = 0;
        [SerializeField] private float massOnBattleScene = 0;

        void Awake()
        {
            SetBobberMass(SceneManager.GetActiveScene().name);
        }

        private void SetBobberMass(string currentScene)
        {
            switch (currentScene)
            {
                case "Main":
                    GetComponent<Rigidbody2D>().mass = massOnMainScene;
                    break;
                case "BattleScene":
                    GetComponent<Rigidbody2D>().mass = massOnBattleScene;
                    break;
                default:
                    GetComponent<Rigidbody2D>().mass = massOnBattleScene;
                    break;
            }
        }
    }
}
