using App.Chests;
using App.Dialogs;
using App.Dialogs.BathCollection;
using App.Dialogs.BattlePass;
using App.Dialogs.CardCollections;
using App.Dialogs.EndGameDialog;
using App.Dialogs.Merge;
using App.Dialogs.Settings;
using App.Dialogs.Shop;
using App.SetupsScript;
using UnityEngine;
using Zenject;

namespace Library.Dialogs
{
    public class DialogsContainer : MonoBehaviour
    {
        [SerializeField]private RectTransform ParentRectTransform;

        protected DialogController _dialogController;
     
        
        [Header("DIALOGS")]
        [SerializeField] protected SettingsDialog SettingsDialog;
        [SerializeField] protected ChestsContainerDialog ChestsContainerDialog;
        [SerializeField] protected MergeDialog MergeDialog;
        [SerializeField] protected ChestsDialog _chestsDialog;
        [SerializeField] protected PassiveDialog _passiveDialog;
        [SerializeField] protected ExitBattleDialog _battleDialog;
        [SerializeField] protected BossBattleEndDialog _bossBattleEndDialog;
        [SerializeField] protected WeaponCollectionDialog weaponCollectionDialog;
        [SerializeField] protected BathCollectionDialog bathCollectionDialog;
        [SerializeField] protected SaveProgressDialog saveProgressDialog;
        [SerializeField] protected LoadProgressDialog loadProgressDialog;
        [SerializeField] protected TakeCardsDialog takeCardsDialog;
        [SerializeField] protected BattlePassDialog battlePassDialog;
        [SerializeField] protected RestorePurchasesDialog restorePurchasesDialog;
        
        
        [Inject] protected BathSetup _bathSetup;
        [Inject] protected WeaponSetup _weaponSetup;
        [Inject] protected ChestSetup _chestSetup;

        [SerializeField] protected AchievementDialog achievementDialog;
        [SerializeField] protected BoostersDialog boostersDialog;
        [SerializeField] protected BuyBoosterDialog buyBoosterDialog;
        [SerializeField] protected PremPurchaseDialog premshopDialog;
        [SerializeField] protected PurchasedDialog purchasedDialog;
        public bool HasActiveDialog() => _dialogController.HasActiveDialog();
        
        public bool OnBackKey()
        {
            if (_dialogController.HasActiveDialog())
            {
                _dialogController.CloseTopDialog();
                return true;
            }
            return false;
        }


        public void AddCountChangeListener(DialogController.CountChangeHandler countChangeHandler)
        {
            if (ParentRectTransform == null)
                ParentRectTransform = GameObject.FindWithTag("mainCanvas").GetComponent<RectTransform>();
            
            if (_dialogController == null)
                _dialogController = new DialogController(ParentRectTransform);
            
            _dialogController.OnCountChanged += countChangeHandler;
        }

        public void RemoveCountChangeListener(DialogController.CountChangeHandler countChangeHandler)
        {
            _dialogController.OnCountChanged -= countChangeHandler;
        }
        
        
        private void Awake()
        {
            if (ParentRectTransform == null) ParentRectTransform = GameObject.FindWithTag("mainCanvas").GetComponent<RectTransform>();
            _dialogController = new DialogController(ParentRectTransform);
        }

        public void ShowSettingsDialog()
        {
            ShowDialog(Instantiate(SettingsDialog.gameObject));
        } 
        public void ShowMergeDialog()
        {
            MergeDialog mergeDialog = Instantiate(MergeDialog);
            ShowDialog(mergeDialog.gameObject);
        }

        public GameObject ShowChestsContainerDialog()
        {
            ChestsContainerDialog obj = Instantiate(ChestsContainerDialog);
            obj.SetChestSetup(_chestSetup);
            ShowDialog(obj.gameObject);
            return obj.gameObject;
        }

        public void ShowPassiveDialog() => ShowDialog(Instantiate(_passiveDialog).gameObject);

        public void ShowExitBattleDialog() => ShowDialog(Instantiate(_battleDialog).gameObject);

        public void ShowWeaponCollectionDialog()
        {
            WeaponCollectionDialog dialog = Instantiate(weaponCollectionDialog);
            dialog.SetupWeaponSetup(_weaponSetup);
            ShowDialog(dialog.gameObject);
        }

        public void ShowBathCollectionDialog()
        {
            BathCollectionDialog dialog = Instantiate(bathCollectionDialog);
            dialog.Setup(_bathSetup);
            ShowDialog(dialog.gameObject);
        }

        public ChestsDialog ShowChestDialog()
        {
            ChestsDialog dialog = Instantiate(_chestsDialog);
            ShowDialog(dialog.gameObject);
            dialog.gameObject.SetActive(false);
            return dialog;
        }

        private void ShowDialog(GameObject dialog)
        {
            if (ParentRectTransform == null) ParentRectTransform = GameObject.FindWithTag("mainCanvas").GetComponent<RectTransform>();
            _dialogController = new DialogController(ParentRectTransform);
            _dialogController.ShowDialog(dialog);
            dialog.GetComponent<RectTransform>().sizeDelta = dialog.GetComponent<RectTransform>().parent.GetComponent<RectTransform>().sizeDelta;
        }
        
        public BossBattleEndDialog ShowBossBattleEndDialog()
        {
            BossBattleEndDialog dialog = Instantiate(_bossBattleEndDialog);
            ShowDialog(dialog.gameObject);
            return dialog;
        }


        public SaveProgressDialog ShowSaveProgressPopup()
        {
            SaveProgressDialog dialog = Instantiate(saveProgressDialog);
            ShowDialog(dialog.gameObject);
            return dialog;
        }

        public RestorePurchasesDialog ShowRestorePurchases()
        {
            RestorePurchasesDialog dialog = Instantiate(restorePurchasesDialog);
            ShowDialog(dialog.gameObject);
            return dialog;
        }

        public LoadProgressDialog ShowLoadProgressPopup()
        {
            LoadProgressDialog dialog = Instantiate(loadProgressDialog);
            ShowDialog(dialog.gameObject);
            return dialog;
        }

        public TakeCardsDialog ShowTakeCardsDialog()
        {
            TakeCardsDialog dialog = Instantiate(takeCardsDialog);
            dialog.SetChestSetup(_chestSetup);
            ShowDialog(dialog.gameObject);
            return dialog;
        }
        
        public BattlePassDialog ShowBattlePassDialog()
        {
            BattlePassDialog dialog = Instantiate(battlePassDialog);
            ShowDialog(dialog.gameObject);
            dialog.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            //_dialogController.ShowDialog(dialog.gameObject);
            return dialog;
        }

        public AchievementDialog ShowAchievementsDialog()
        {
            AchievementDialog dialog = Instantiate(achievementDialog);
            ShowDialog(dialog.gameObject);
            if ((float)Screen.width / Screen.height > 2)
                dialog.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0);
            return dialog;
        }

        public BoostersDialog ShowBoostersDialog()
        {
            BoostersDialog dialog = Instantiate(boostersDialog);
            ShowDialog(dialog.gameObject);
            if ((float)Screen.width / Screen.height > 2)
                dialog.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0);
            return dialog;
        }

        public BuyBoosterDialog ShowBuyBoosterDialog()
        {
            BuyBoosterDialog dialog = Instantiate(buyBoosterDialog);
            ShowDialog(dialog.gameObject);
            return dialog;
        }

        public PremPurchaseDialog ShowPremShopDialog()
        {
            if (ParentRectTransform == null) ParentRectTransform = GameObject.FindWithTag("mainCanvas").GetComponent<RectTransform>();
            PremPurchaseDialog dialog = Instantiate(premshopDialog, ParentRectTransform);
            ShowDialog(dialog.gameObject);
            return dialog;
        }

        public PurchasedDialog ShowPurchasingNotification()
        {
            PurchasedDialog dialog = Instantiate(purchasedDialog);
            ShowDialog(dialog.gameObject);
            return dialog;
        }
    }
}