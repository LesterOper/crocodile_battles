using App.Weapon.WeaponBases;
using UnityEngine;

public class HotFix : MonoBehaviour
{
    [SerializeField] private WaterUsedWeaponBase water;
    // Start is called before the first frame update
    void Start()
    {
        water.StartAction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
