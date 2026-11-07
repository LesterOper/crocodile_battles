using System;
using App.Achievements;
using App.Audio;
using App.Signals;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Common;
using Library.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace App.Utils
{
    public class StatsChanger : MonoBehaviour
    {
        [Header("PlayerStats")]
        [SerializeField] private TextMeshProUGUI healthPointsPlayer;
        [SerializeField] private TextMeshProUGUI attackPointsPlayer;
        [SerializeField] private Slider healthBarPlayerSlider;
        [SerializeField] private Slider healthBarPlayerSliderMain;
        [SerializeField] private Slider armorBarPlayerSlider;
        [SerializeField] private Image armorIconPlayer;
        [SerializeField] private Image healthBarBackGroundPlayer;
        [SerializeField] private TextMeshProUGUI armorPointsPlayer;
        [SerializeField] private RectTransform DefencePlayerRect;
        [SerializeField] private TMP_Text namePlayer;
        [SerializeField] private TMP_Text manaPlayer;
        [SerializeField] private GameObject x2DamageLabel;
        [Header("EnemyStats")]
        [SerializeField] private TextMeshProUGUI healthPointsEnemy;
        [SerializeField] private TextMeshProUGUI attackPointsEnemy;
        [SerializeField] private Slider healthBarEnemySlider;
        [SerializeField] private Slider healthBarEnemySliderMain;
        [SerializeField] private Slider armorBarEnemySlider;
        [SerializeField] private Image armorIconEnemy;
        [SerializeField] private Image healthBarBackGroundEnemy;
        [SerializeField] private TextMeshProUGUI armorPointsEnemy;
        [SerializeField] private RectTransform DefenceEnemyRect;
        [SerializeField] private TMP_Text nameEnemy;
        [SerializeField] private TMP_Text manaEnemy;

        [Header("Common")] 
        [SerializeField] private Color healColor;
        [SerializeField] private Color damageColor;
    
        private bool _decreaseHealthPlayer;
        private float _currentHealthPlayer = 1f;
        private float _currentArmorPlayer = 0;
        private int _currentManaPlayer;
        private Vector3 _rectStarPositionPlayer = Vector3.zero;
    
        private bool _decreaseHealthEnemy;
        private float _currentHealthEnemy = 1f;
        private float _currentArmorEnemy = 0;
        private int _currentManaEnemy;
        private Vector3 _rectStarPositionEnemy = Vector3.zero;
        private SoundPlayer _soundPlayer;
        private SignalBus _signalBus;
        private IGameInfoService _gameInfoService;
        private bool isDoubleDamage;

        [Inject]
        public void Construct(SignalBus signalBus, IGameInfoService gameInfoService)
        {
            _signalBus = signalBus;
            _gameInfoService = gameInfoService;
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<SwitchArmorSignal>(SwitchArmor);
            _signalBus.Subscribe<ArmorPointsChangedSignal>(OnArmorChanged);
            _signalBus.Subscribe<HealthPointsChangeSignal>(OnHPChanged);
            _signalBus.Subscribe<AttackChangedSignal>(OnAttackChanged);
            _signalBus.Subscribe<NameChangedSignal>(SetName);
            _signalBus.Subscribe<ManaChangedSignal>(OnManaChanged);
            _signalBus.Subscribe<ActivateBoosterSignal>(ActivateBoosterDoubleDamage);
        }

        private void ActivateBoosterDoubleDamage(ActivateBoosterSignal signal)
        {
            if (signal.BoosterType == BoosterType.DOUBLE_DAMAGE)
            {
                isDoubleDamage = !signal.IsUsed;
                x2DamageLabel.SetActive(!signal.IsUsed);
            }
                
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<SwitchArmorSignal>(SwitchArmor);
            _signalBus.TryUnsubscribe<ArmorPointsChangedSignal>(OnArmorChanged);
            _signalBus.TryUnsubscribe<HealthPointsChangeSignal>(OnHPChanged);
            _signalBus.TryUnsubscribe<AttackChangedSignal>(OnAttackChanged);
            _signalBus.TryUnsubscribe<NameChangedSignal>(SetName);
            _signalBus.TryUnsubscribe<ManaChangedSignal>(OnManaChanged);
            _signalBus.TryUnsubscribe<ActivateBoosterSignal>(ActivateBoosterDoubleDamage);
        }
    
        [Inject]
        public void Construct(SoundPlayer soundPlayer)
        {
            _soundPlayer = soundPlayer;
        }

        private void Awake()
        {
            
            if (_gameInfoService.IsBossFight())
            {
                if (healthPointsEnemy != null)
                {
                    var bossColor = new Color(0.996f, 0.8235f, 0f);
                    if (healthPointsEnemy != null && attackPointsEnemy != null && nameEnemy != null)
                    {
                        healthPointsEnemy.color = bossColor;
                        attackPointsEnemy.color = bossColor;
                        nameEnemy.color = bossColor;
                    }
                }

            }
        }

        private void SetName(NameChangedSignal signal)
        {
            string name = signal.Name;
            if(name != null)
                nameEnemy.text = name;
            _rectStarPositionEnemy = DefenceEnemyRect.localPosition;
            _rectStarPositionPlayer = DefencePlayerRect.localPosition;

        }

        private void OnManaChanged(ManaChangedSignal signal)
        {
            bool player = signal.isPlayer;
            int mana = signal.Mana;

            if (player)
            {
                _currentManaPlayer = mana;
                manaPlayer.text = _currentManaPlayer.ToString();
            }
            else
            {
                _currentManaEnemy = mana;
                manaEnemy.text = _currentManaEnemy.ToString();
            }
        }

        private void OnArmorChanged(ArmorPointsChangedSignal signal)
        {
            bool isPlayer = signal.IsPlayer;
            float armor = signal.CurrentArmorPoint;
            float percent = signal.ArmorChangedPercent;
            if (isPlayer)
            {
                _currentArmorPlayer = armor;
                UpdatePlayerArmorBarView(percent, 1f, MathA.Round(armor).ToString());
            }
            else
            {
                _currentArmorEnemy = armor;
                UpdateEnemyArmorBarView(percent, 1f, MathA.Round(armor).ToString());
            }
        }
        
        private void OnHPChanged(HealthPointsChangeSignal signal)
        {
            bool _Player = signal.IsPlayer;
            float hp = signal.CurrentHealthPoint;
            float percent = signal.HPChangedPercent;
            if (_Player)
            {
                if (MathA.Round(_currentHealthPlayer) > MathA.Round(hp))
                    _decreaseHealthPlayer = true;
                else if (MathA.Round(_currentHealthPlayer) < MathA.Round(hp))
                {
                    if(SceneManager.GetActiveScene().name == "BattleScene")
                        _signalBus.Fire(new BathEffectSignal(){IsPlayer = true, Effect = EffectType.HEAL});
                    _decreaseHealthPlayer = false;
                }
                else
                {
                    _currentHealthPlayer = hp;
                    return;
                }
                _currentHealthPlayer = hp;
                UpdatePlayerHealthBarView(percent, 1f, MathA.Round(hp).ToString());
            }
            else
            {
                if (MathA.Round(_currentHealthEnemy) > MathA.Round(hp))
                    _decreaseHealthEnemy = true;
                else if (MathA.Round(_currentHealthEnemy) < MathA.Round(hp))
                {
                    if(SceneManager.GetActiveScene().name == "BattleScene")
                        _signalBus.Fire(new BathEffectSignal(){IsPlayer = false, Effect = EffectType.HEAL});
                    _decreaseHealthEnemy = false;
                }
                else
                {
                    _currentHealthEnemy = hp;
                    return;
                }
                _currentHealthEnemy = hp;
                UpdateEnemyHealthBarView(percent, 1f, MathA.Round(hp).ToString());
            }
        }
        private void OnAttackChanged(AttackChangedSignal signal)
        {
            if(signal.IsPlayer)
                UpdatePlayerAttackTextView(signal.AttackPoints);
            else UpdateEnemyAttackTextView(signal.AttackPoints);
        }

        private void SwitchArmor(SwitchArmorSignal signal)
        {
            bool isPlayer = signal.IsPlayer;
            bool isActive = signal.IsActive;
            if (isPlayer)
            {
                armorIconPlayer.gameObject.SetActive(isActive);
                healthPointsPlayer.gameObject.SetActive(!isActive);
                armorPointsPlayer.gameObject.SetActive(isActive);
            }
            else
            {
                armorIconEnemy.gameObject.SetActive(isActive);
                healthPointsEnemy.gameObject.SetActive(!isActive);
                armorPointsEnemy.gameObject.SetActive(isActive);
            }
                
        }
        
        private void UpdatePlayerArmorBarView(float percent, float duration, string armor)
        {
            if (armorBarPlayerSlider != null)
            {
                armorBarPlayerSlider.DOValue(percent, duration);
                armorPointsPlayer.text = armor;
            }
        }

        private void UpdateEnemyArmorBarView(float percent, float duration, string armor)
        {
            if (armorBarEnemySlider != null)
            {
                armorBarEnemySlider.DOValue(percent, duration);
                armorPointsEnemy.text = armor;
            }
        }

        private void UpdatePlayerHealthBarView(float percent, float duration, string hp)
        {
            if (healthBarPlayerSlider != null)
            {
                healthBarPlayerSliderMain.value = percent;
                if (_decreaseHealthPlayer)
                {

                    _soundPlayer.PlaySound("SFX_DAMAGE_2", AudioMixerController.MasterMixerGroup.SFX, 0.15f);
                    DefencePlayerRect.DOShakePosition(0.3f, Vector2.right * 10).OnComplete(() =>
                        DefencePlayerRect.DOLocalMove(_rectStarPositionPlayer, 0.3f));
                }

                healthBarBackGroundPlayer.DOColor(_decreaseHealthPlayer ? damageColor : healColor, 0.1f)
                    .OnComplete(() => healthBarBackGroundPlayer.DOColor(Color.white, 0.8f));
                healthBarPlayerSlider.DOValue(percent, duration);
                healthPointsPlayer.text = hp;
            }
        }

        private void UpdateEnemyHealthBarView(float percent, float duration, string hp)
        {
            if (healthBarEnemySlider != null)
            {
                healthBarEnemySliderMain.value = percent;
                if (_decreaseHealthEnemy)
                {
                    _soundPlayer.PlaySound("SFX_DAMAGE_2", AudioMixerController.MasterMixerGroup.SFX, 0.15f);
                    DefenceEnemyRect.DOShakePosition(0.3f, Vector2.right * 10).OnComplete(() =>
                        DefenceEnemyRect.DOLocalMove(_rectStarPositionEnemy, 0.3f));
                }

                healthBarBackGroundEnemy.DOColor(_decreaseHealthEnemy ? damageColor : healColor, 0.1f)
                    .OnComplete(() => healthBarBackGroundEnemy.DOColor(Color.white, 0.8f));
                healthBarEnemySlider.DOValue(percent, duration);
                healthPointsEnemy.text = hp;
            }
        }

        private void UpdatePlayerAttackTextView(long attack)
        {
            if(attackPointsPlayer != null)
                attackPointsPlayer.text = (isDoubleDamage ? attack*2 : attack).ToString();
        }

        private void UpdateEnemyAttackTextView(long attack)
        {
            if(attackPointsEnemy != null)
                attackPointsEnemy.text = attack.ToString();
        }
    }
}
