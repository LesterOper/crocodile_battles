using App.DAO;
using CodeBase;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using I2.Loc;
using Library.Common;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using Zenject;

namespace App.Dialogs.Settings
{
    public class SettingsDialog : Dialog
    {
        [SerializeField] private Button save;
        [SerializeField] private Button load;
        
        [Header("social")]
        [SerializeField] private Button insta;
        [SerializeField] private Button vk;
        [SerializeField] private Button facebook;
        [SerializeField] private Button youtube;
        [SerializeField] private Button mail;

        [Header("Localization")] 
        [SerializeField] private Button prevLang;
        [SerializeField] private Button nextLang;
        [SerializeField] private TextMeshProUGUI lang;

        [Header("Sound")]
        [SerializeField] private TextMeshProUGUI soundText;
        [SerializeField] private Image soundImg;
        [SerializeField] private Sprite active, nonActive;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Button restore;

        private const string SOUND_ON = "SOUND_ON";
        private const string SOUND_OFF = "SOUND_OFF";
        protected override void Start()
        {
            base.Start();
            Save();
            Load();
            OpenSocialUrl(insta, Constants.INSTA);
            OpenSocialUrl(vk, Constants.VK);
            OpenSocialUrl(mail, Constants.GetMail());
            OpenSocialUrl(facebook, Constants.FB);
            OpenSocialUrl(youtube, Constants.YOUTUBE);
            NextLanguage();
            PreviousLanguage();
            lang.text = LocalizationManager.CurrentLanguageCode;

            gameObject.transform.DOPunchScale(Vector3.one*0.15f, 0.2f, 1, 0);
            gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        
            var musicVolume = 0f;
            musicVolume = PlayerPrefs.HasKey("music_volume") ? PlayerPrefs.GetFloat("music_volume") : 1f;
            soundText.text =!(musicVolume < 0.05)
                ? LocalizationManager.GetTranslation(SOUND_ON)
                : LocalizationManager.GetTranslation(SOUND_OFF);
            AudioListener.volume = musicVolume;
            _musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            _musicSlider.value = musicVolume;
            soundImg.sprite = !(musicVolume < 0.05) ? active : nonActive;
            restore.onClick.AddListener(RestorePurchases);
        }

        private void RestorePurchases()
        {
            BillingServices.OnRestorePurchasesComplete += RestorePurchaseNotification;
            BillingServices.RestorePurchases();
            
        }

        private static void RestorePurchaseNotification(BillingServicesRestorePurchasesResult restoreResult, Error error)
        {
            FindObjectOfType<DialogsContainer>().ShowRestorePurchases();
            BillingServices.OnRestorePurchasesComplete -= RestorePurchaseNotification;
        }
        
        private void ChangeMusicVolume(float volume)
        {
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat("music_volume", volume);
            soundImg.sprite = (volume < 0.05f) ? nonActive : active;
            soundText.text =!(volume < 0.05)
                ? LocalizationManager.GetTranslation(SOUND_ON)
                : LocalizationManager.GetTranslation(SOUND_OFF);
        }

        private void Save() => save.onClick.AddListener(() =>
        {
            _hideDialogButtonArea.interactable = false;
            _closeButton.interactable = false;
            FindObjectOfType<SavingHelper>().SaveGame(status =>
            {
                _hideDialogButtonArea.interactable = true;
                _closeButton.interactable = true;
                Debug.Log("[SavingHelper] - completeHandler");
                if (status)
                {
                    Debug.Log("[SavingHelper] - completeHandler : " + status);
                    _dataService.SetLastSavedTime();
                    FindObjectOfType<DialogsContainer>().ShowSaveProgressPopup();
                }
            });
        });

        private void Load() => load.onClick.AddListener(() =>
        {
            _hideDialogButtonArea.interactable = false;
            _closeButton.interactable = false;
            FindObjectOfType<SavingHelper>().LoadCloudData(data =>
            {
                if (data != null)
                {
                    _dataService.ApplyData(data);
                    FindObjectOfType<DialogsContainer>().ShowLoadProgressPopup();
                    DOVirtual.DelayedCall(2, () => SceneManager.LoadScene(1));
                }
            });
        });
        
        private void OpenSocialUrl(Button button, string url) => button.onClick.AddListener(() => Application.OpenURL(url));

        private void NextLanguage() => nextLang.onClick.AddListener(() =>
        {
            bool isSoundActive = PlayerPrefs.GetInt(Constants.SOUND, 1) == 1;
            int langId = LocalizationManager.GetAllLanguages().IndexOf(LocalizationManager.CurrentLanguage);
            if (langId == LocalizationManager.GetAllLanguages().Count-1)
            {
                LocalizationManager.CurrentLanguage = LocalizationManager.GetAllLanguages()[0];
            }
            else LocalizationManager.CurrentLanguage = LocalizationManager.GetAllLanguages()[langId + 1];
            lang.text = LocalizationManager.CurrentLanguageCode;
            soundText.text = isSoundActive
                ? LocalizationManager.GetTranslation(SOUND_ON)
                : LocalizationManager.GetTranslation(SOUND_OFF);
        });
        private void PreviousLanguage() => prevLang.onClick.AddListener(() =>
        {
            bool isSoundActive = PlayerPrefs.GetInt(Constants.SOUND, 1) == 1;
            int langId = LocalizationManager.GetAllLanguages().IndexOf(LocalizationManager.CurrentLanguage);
            if (langId <= 0)
            {
                LocalizationManager.CurrentLanguage = LocalizationManager.GetAllLanguages()[LocalizationManager.GetAllLanguages().Count-1];
            }
            else LocalizationManager.CurrentLanguage = LocalizationManager.GetAllLanguages()[langId - 1];
            lang.text = LocalizationManager.CurrentLanguageCode;
            soundText.text = isSoundActive
                ? LocalizationManager.GetTranslation(SOUND_ON)
                : LocalizationManager.GetTranslation(SOUND_OFF);
        });
    }
}