using System;
using App.Achievements;
using App.DAO;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using Library;
using Zenject;

namespace App.Crocodile.Handlers
{
    public class CrocodileStatsHandler
    {
        private float CurrentHealthAmount = 0;
        private float _maxHealthAmount = 0;
        private long CurrentAttackAmount = 0;
        private float hpChangedPercent = 100;
        private int _currentManaAmount = 0;
        private bool isPlayer;
        private IDataService _dataService;
        private SignalBus _signalBus;
        private bool _isArmorActive = false;
        private float _currentArmorAmount = 0;
        private float _maxArmorAmount;
        private float _armorChangedPercent = 100;
        public CrocodileStatsHandler(long currentAttackAmount, long currentHealthAmount, int currentManaAmount, long currentArmorAmount, bool isPlayer, SignalBus signalBus, String name = "", IDataService dataService = null)
        {
            _dataService = dataService;
            CurrentAttackAmount = currentAttackAmount;
            CurrentHealthAmount = currentHealthAmount;
            _maxHealthAmount = currentHealthAmount;
            _currentManaAmount = currentManaAmount;
            _currentArmorAmount = currentArmorAmount;
            _signalBus = signalBus;
            this.isPlayer = isPlayer;
            _signalBus.Subscribe<SwitchArmorSignal>(SwitchArmor);
            if(!isPlayer)
                _signalBus.Fire(new NameChangedSignal(){IsPlayer = isPlayer, Name = name});
            _signalBus.Fire(new HealthPointsChangeSignal(){IsPlayer = isPlayer, CurrentHealthPoint = CurrentHealthAmount, HPChangedPercent = hpChangedPercent});
            _signalBus.Fire(new AttackChangedSignal(){IsPlayer = isPlayer, AttackPoints = CurrentAttackAmount});
            _signalBus.Fire(new ManaChangedSignal(){isPlayer = isPlayer, Mana = _currentManaAmount});
        }

        private void SwitchArmor(SwitchArmorSignal signal)
        {
            if (isPlayer == signal.IsPlayer)
            {
                _isArmorActive = signal.IsActive;
                _maxArmorAmount += signal.MaxArmor;
            }
        }

        public bool IsArmorActive() => _isArmorActive;

        public float CurrentArmorProperty
        {
            get { return _currentArmorAmount; }
            set
            {
                _currentArmorAmount = value;
                _armorChangedPercent = (100f * _currentArmorAmount / _maxArmorAmount) / 100f;
                _signalBus.Fire(new ArmorPointsChangedSignal(){IsPlayer = isPlayer, CurrentArmorPoint = _currentArmorAmount, ArmorChangedPercent = _armorChangedPercent});
                if (_currentArmorAmount <= 0)
                {
                    _isArmorActive = false;
                    _signalBus.Fire(new SwitchArmorSignal(){IsPlayer = isPlayer, IsActive = false});
                }
            }
        }
        
        
        
        public float CurrentHealthPointProperty
        {
            get => CurrentHealthAmount;
            set
            {
                CurrentHealthAmount = value;

                if (CurrentHealthAmount < 1)
                    CurrentHealthAmount = 0;
            
                hpChangedPercent = (100f * CurrentHealthAmount / _maxHealthAmount) / 100f;
                _signalBus.Fire(new HealthPointsChangeSignal(){IsPlayer = isPlayer, CurrentHealthPoint = CurrentHealthAmount, HPChangedPercent = hpChangedPercent});
                if (isPlayer)
                    _dataService.SetLong("PlayerHealth", (long)CurrentHealthAmount);
                else
                    _dataService.SetLong("EnemyHealth", (long)CurrentHealthAmount);

                if (CurrentHealthAmount <= 0)
                {
                    _signalBus.Fire(new GameOverSignal()
                    {
                        EnemyHealth = _dataService.GetLong("EnemyHealth"),
                        PlayerHealth = _dataService.GetLong("PlayerHealth")
                    });
                }
            }
        }

        public int CurrentManaPoint
        {
            get => _currentManaAmount;
            set
            {
                _currentManaAmount = value;
                if (_currentManaAmount < 0)
                    _currentManaAmount = 0;
                _signalBus.Fire(new ManaChangedSignal(){isPlayer = isPlayer, Mana = _currentManaAmount});
                if (isPlayer)
                {
                    _dataService.SetLong("PlayerMana", _currentManaAmount);
                    if(_currentManaAmount <= 0 && IsNotTutorial())
                        _signalBus.Fire<StartNewRounsSignal>();
                }
                else
                    _dataService.SetLong("EnemyMana", _currentManaAmount);
            }
        }

        private bool IsNotTutorial()
        {
            return _dataService.GetTutorialCurrentStep() != TutorialStepType.BRUSH_HELP_POINTER &&
                   _dataService.GetTutorialCurrentStep() != TutorialStepType.HEALING_HELP_POINTER;
        }

        public float MaxHealthPointProperty
        {
            get => _maxHealthAmount;
            set => _maxHealthAmount = value;
        }

        public long CurrentAttackPointProperty
        {
            get => CurrentAttackAmount;
            set
            {
                CurrentAttackAmount = value;
                _signalBus.Fire(new AttackChangedSignal(){IsPlayer = isPlayer, AttackPoints = CurrentAttackAmount});
            }
        }
    }
}
