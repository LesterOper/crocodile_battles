using App.Audio;
using App.Weapon.WeaponBases;
using Library;
using UnityEngine;

namespace App.Weapon.Catapult
{
    public class Catapult : LongDistanceWeaponBase
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform parent;
        private GameObject catapultBomb;

        #region AnimationEvent

        private void EventOfAnimation()
        {
            //_soundPlayer.PlaySound(shootAudio.name, AudioMixerController.MasterMixerGroup.SFX, 0.8f);
            CardCharactaristicsUtils utils = new CardCharactaristicsUtils();
            catapultBomb = Instantiate(prefab, parent.position, Quaternion.identity);
            catapultBomb.transform.localScale = Vector3.one * 1f;
            catapultBomb.layer = LayerMask.NameToLayer(isPlayer ? "ProjectilePlayer" : "ProjectileEnemy");
            catapultBomb.GetComponent<Projectile>().Setup(new Vector3(isPlayer ? 1f : - 1f, 2.45f), 8.5f , 0, utils.GetAttackValueByLevel(level, _view.Type));
        }

        #endregion
    }
}