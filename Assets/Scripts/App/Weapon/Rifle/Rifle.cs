using App.Signals;
using App.Weapon.WeaponBases;
using Library;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Weapon.Rifle
{
    public class Rifle : WaterUsedWeaponBase
    {
        [Header("Rifle")] 
        [SerializeField] private LineRenderer _renderer;

        [SerializeField] private Transform startLineRendererPosition;
        private Transform target;

        private bool battle;
        private void Start()
        {
            battle = SceneManager.GetActiveScene().name != "Main";
            if(battle)
                target = _controller.GetEnemy().GetCrocodileInfo().BathFastenersHandler.GetLineRendererEndPos();
            else _renderer.gameObject.SetActive(false);
            _signalBus.Subscribe<ShowEndBattleViewSignal>(HideAim);
        }
        
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<ShowEndBattleViewSignal>(HideAim);
        }

        private void HideAim(ShowEndBattleViewSignal signal)
        {
            battle = false;
        }

        public override void StartAction()
        {
            if (battle)
            {
                base.StartAction();
                _renderer.SetPosition(0, startLineRendererPosition.position);
                _renderer.SetPosition(1, target.position);
            }
        }

        void Update()
        {
            if(!battle) return;
            _renderer.SetPosition(0, startLineRendererPosition.position);
            _renderer.SetPosition(1, target.position);
            Vector3 dir = target.position - transform.position;
            float angle = Mathf.Atan2(dir.y,isPlayer ? dir.x : -dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, isPlayer ? Vector3.forward : -Vector3.forward);    
        }
    }
}
