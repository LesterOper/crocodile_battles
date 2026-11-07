using System;
using App.Analytic;
using App.DAO;
using App.PrototypeBattle;
using App.Signals;
using CodeBase.Infrastructure.Services;
using Library;
using Library.Utils;
using UnityEngine;
using Zenject;

namespace App.Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private Pointer _pointerPrefab;

        [SerializeField] private HelpView _helpViewPrefab;
    
        private TutorialStepType current;
        [Inject]private IDataService _dataService;
        private HelpView view;
        private Pointer _pointer;
        [Inject]private IAnalytics _analytics;
        private Transform _mainCanvas;
        [Inject]private SignalBus _signalBus;

        private void OnEnable()
        {
            _signalBus.Subscribe<ShowTutorialSignal>(Show);
            _signalBus.Subscribe<CompleteTutorialSignal>(CompleteStep);
            _signalBus.Subscribe<TutorialPointerSignal>(ShowPointer);
            _signalBus.Subscribe<SetTutorialStepSignal>(SetSubStep);
            _signalBus.Subscribe<TutorialViewPointerSignal>(ShowHelpViewWithPointer);
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<ShowTutorialSignal>(Show);
            _signalBus.TryUnsubscribe<CompleteTutorialSignal>(CompleteStep);
            _signalBus.TryUnsubscribe<TutorialPointerSignal>(ShowPointer);
            _signalBus.TryUnsubscribe<SetTutorialStepSignal>(SetSubStep);
            _signalBus.TryUnsubscribe<TutorialViewPointerSignal>(ShowHelpViewWithPointer);
        }

        public void Setup()
        {
            current = _dataService.GetTutorialCurrentStep();
            if(current == TutorialStepType.NONE) Destroy(this);
            if (current == TutorialStepType.TUTORIAL_BATTLE)
            {
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_game_start);
                current = TutorialStepType.SHOOT_HIM;
            }
        }

        private void SetSubStep(SetTutorialStepSignal signal)
        {
            current = signal.StepType;
            _dataService.SetTutorialCurrentStep(current);
        }
    
        private void Show()
        {
            TutorialAttribute attribute = current.GetAttribute<TutorialAttribute>();
        
            if (attribute.NeedCrocoDialog)
            {
                _analytics.SendEvent(attribute.AmplitudeStartEvent);
                Transform canvas = GameObject.FindWithTag("tutorialCanvas").transform;
                view = Instantiate(_helpViewPrefab, canvas);
                view.Setup(current);
                
            }
        }

        private void ShowHelpViewWithPointer()
        {
            TutorialStepType currentStep = _dataService.GetTutorialCurrentStep();
            Show();
            _signalBus.Fire<TutorialSendTargetSignal>();
            if(currentStep == TutorialStepType.READY_BUTTON_HELP || currentStep == TutorialStepType.READY_BUTTON_HELP_2 ||
               currentStep == TutorialStepType.READY_BUTTON_HELP_3 ||
               currentStep == TutorialStepType.READY_BUTTON_HELP_4 )
                _signalBus.Fire<ButtonInteractableInTutor>();
        }
        
        private void ShowPointer(TutorialPointerSignal signal)
        {
            _mainCanvas = GameObject.FindWithTag("mainCanvas").transform;
        
            TutorialAttribute attribute = current.GetAttribute<TutorialAttribute>();
            
            if (attribute.PointerType == PointerType.HAND)
            {
                _analytics.SendEvent(attribute.AmplitudeStartEvent);
                Transform target = signal.PointerEndTarget;
                Transform targetCard = signal.PointerTarget;
                WeaponType weaponType = signal.WeaponType;
                bool isMerge = signal.IsMergeTutorial; 
                _pointer = Instantiate(_pointerPrefab, _mainCanvas.transform);
                _pointer.transform.position = targetCard.position;
                if (attribute.IsMirror)
                {
                    _pointer.transform.localScale = new Vector3(_pointer.transform.localScale.x * -1, _pointer.transform.localScale.y,_pointer.transform.localScale.z);
                }
                _pointer.Setup(attribute.PointerType, target, targetCard, weaponType, isMerge, attribute.IsMirror);
            }
            else if (attribute.PointerType == PointerType.ARROW)
            {
                _analytics.SendEvent(attribute.AmplitudeStartEvent);
                if (_pointer != null)
                    Destroy(_pointer.gameObject);
                Transform target = signal.PointerTarget;
                _pointer = Instantiate(_pointerPrefab, _mainCanvas.transform);
                _pointer.transform.position = target.position;
                if (attribute.IsMirror)
                {
                    _pointer.transform.localScale = new Vector3(_pointer.transform.localScale.x * -1, _pointer.transform.localScale.y,_pointer.transform.localScale.z);
                }
                _pointer.Setup(attribute.PointerType, target,null, WeaponType.NONE, false, attribute.IsMirror, delay: attribute.Dealy);
            }
            else if (attribute.PointerType == PointerType.MASK)
            {
                if (_pointer != null)
                    Destroy(_pointer.gameObject);
                Transform target = signal.PointerTarget; 
                _pointer = Instantiate(_pointerPrefab, _mainCanvas.transform);
                _pointer.Setup(attribute.PointerType, target,null, WeaponType.NONE, false, attribute.IsMirror);
                if(view != null) 
                    view.SetBlackOutProperties(target);
            }
            else if (attribute.PointerType == PointerType.WHOLE)
            {
                _analytics.SendEvent(attribute.AmplitudeStartEvent);
                if (_pointer != null)
                    Destroy(_pointer.gameObject);
                Transform target = signal.PointerTarget;  
                view = Instantiate(_helpViewPrefab, _mainCanvas);
                view.SetupWhole();
                if(view != null) 
                    view.SetBlackOutProperties(target);
            }
            else if (attribute.PointerType == PointerType.ARROW_AND_WHOLE)
            {
                _analytics.SendEvent(attribute.AmplitudeStartEvent);
                if (_pointer != null)
                    Destroy(_pointer.gameObject);
                Transform target = signal.PointerTarget; 
                view = Instantiate(_helpViewPrefab, _mainCanvas);
                view.SetupWhole();
                if(view != null) 
                    view.SetBlackOutProperties(target);
                _pointer = Instantiate(_pointerPrefab, _mainCanvas.transform);
                _pointer.transform.position = target.position;
                _pointer.Setup(attribute.PointerType, target,null, WeaponType.NONE, false, attribute.IsMirror, delay: attribute.Dealy);
            }
        }
        
        
        private void CompleteStep()
        {
            if (view != null)
            {
                if(current.GetAttribute<TutorialAttribute>().IsCloseAfterFirstClick)
                    view.Fade();
            }
            if(_pointer != null)
                Destroy(_pointer.gameObject);
            _dataService.SetTutorialStepComplete(current);
            if (current == TutorialStepType.MERGE_POINTER_2)
            {
                _dataService.SetExtensionUnlock(Extension.MERGE_TUTOR_END, true);
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_merge_complete);
            }
            if (current != TutorialStepType.TUTORIAL_BATTLE)
            {
                _analytics.SendEvent(current.GetAttribute<TutorialAttribute>().AmplitudeCompleteEvent);
            } 
            current = current.GetAttribute<TutorialAttribute>().NextStep;
            _dataService.SetTutorialCurrentStep(current);
        }
    }

    public enum TutorialStepType
    {
        [Tutorial()] NONE = 0,

        #region MainTutorial

        [Tutorial(SHOOT_HIM,  PointerType.NONE, false, "", true, false)] 
        TUTORIAL_BATTLE = 1,
        [Tutorial(SHOOT_POINTER,  PointerType.NONE, true, "Стреляй в него!", true, false, AmplitudeTutorialEvent.tutor_game_dialog_1_start, AmplitudeTutorialEvent.tutor_game_dialog_1_complete)] 
        SHOOT_HIM = 2,
        [Tutorial(MANA_HELP,  PointerType.HAND, false, "", true, false, AmplitudeTutorialEvent.tutor_game_weapon_1_start, AmplitudeTutorialEvent.tutor_game_weapon_1_complete)] 
        SHOOT_POINTER = 3,
        [Tutorial(READY_BUTTON_HELP,  PointerType.MASK, true, "Нужна энергия, чтобы использовать оружие. Каждый новый раунд энергия восстанавливается.", true, false, AmplitudeTutorialEvent.tutor_game_mana_start, AmplitudeTutorialEvent.tutor_game_mana_complete, true, true)] 
        MANA_HELP = 4,
        [Tutorial(BOMB_HELP,  PointerType.ARROW, false,"",true, false, AmplitudeTutorialEvent.tutor_game_round_1_start, AmplitudeTutorialEvent.tutor_game_round_1_complete, true, true, true, 2f)] 
        READY_BUTTON_HELP = 5,
        [Tutorial(BOMB_HELP_POINTER, PointerType.MASK, true, "Новая карта! Конечно! Их всегда 4 к новому раунду. Используй её!", true, false, AmplitudeTutorialEvent.tutor_game_dialog_2_start, AmplitudeTutorialEvent.tutor_game_dialog_2_complete, true)] 
        BOMB_HELP = 6,
        [Tutorial(BRUSH_HELP, PointerType.HAND, false, "", true, false, AmplitudeTutorialEvent.tutor_game_weapon_2_start, AmplitudeTutorialEvent.tutor_game_weapon_2_complete, false)] 
        BOMB_HELP_POINTER = 7,
        [Tutorial(BRUSH_HELP_POINTER, PointerType.NONE, true, "У нас осталась энергия. Давай ударим ближним боем.", true, false, AmplitudeTutorialEvent.tutor_game_dialog_3_start, AmplitudeTutorialEvent.tutor_game_dialog_3_complete)]
        BRUSH_HELP = 8,
        [Tutorial(READY_BUTTON_HELP_2, PointerType.HAND, false, "", true, false, AmplitudeTutorialEvent.tutor_game_weapon_3_start, AmplitudeTutorialEvent.tutor_game_weapon_3_complete)] 
        BRUSH_HELP_POINTER = 9,
        [Tutorial(UMBRELLA_HELPER, PointerType.ARROW, false, "", true, false, AmplitudeTutorialEvent.tutor_game_round_2_start, AmplitudeTutorialEvent.tutor_game_round_2_complete, true, true)] 
        READY_BUTTON_HELP_2 = 10,
        [Tutorial(UMBRELLA_HELP_POINTER, PointerType.MASK, true, "Тебе выпала карта брони, используй ее", true, false, AmplitudeTutorialEvent.tutor_game_dialog_4_start, AmplitudeTutorialEvent.tutor_game_dialog_4_complete)] 
        UMBRELLA_HELPER = 11,
        [Tutorial(HEALING_HELPER, PointerType.HAND, false, "", true, false, AmplitudeTutorialEvent.tutor_game_weapon_4_start, AmplitudeTutorialEvent.tutor_game_weapon_4_complete)] 
        UMBRELLA_HELP_POINTER = 12,
        [Tutorial(HEALING_HELP_POINTER, PointerType.MASK, true, "Остатки энергии стоит потратить на лечение. Используй карту", true, false, AmplitudeTutorialEvent.tutor_game_dialog_5_start, AmplitudeTutorialEvent.tutor_game_dialog_5_complete)] 
        HEALING_HELPER = 13,
        [Tutorial(READY_BUTTON_HELP_3, PointerType.HAND, false, "", true, false, AmplitudeTutorialEvent.tutor_game_weapon_5_start, AmplitudeTutorialEvent.tutor_game_weapon_5_complete)] 
        HEALING_HELP_POINTER = 14,
        [Tutorial(MINA_HELPER, PointerType.ARROW, false, "", true, false, AmplitudeTutorialEvent.tutor_game_round_3_start, AmplitudeTutorialEvent.tutor_game_round_3_complete, true, true)] 
        READY_BUTTON_HELP_3 = 15,
        [Tutorial(MINA_HELP_POINTER, PointerType.MASK, true, "Против ближнего боя хорошо работает мина. Попробуй.", true, false, AmplitudeTutorialEvent.tutor_game_dialog_6_start, AmplitudeTutorialEvent.tutor_game_dialog_6_complete)] 
        MINA_HELPER = 16,
        [Tutorial(READY_BUTTON_HELP_4, PointerType.HAND, false, "", true, false, AmplitudeTutorialEvent.tutor_game_weapon_6_start, AmplitudeTutorialEvent.tutor_game_weapon_6_complete)] 
        MINA_HELP_POINTER = 17,
        [Tutorial(FINISH_HIM, PointerType.ARROW, false, "", true, false, AmplitudeTutorialEvent.tutor_game_round_4_start, AmplitudeTutorialEvent.tutor_game_round_4_complete, true, true)] 
        READY_BUTTON_HELP_4 = 18,
        [Tutorial(BATTLE_TUTORIAL_END, PointerType.MASK, true, "Добивай его!", true, false, AmplitudeTutorialEvent.tutor_game_dialog_7_start, AmplitudeTutorialEvent.tutor_game_dialog_7_complete)] 
        FINISH_HIM = 19,
        
        [Tutorial(AFTER_WIN, PointerType.MASK, true, "", true)] 
        BATTLE_TUTORIAL_END = 20,
        
        
        [Tutorial(FIRST_CHEST,  PointerType.NONE, true, "Вот это бой! Спасибо, что подменил меня. Ты доказал, что достоин сражаться на арене", false, false, AmplitudeTutorialEvent.tutor_chest_dialog_1_start, AmplitudeTutorialEvent.tutor_chest_dialog_1_complete, false, true, false, 0, false)]
        AFTER_WIN = 21,
        
        [Tutorial(OPEN_CHEST, PointerType.NONE, true, "Но тебе нужна собственная коллекция оружия. Лови подарок и готовься к бою!", false, false, AmplitudeTutorialEvent.tutor_chest_dialog_2_start, AmplitudeTutorialEvent.tutor_chest_dialog_2_complete)]
        FIRST_CHEST = 22,
            
        [Tutorial(MOVE_TO_GARAGE, PointerType.NONE, false, "", false,false, AmplitudeTutorialEvent.tutor_chest_open_start,"" /*AmplitudeTutorialEvent.tutor_chest_open_complete*/)]
        OPEN_CHEST = 23,
        
        [Tutorial(EQUIP_PREBATTLE, PointerType.ARROW, false, "", false, false, AmplitudeTutorialEvent.tutor_weapon_button_start, "")]
        MOVE_TO_GARAGE = 24,
        
        [Tutorial(FIRST_WEAPON_POINTER, PointerType.NONE, true, "Здесь ты можешь собирать колоду для боя", false, false, AmplitudeTutorialEvent.tutor_weapon_dialog_start, AmplitudeTutorialEvent.tutor_weapon_dialog_complete)]
        EQUIP_PREBATTLE = 25,

        [Tutorial(AUTO_BUILD_HELP, PointerType.HAND, false, "", false, false, AmplitudeTutorialEvent.tutor_weapon_change_start, AmplitudeTutorialEvent.tutor_weapon_change_complete)] 
        FIRST_WEAPON_POINTER = 26,
        
        [Tutorial(AUTO_BUILD_POINTER_HELP, PointerType.NONE, true, "Нажми кнопку авто, чтобы быстро отправить все карты в колоду", false, false, AmplitudeTutorialEvent.tutor_weapon_auto_start, "")]
        AUTO_BUILD_HELP = 27,
        
        [Tutorial(CLOSE_WEAPON_COLLECTION_DIALOG_POINTER_HELP, PointerType.ARROW_AND_WHOLE, false, "", false, false, "", AmplitudeTutorialEvent.tutor_weapon_auto_complete)]
        AUTO_BUILD_POINTER_HELP = 28,
        
        [Tutorial(MOVE_TO_BATH_COLLECTION_DIALOG, PointerType.ARROW, false, "", false, false, "", AmplitudeTutorialEvent.tutor_weapon_complete)]
        CLOSE_WEAPON_COLLECTION_DIALOG_POINTER_HELP = 29,
        
        [Tutorial(EQUIP_BATH_HELP, PointerType.ARROW, false, "", false, false, AmplitudeTutorialEvent.tutor_bath_start, AmplitudeTutorialEvent.tutor_bath_button_complete)]
        MOVE_TO_BATH_COLLECTION_DIALOG = 30,
        
        [Tutorial(FIRST_BATH_POINTER, PointerType.NONE, true, "Здесь ты можешь менять ванны", false, false, AmplitudeTutorialEvent.tutor_bath_dialog_start, AmplitudeTutorialEvent.tutor_bath_dialog_complete)]
        EQUIP_BATH_HELP = 31,
        
        [Tutorial(CLOSE_BATH_COLLECTION_DIALOG_POINTER_HELP, PointerType.HAND, false, "", false, false, AmplitudeTutorialEvent.tutor_bath_change_start, AmplitudeTutorialEvent.tutor_bath_change_complete)] 
        FIRST_BATH_POINTER = 32,
        
        [Tutorial(RANDOM_BATTLE_POINTER_HELP, PointerType.ARROW, false, "", false, false, "", AmplitudeTutorialEvent.tutor_bath_complete)]
        CLOSE_BATH_COLLECTION_DIALOG_POINTER_HELP = 33,
        
        [Tutorial(MAIN_TUTORIAL_END, PointerType.ARROW, false, "", false, false, AmplitudeTutorialEvent.tutor_game_button_start, AmplitudeTutorialEvent.tutor_game_button_complete)]
        RANDOM_BATTLE_POINTER_HELP = 34,
        
        
        
        
        
        [Tutorial(NONE, PointerType.NONE, false, "", false, false)]
        MAIN_TUTORIAL_END = 35,

        
        [Tutorial(TAKE_CHEST_POINTER_HELP, PointerType.NONE, true, "Пора открыть новый сундук с оружием", false, false, AmplitudeTutorialEvent.tutor_new_chest_dialog_start, AmplitudeTutorialEvent.tutor_new_chest_dialog_complete, false, true, false, 0, true)]
        TAKE_CHEST_HELP = 36,
        
        [Tutorial(OPEN_CHEST_POINTER_HELP,  PointerType.WHOLE, false, "", false, false, AmplitudeTutorialEvent.tutor_new_chest_pointer_start, AmplitudeTutorialEvent.tutor_new_chest_pointer_complete, true, true)] 
        TAKE_CHEST_POINTER_HELP = 37,
        
        [Tutorial(OPEN_CHEST_ON_LOCATION,  PointerType.WHOLE, false, "", false, false, AmplitudeTutorialEvent.tutor_new_chest_open_start, AmplitudeTutorialEvent.tutor_new_chest_open_complete, true, true)] 
        OPEN_CHEST_POINTER_HELP = 38,
        
        [Tutorial(NONE, PointerType.NONE, false, "", false, false)]
        OPEN_CHEST_ON_LOCATION = 39,
        
        #endregion
        
        #region Merge

        [Tutorial(MERGE_OPEN_DIALOG, PointerType.NONE, true, "Кажется ты получил дубликат оружия. Зайди в гараж - покажу, что делать", false, false, AmplitudeTutorialEvent.tutor_merge_dialog_1_start, AmplitudeTutorialEvent.tutor_merge_dialog_1_complete)]
        MERGE = 40,
    
        [Tutorial(MERGE_IN_DIALOG_HELP, PointerType.ARROW, false, "", false, true, global::App.Analytic.AmplitudeTutorialEvent.tutor_merge_button_start, AmplitudeTutorialEvent.tutor_merge_button_complete)]
        MERGE_OPEN_DIALOG = 41,
    
        [Tutorial(MERGE_POINTER_1, PointerType.NONE, true, "Здесь можно соединить дубликаты карты и усилить характеристики оружия. Попробуй!", false, false, AmplitudeTutorialEvent.tutor_merge_dialog_2_start, AmplitudeTutorialEvent.tutor_merge_dialog_2_complete)]
        MERGE_IN_DIALOG_HELP = 42,
    
        [Tutorial(MERGE_POINTER_2, PointerType.HAND, false, "", false, false, global::App.Analytic.AmplitudeTutorialEvent.tutor_merge_card_1_start, AmplitudeTutorialEvent.tutor_merge_card_1_complete)]
        MERGE_POINTER_1 = 43,
    
        [Tutorial(MERGE_TUTORIAL_END, PointerType.HAND, false, "", false, false, global::App.Analytic.AmplitudeTutorialEvent.tutor_merge_card_2_start, AmplitudeTutorialEvent.tutor_merge_card_2_complete)]
        MERGE_POINTER_2 = 44,
        
        [Tutorial(NONE, PointerType.NONE, false, "", false, false)]
        MERGE_TUTORIAL_END = 45,

        #endregion
        
        
        #region Boss

        [Tutorial(BOSS_POINTER, PointerType.NONE, true, "Твои победы заметил местный главарь. Он ждет тебя на арене", false, false, AmplitudeTutorialEvent.tutor_boss_start, AmplitudeTutorialEvent.tutor_boss_dialog_start)] 
        BOSS_OPEN = 46,
        [Tutorial(BOSS_BUTTON_POINTER, PointerType.ARROW, false, "", false, true, AmplitudeTutorialEvent.tutor_boss_dialog_complete)] 
        BOSS_POINTER = 47,
        [Tutorial(END_BOSS_TUTOR, PointerType.ARROW, false, "", false, true, AmplitudeTutorialEvent.tutor_boss_button_start)] 
        BOSS_BUTTON_POINTER = 48,
        [Tutorial(NONE, PointerType.NONE, false, "", false, true)] 
        END_BOSS_TUTOR = 49,
    

        #endregion*/
    }

    public class TutorialAttribute : Attribute
    {
        private TutorialStepType nextStep;
        private PointerType _pointerType;
        private string defaultText;
        private float _delay;
        private bool _isNeedDelay;
        private bool needCrocoDialog;
        private bool isBattle;
        private bool isMirror;
        private bool isMaskNeed;
        private bool isCloseAfterFirstClick;
        

        private string amplitudeStartEvent;
        private string amplitudeCompleteEvent;

        private bool _needDelayafterClick;
        
        public TutorialAttribute(TutorialStepType nextStep = TutorialStepType.NONE, 
            PointerType _pointerType = PointerType.NONE, bool needCrocoDialog = false, 
            string defaultText = "", bool isBattle = false, bool isMirror = false, string amplitudeStartEvent ="", string amplitudeCompleteEvent ="", bool isMaskNeed = false, bool isCloseAfterFirstClick = true,
            bool isNeedDelay = false, float delay = 0, bool needDelayafterClick = false)
        {
            this.nextStep = nextStep;
            this._pointerType = _pointerType;
            this.needCrocoDialog = needCrocoDialog;
            this.defaultText = defaultText;
            this.isBattle = isBattle;
            this.isMirror = isMirror;
            this.amplitudeStartEvent = amplitudeStartEvent;
            this.amplitudeCompleteEvent = amplitudeCompleteEvent;
            this.isMaskNeed = isMaskNeed;
            this.isCloseAfterFirstClick = isCloseAfterFirstClick;
            this._isNeedDelay = isNeedDelay;
            this._delay = delay;
            this._needDelayafterClick = needDelayafterClick;
        }
    

        public TutorialStepType NextStep => nextStep;
        public PointerType PointerType => _pointerType;
        public bool NeedCrocoDialog => needCrocoDialog;
        public bool IsMaskNeed => isMaskNeed;
        public string DefaultText => defaultText;
        public bool IsBattle => isBattle;
        public bool IsCloseAfterFirstClick => isCloseAfterFirstClick;
        public bool IsNeedDelay => _isNeedDelay;
        public float Dealy => _delay;
        public bool IsMirror => isMirror;

        public string AmplitudeStartEvent => amplitudeStartEvent;

        public string AmplitudeCompleteEvent => amplitudeCompleteEvent;
        public bool NeedDelayAfterClick => _needDelayafterClick;
    }

    public enum PointerType
    {
        NONE = 0,
        HAND = 1,
        ARROW = 2,
        MASK = 3,
        WHOLE = 4,
        ARROW_AND_WHOLE = 5
    }
}