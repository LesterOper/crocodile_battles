using App;
using App.Weapon.WeaponBases;
using Library.Collections;
using UnityEngine;

namespace Effects
{
    public class EffectPool : Singleton<EffectPool>
    {
        [SerializeField] private GameObject effectObjectWithCroco = null;
        [SerializeField] private GameObject effectObjectWithWater = null;
        [SerializeField] private GameObject effectSolidProjectileWithCroco = null;
        [SerializeField] private GameObject effectSolidProjectileWithWater = null;
        [SerializeField] private Projectile bottleProjectile = null;
        private GameObjectPool _objectPoolEffectWithCroco;
        private GameObjectPool _objectPoolEffectWithWater;
        private GameObjectPool _objectPoolEffectSolidWithCroco;
        private GameObjectPool _objectPoolEffectSolidWithWater;
        private GameObjectPool _objectPoolBottle;

        public GameObjectPool GetEffectWithCrocoPool() => _objectPoolEffectWithCroco;
        public GameObjectPool GetEffectWithWaterPool() => _objectPoolEffectWithWater;
        public GameObjectPool GetEffectSolidWithCroco() => _objectPoolEffectSolidWithCroco;
        public GameObjectPool GetEffectSolidWithWater() => _objectPoolEffectSolidWithWater;
        public GameObjectPool GetBottle() => _objectPoolBottle;
        private void Awake()
        {
            _objectPoolEffectWithCroco = new GameObjectPool(effectObjectWithCroco, gameObject.transform);
            _objectPoolEffectWithWater = new GameObjectPool(effectObjectWithWater, gameObject.transform);
            _objectPoolEffectSolidWithCroco = new GameObjectPool(effectSolidProjectileWithCroco, gameObject.transform);
            _objectPoolEffectSolidWithWater = new GameObjectPool(effectSolidProjectileWithWater, gameObject.transform);
            _objectPoolBottle = new GameObjectPool(bottleProjectile.gameObject, gameObject.transform);
        }
    }
}