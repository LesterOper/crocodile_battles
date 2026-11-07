using App.Audio;
using App.DAO;
using App.SetupsScript;
using Library.Common;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Library.Dialogs
{
    public class Dialog : MonoBehaviour
    {
        public delegate void DialogHiddenHandler(GameObject dialog);
        public delegate void DialogShownHandler(GameObject dialog);
        private readonly bool CanUnMuteCogs = false;
        private GameObject[] _audio;
        private float[] _audioVolume;
        [SerializeField] protected Button _closeButton;
        [SerializeField] protected Button _hideDialogButtonArea;
        private GameObject[] _locationElements;
        private float[] _locationElementsVolume;
        private bool CanUnMute;
        [Inject] protected SignalBus _signalBus;
        [Inject] protected IDataService _dataService;
        [Inject] protected SoundPlayer _soundPlayer;


        public bool Visible { get; private set; }

        private event DialogShownHandler OnDialogShown;
        private event DialogHiddenHandler OnDialogHidden;

        protected virtual void Start()
        {
            
            if (_closeButton != null)
                _closeButton.onClick.AddListener(Hide);

            if (_hideDialogButtonArea != null)
                _hideDialogButtonArea.onClick.AddListener(Hide);

            if (GetComponent<Button>() != null)
                GetComponent<Button>().onClick.AddListener(Hide);
            
            if (!Visible)
            {
                Visible = true;
                NotifyListeners();
            }
            Invoke(nameof(MuteAudio), 0.1f);
        }

        public void MuteAudio()
        {
            CanUnMute = true;
        }

        private void MuteCogs()
        {
            _locationElements = GameObject.FindGameObjectsWithTag("_cog");
            var index = 0;

            _locationElementsVolume = new float[_locationElements.Length];

            foreach (var a in _locationElements)
            {
                _locationElementsVolume[index] = a.GetComponent<AudioSource>().volume;
                a.GetComponent<AudioSource>().volume = 0.1f;
                index++;
            }
        }

        public void UNMuteAudio()
        {

        }

        private void UNMuteCogs()
        {
            if (CanUnMuteCogs)
            {
                var index = 0;
                foreach (var a in _locationElements)
                {
                    a.GetComponent<AudioSource>().volume = _locationElementsVolume[index];
                    index++;
                }
            }

            //_audioMixerController_1.SetMixerGroupVolume(AudioMixerController.MasterMixerGroup.SFX, 0.1f);
        }

        public virtual void Show(RectTransform parentTransform)
        {
            if (!Visible)
            {
                //Context.SoundPlayer.PlaySound("open_window_1", AudioMixerController.MasterMixerGroup.SFXCONST, 1);
                _soundPlayer.PlaySound("window_opn_01", AudioMixerController.MasterMixerGroup.SFX, 0.5f);
                gameObject.SetActive(true);
                GetComponent<RectTransform>().SetParent(parentTransform);
                Visible = true;
                NotifyListeners();
            }
        }

        public void SetOnTopCanvas()
        {
            transform.parent = GetComponentInParent<Canvas>().transform;
            transform.SetAsLastSibling();
        }

        public virtual void ForceHide()
        {
            gameObject.SetActive(false);
            Visible = false;
            NotifyListeners();
            UNMuteAudio();
            Destroy(gameObject);
        }

        public virtual void Hide()
        {
            if (Visible)
            {
                _soundPlayer.PlaySound("window_cls_01", AudioMixerController.MasterMixerGroup.SFX, 0.5f);
                gameObject.SetActive(false);
                Visible = false;
                

                NotifyListeners();
                UNMuteAudio();
                Destroy(gameObject);

            }
        }

        protected void NotifyListeners()
        {
            if (Visible)
            {
                if (OnDialogShown != null)
                    OnDialogShown(gameObject);
            }
            else
            {
                if (OnDialogHidden != null)
                    OnDialogHidden(gameObject);
            }
        }

        public Dialog AddShownHandler(DialogShownHandler dialogShownHandler)
        {
            OnDialogShown += dialogShownHandler;
            return this;
        }

        public Dialog RemoveShownHandler(DialogShownHandler dialogShownHandler)
        {
            OnDialogShown -= dialogShownHandler;
            return this;
        }

        public virtual Dialog AddHiddenHandler(DialogHiddenHandler dialogHiddenHandler)
        {
            OnDialogHidden += dialogHiddenHandler;
            return this;
        }

        public virtual Dialog RemoveHiddenHandler(DialogHiddenHandler dialogHiddenHandler)
        {
            OnDialogHidden -= dialogHiddenHandler;
            return this;
        }

        public Dialog DisableHideArea()
        {
            _hideDialogButtonArea.enabled = false;
            return this;
        }
    }
}