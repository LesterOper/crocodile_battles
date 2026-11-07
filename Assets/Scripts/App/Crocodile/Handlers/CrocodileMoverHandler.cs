using System;
using System.Collections;
using App.Audio;
using App.Controllers;
using App.PrototypeBattle;
using App.Signals;
using Castle.DynamicProxy.Generators.Emitters;
using CodeBase.Infrastructure;
using EasyButtons;
using Library;
using Library.Common;
using UnityEngine;
using Zenject;

namespace App.Crocodile.Handlers
{
    public class CrocodileMoverHandler
    {
        private readonly ICoroutineRunner _runner;
        private readonly Transform startPoint;
        private readonly Transform BattlePoint;
        private readonly Transform zeroPoint;
        private readonly CrocodileInfo _crocodileInfo;
        private readonly Rigidbody2D _rigidbody2D;
        private readonly AnimationCurve Curve;
        private readonly AnimationCurve Curve1;
        private AnimationCurve CurveCurrent;
        private CrocodileInfo anotherCroco;
        private Coroutine _moverCoroutine;
        private Action callback;
        private float duration = 1.1f;
        private SignalBus _signalBus;
        private SoundPlayer _soundPlayer;

        public CrocodileMoverHandler(Rigidbody2D rigidbody2D, AnimationCurve curve, AnimationCurve curve1, CrocodileInfo crocodileInfo,
            CrocodileInfo enemyRigid, ICoroutineRunner runner, SignalBus signalBus)
        {
            _rigidbody2D = rigidbody2D;
            Curve = curve;
            Curve1 = curve1;
            _crocodileInfo = crocodileInfo;
            _signalBus = signalBus;
            startPoint = _crocodileInfo.Crocodile.GetStartPoint();
            BattlePoint = _crocodileInfo.Crocodile.GetBattlePoint();
            zeroPoint = _crocodileInfo.Crocodile.GetEndBattlePoint();
            _runner = runner;
            anotherCroco = enemyRigid;
            _soundPlayer = GameObject.FindObjectOfType<SoundPlayer>();
        }

        public void ReWriteEnemy(CrocodileInfo enemyRigid) => anotherCroco = enemyRigid;

        public void StopMoveCoroutine(int impulseForce, float delay)
        {
            _runner.StopCoroutine(_moverCoroutine);
            _rigidbody2D.AddForce(new Vector2(  10,0) *( LayerMask.LayerToName(_rigidbody2D.gameObject.layer) == 
                                                    "Player" ? -impulseForce : impulseForce), ForceMode2D.Force);
        } 

        public void EndBattle(Action callback)
        {
            _runner.StopAllCoroutines();
            _rigidbody2D.mass = 15;
            _signalBus.Fire(new MoveEffectSignal(){ShowEffect = false});
            _crocodileInfo.BathFastenersHandler.CreateEndBattleEffect(callback);
        }

        public void StartCrocodile()
        {
            CurveCurrent = Curve1;
            if(_runner != null)
                _moverCoroutine = _runner.StartCoroutine(DoPhysicalMove(zeroPoint.position, startPoint.position, duration + 0.7f, CurveCurrent, 0));
            callback = () => _signalBus.Fire(new MoveEffectSignal(){ShowEffect = true});
            _crocodileInfo.BathFastenersHandler.SetCrocoAnimation("idle", true,true);
            _signalBus.Fire(new MoveEffectSignal(){ShowEffect = false});
        }
    
        [Button]
        public void GoToStart(Action action)
        {
            callback = () =>
            {
                action?.Invoke();
                _signalBus.Fire(new MoveEffectSignal(){ShowEffect = true});
            };
            if (_rigidbody2D != null)
            {
                CurveCurrent = Curve1;
                if (_crocodileInfo.Crocodile.CanMove())
                {
                    _moverCoroutine = _runner.StartCoroutine(DoPhysicalMove(_rigidbody2D.gameObject.transform.position,
                        startPoint.position,
                        duration, CurveCurrent, 0));
                    _signalBus.Fire(new MoveEffectSignal(){ShowEffect = false});
                }
                else
                {
                    _moverCoroutine = _runner.StartCoroutine(DoPhysicalMove(_rigidbody2D.gameObject.transform.position,
                        startPoint.position, duration / 2, Curve, 0));
                    callback = null;
                }
            }

            _crocodileInfo.BathFastenersHandler.SetCrocoAnimation("idle", true,true);
        }

        public void GoToCroco()
        {
            bool isCrocodileCanMove = _crocodileInfo.Crocodile.CanMove();
            bool isOppositeCanMove = anotherCroco.Crocodile.CanMove();
            if (_rigidbody2D != null)
            {
                CurveCurrent = Curve;
                var fast = _crocodileInfo.BathFastenersHandler.GetFasteners()
                    .Find(local =>
                    {
                        ICardInfo cardInfo = local.Card;
                        if (cardInfo != null)
                            return (WeaponType) cardInfo.GetCardType() == WeaponType.SHOWER;
                        else 
                            return false;
                    });

                callback = () =>
                {
                    if (fast != null)
                        fast.ActivateWeapon(ActivationState.CLOSE_DISTANCE);
                };
                
                if (isCrocodileCanMove)
                {
                    if (isOppositeCanMove)
                    {
                        _moverCoroutine = _runner.StartCoroutine(DoPhysicalMove(_rigidbody2D.gameObject.transform.position,
                            BattlePoint.position, 1f, CurveCurrent, 0));
                    }
                    else  _moverCoroutine = _runner.StartCoroutine(DoPhysicalMove(_rigidbody2D.gameObject.transform.position,
                        anotherCroco.MoverHandler._rigidbody2D.position, 2f, CurveCurrent, 1));
                   
                    _signalBus.Fire(new MoveEffectSignal(){ShowEffect = false});
                    return;
                }

                _signalBus.Fire(new MoveEffectSignal(){ShowEffect = false});
            }
        }

        private IEnumerator DoPhysicalMove(Vector3 startPoint, Vector3 endPoint, float duration, AnimationCurve curve, float delay)
        {
            yield return new WaitForSeconds(delay);
            float timePassed = 0f;
            _soundPlayer.PlaySound("bath_swimm_01", AudioMixerController.MasterMixerGroup.SFX, 0.1f);
            while (timePassed <= duration)
            {
                float newNormalTime = (timePassed + Time.fixedDeltaTime) / duration;
                float newNormalPos = curve.Evaluate(newNormalTime);

                if (_rigidbody2D == null) yield return null;
                Vector3 newPosition = Vector3.Lerp(new Vector3(startPoint.x, _rigidbody2D.transform.position.y), new Vector3(endPoint.x, _rigidbody2D.transform.position.y), newNormalPos);
                Vector3 distancePerUpdate = newPosition - _rigidbody2D.transform.position;
            
                _rigidbody2D.velocity = distancePerUpdate / Time.fixedDeltaTime;
                timePassed += Time.fixedDeltaTime;
            
                yield return new WaitForFixedUpdate();
            }
            callback?.Invoke();
            callback = null;
        }
    }
}