using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

namespace Library.Dialogs
{
    public class ThreeButtonDialog : Dialog
    {
        public delegate void NegativeHandler();

        public delegate void NeutralHandler();

        public delegate void PositiveHandler();

        [SerializeField] public Button _negativeButton;
        [SerializeField] public Button _neutralButton;

        [SerializeField] public Button _positiveButton;
        [SerializeField] private Localize _txtMessage;
        [SerializeField] private Localize _txtTitle;

        private NegativeHandler OnNegative;
        private NeutralHandler OnNeutral;
        private PositiveHandler OnPositive;

        protected Localize TxtMessage => _txtMessage;
        protected Localize TxtTitle => _txtTitle;

        //[SerializeField] private AudioSource _sfx;

        private void OnEnable()
        {
            //Messenger.AddListener(MessengerKeys.BRIBE_SOUND, BribeSound);
        }

        private void OnDisable()
        {
            //Messenger.RemoveListener(MessengerKeys.BRIBE_SOUND, BribeSound);
        }

        protected override void Start()
        {
            base.Start();

            //_sfx.Play();
            _positiveButton.onClick.AddListener(OnPositiveClicked);
            _neutralButton.onClick.AddListener(OnNeutralClicked);
            _negativeButton.onClick.AddListener(OnNegativeClicked);
        }

        public ThreeButtonDialog SetTitle(string textKey)
        {
            _txtTitle.SetTerm(textKey);
            return this;
        }

        public ThreeButtonDialog SetTitle(string textKey, params object[] args)
        {
            // _txtTitle.SetTerm(textKey, args);
            return this;
        }

        public ThreeButtonDialog SetMessage(string textKey)
        {
            _txtMessage.SetTerm(textKey);
            return this;
        }

        public ThreeButtonDialog SetMessage(string textKey, params object[] args)
        {
            // _txtMessage.SetTerm(textKey, args);
            return this;
        }

        public ThreeButtonDialog SetMessage(string delimiter, string[] textKeys)
        {
            // _txtMessage.
            return this;
        }

        public ThreeButtonDialog SetMessage(string delimiter, params Tuple<string, object[]>[] textData)
        {
            // _txtMessage.SetTerm(delimiter, textData);
            return this;
        }

        public ThreeButtonDialog SetPositiveEnabled(bool buttonEnabled)
        {
            _positiveButton.gameObject.SetActive(buttonEnabled);
            return this;
        }

        public ThreeButtonDialog AddPositiveHandler(PositiveHandler positiveHandler)
        {
            OnPositive += positiveHandler;
            return this;
        }

        public ThreeButtonDialog RemovePositiveHandler(PositiveHandler positiveHandler)
        {
            OnPositive -= positiveHandler;
            return this;
        }

        public ThreeButtonDialog SetNeutralEnabled(bool buttonEnabled)
        {
            _neutralButton.gameObject.SetActive(buttonEnabled);
            return this;
        }

        public ThreeButtonDialog AddNeutralHandler(NeutralHandler neutralHandler)
        {
            OnNeutral += neutralHandler;
            return this;
        }

        public ThreeButtonDialog RemoveNeutralHandler(NeutralHandler neutralHandler)
        {
            OnNeutral -= neutralHandler;
            return this;
        }

        public ThreeButtonDialog RemoveHideArea()
        {
            DisableHideArea();
            return this;
        }

        public ThreeButtonDialog SetNegativeEnabled(bool buttonEnabled)
        {
            _negativeButton.gameObject.SetActive(buttonEnabled);
            return this;
        }

        public ThreeButtonDialog AddNegativeHandler(NegativeHandler negativeHandler)
        {
            OnNegative += negativeHandler;
            return this;
        }

        public ThreeButtonDialog RemoveNegativeHandler(NegativeHandler negativeHandler)
        {
            OnNegative -= negativeHandler;
            return this;
        }

        public virtual void OnPositiveClicked()
        {
            //Context.SoundPlayer.PlaySound("click", AudioMixerController.MasterMixerGroup.SFXCONST, 0.8f);
            if (OnPositive != null)
                OnPositive();
            Hide();
        }

        public void OnNeutralClicked()
        {
            //Context.SoundPlayer.PlaySound("click", AudioMixerController.MasterMixerGroup.SFXCONST, 0.8f);
            if (OnNeutral != null)
                OnNeutral();
            Hide();
        }

        public void OnNegativeClicked()
        {
            //Context.SoundPlayer.PlaySound("click", AudioMixerController.MasterMixerGroup.SFXCONST, 0.8f);
            if (OnNegative != null)
                OnNegative();
            Hide();
        }

        private void BribeSound()
        {
            //if (SceneManager.GetActiveScene().name.Equals("bank"))
               // Context.SoundPlayer.PlaySound("podkup_sotrudnika", AudioMixerController.MasterMixerGroup.SFXCONST, 1f);
        }
    }
}