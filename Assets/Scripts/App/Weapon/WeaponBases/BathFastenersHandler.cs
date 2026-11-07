using System;
using System.Collections.Generic;
using System.Linq;
using App.Achievements;
using App.Controllers;
using App.Crocodile.Fasteners;
using App.Crocodile.Interfaces;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using App.View;
using Battle.Equipment;
using DG.Tweening;
using Library.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;
using Zenject;
using Animation = UnityEngine.Animation;
using AnimationState = Spine.AnimationState;
using Random = UnityEngine.Random;

namespace App.Weapon.WeaponBases
{
    public class BathFastenersHandler : MonoBehaviour
    {
        [SerializeField] private List<BathFastener> parentsForWeapons;
        [SerializeField] private List<Fastener> _fasteners;
        [SerializeField] private SkeletonAnimation croco;
        [SerializeField] private SpriteRenderer bathRenderer;
        [SerializeField] private Rigidbody2D bathRigidbody;
        [SerializeField] private WeaponSetup _weaponSetup; 
        [SerializeField] private GameObject effectEndPrefab;
        [SerializeField] private Transform lineRendererEndPos;
        [SerializeField] private Transform waves;
        [SerializeField] private Transform parentForBelt;
        [SerializeField] private Animation _animation;
        [SerializeField] private Color color;
        [SerializeField] private GameObject gluePrefab;
        private CrocoItemsDOFader _fader;
        private ICrocodileController crocodileController;
        private Transform OneActionBombTarget;
        private bool isShakePlaying;
        private bool isAnimationCrocoPlay;
        private IEquipment oneActionWeapons;
        private Card cardsOneAction;
        private CardCharactaristicsUtils utils;
        [Inject] private SignalBus _signalBus;
        [Inject] private WeaponSetup _setup;
        public WeaponSetup Setup => _weaponSetup;
        public SkeletonAnimation GetSkeletonAnimations() => croco;
        private Action callbackEndBattle;
        [Serializable]
        class BathFastener
        {
            [SerializeField] private WeaponType _type;
            [SerializeField] private Transform parent;

            public Transform GetParent() => parent;
            public WeaponType TYPE => _type;
        }

        public void AddOneActionEquipment(IEquipment equipment, Card cardObject)
        {
            if (oneActionWeapons != null)
            {
                var att = utils.GetAttackValueByLevel(cardsOneAction.GetCardInfo().GetLevel(), (WeaponType)cardsOneAction.GetCardInfo().GetCardType());
                crocodileController.GetCrocodileInfo().StatsHandler.CurrentAttackPointProperty -= att;
                Destroy(oneActionWeapons.GetObject());
                ResetOneActionWeapons();
                oneActionWeapons = null;
                cardsOneAction = null;
            }
            oneActionWeapons = equipment;
            cardsOneAction = cardObject;
        }

        public void ResetOneActionWeapons()
        {
            if (oneActionWeapons != null)
            {
                Destroy(oneActionWeapons.GetObject());
                oneActionWeapons = null;
                /*cardsOneAction.WeaponEquipped = false;
                cardsOneAction.ChangeViewState();*/
                cardsOneAction.transform.SetSiblingIndex(0);
               // CustomEventManager.InvokeUseCardEvent(false, cardsOneAction);
               crocodileController.GetCrocodileInfo().StatsHandler.CurrentManaPoint +=
                   _setup.GetWeaponView((WeaponType) cardsOneAction.GetCardInfo().GetCardType()).Mana;
                    //utils.GetManaCostByLevel(cardsOneAction.GetCardInfo().GetLevel(), (WeaponType)cardsOneAction.GetCardInfo().GetCardType());

                cardsOneAction = null;
            }
        }
        
        public void ActivateOneActionWeapons()
        {
            if (oneActionWeapons == null) return;
            oneActionWeapons.StartAction();
            oneActionWeapons = null;
        }

        private void ResetFasteners()
        {
            _fasteners.Clear();
            parentsForWeapons.Clear();
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<ActivateBoosterSignal>(ThrowFastenerBlocker);
        }

        private void Start()
        {
            utils = new CardCharactaristicsUtils();
            crocodileController = GetComponentInParent<ICrocodileController>();
            _fader = GetComponent<CrocoItemsDOFader>();
            _signalBus.Subscribe<ActivateBoosterSignal>(ThrowFastenerBlocker);
        }

        public void SetForOneActionBomb(Transform OneActionBombTarget) => this.OneActionBombTarget = OneActionBombTarget;


        public void CreateEndBattleEffect(Action callBack)
        {
            _animation.Play();
            Instantiate(effectEndPrefab,
                new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z),
                effectEndPrefab.transform.rotation);
            _fader.DOFadeCroco(0.35f, 1.5f, 0.25f);
            _fasteners.ForEach(f => f.GetCurrentEquipment()?.StopAnimation());
            callbackEndBattle = callBack;
            Invoke(nameof(Invoke), 1f);
        }

        private void Invoke() => callbackEndBattle.Invoke();    
    
        public void RotateWaves()
        {
            var localScale =  waves.localScale ;
            localScale.x *= -1;
            waves.localScale  = localScale;
        }

        public bool IsWeaponsFinishedWorkByType(ActivationState state)
        {
            return _fasteners.FindAll(obj => obj.GetCurrentEquipment()?.GetActivationState() == state)
                ?.Find(obj => !obj.GetCurrentEquipment().IsWeaponFinishedWork());
        }
    
        public void VictoryAnim() => croco.state.SetAnimation(0, "win_anim", true);
    
        public void HitEffect(bool needShake = true)
        {
            if (isShakePlaying) return;
            isShakePlaying = true;

            if (needShake)
                transform.GetChild(0).DOShakePosition(0.1f, new Vector3(0.1f, 0.05f, 0), 10, 0)
                    .SetLoops(2)
                    .OnComplete(() => { isShakePlaying = false; });
            else
            {
                crocodileController.GetCrocodileInfo().MoverHandler.StopMoveCoroutine(0, 0);
                isShakePlaying = false;
            }
            DOTween.To(() => croco.Skeleton.R, value => croco.Skeleton.R = value, color.r, 0.1f);
            DOTween.To(() => croco.Skeleton.G, value => croco.Skeleton.G = value, color.g, 0.1f);
            DOTween.To(() => croco.Skeleton.B, value => croco.Skeleton.B = value, color.b, 0.1f);

            bathRenderer.DOColor(color, 0.1f).OnComplete(() =>
            {
                bathRenderer.DOColor(Color.white, 0.1f);
                DOTween.To(() => croco.Skeleton.R, value => croco.Skeleton.R = value, Color.white.r, 0.1f);
                DOTween.To(() => croco.Skeleton.G, value => croco.Skeleton.G = value, Color.white.g, 0.1f);
                DOTween.To(() => croco.Skeleton.B, value => croco.Skeleton.B = value, Color.white.b, 0.1f);
            });

            if (!isAnimationCrocoPlay)
            {
                isAnimationCrocoPlay = true;
                croco.state.SetAnimation(0, "take_damage", false);
                croco.state.Complete += TakeDamageAnimOnComplete();
            }
        }

        private AnimationState.TrackEntryDelegate TakeDamageAnimOnComplete()
        {
            return entry =>
            {
                if (croco.state.GetCurrent(0).Animation.Name == "win_anim") return;
                croco.state.Complete -= TakeDamageAnimOnComplete();
                isAnimationCrocoPlay = false;
                croco.state.SetAnimation(0, "idle", false);
            };
        }


        public void TakeBombAnim()
        {
            croco.state.SetAnimation(0, "take_bomb", false);
            croco.state.AddAnimation(0, "waiting", true,0.05f);
        }

        private AnimationState.TrackEntryDelegate TakeBombAnimOnComplete()
        {
            return entry =>
            {
                croco.state.SetAnimation(0, "waiting", true);
            };
        }


        public void SetCrocoAnimation(string animationName, bool loop , bool randDelay =false)
        {
            if (randDelay)
                croco.state.SetAnimation(0, animationName, loop).Delay = Random.Range(0,1f);
            else
                croco.state.SetAnimation(0, animationName, loop);
            isAnimationCrocoPlay = false;
        }


        private void ThrowFastenerBlocker(ActivateBoosterSignal signal)
        {
            if(signal.BoosterType != BoosterType.BLOCK_FASTENER)
                return;
            if (crocodileController.GetCrocodileInfo().Crocodile.IsPlayer() && !signal.IsUsed)
            {
                croco.state.Event += ThrowGlue;
                croco.state.SetAnimation(0,"throw_bomb",false);
                croco.state.AddAnimation(0, "idle", false,0);
            }
        }

        private void ThrowGlue(TrackEntry entry, Spine.Event e)
        {
            bool match = (e.Data.Name.Equals("throw"));
            if (match)
            {
                GameObject glue = Instantiate(gluePrefab, transform);
                glue.transform.SetParent(null);
                Rigidbody2D rigid = glue.GetComponent<Rigidbody2D>();
                rigid.simulated = true;
                rigid.AddForce(new Vector2( 3.3f, 3.1f) * 40, ForceMode2D.Force);
                croco.state.Event -= ThrowGlue;
            }
        }

        public void ActivateBlockFastenerBooster()
        {
           var fasteners = _fasteners.Where(fastener => fastener.GetActionType() == WeaponActionType.ATTACK_LONG_DISTANCE ||
                                         fastener.GetActionType() == WeaponActionType.ATTACK_CLOSE_DISTANCE).ToList();
           Fastener blockedFastener = fasteners.Random();
           fasteners.Remove(blockedFastener);
           blockedFastener.BlockFastener();
        }
        public Transform GetParentForBelt() => parentForBelt;
        public Transform GetFastenerByType(WeaponType type) => parentsForWeapons.FirstOrDefault(obj => obj.TYPE == type)?.GetParent();
        public List<Fastener> GetFasteners() => _fasteners;
        public Transform GetForOneActionTarget() => OneActionBombTarget;
        public Transform GetLineRendererEndPos() => lineRendererEndPos;
        public Rigidbody2D BathRigidbody => bathRigidbody;
    
        public Transform GetFastenerTransformByWeaponActionType(WeaponActionType weaponActionType)
        {
            return weaponActionType != WeaponActionType.ONE_ACTION_BOMB ? bathRigidbody.transform : GetForOneActionTarget();
        }
    }
}