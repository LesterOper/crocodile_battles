using App;
using App.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Library.Common
{
    public class ButtonClickSound : MonoBehaviour
    {
        private Button _button;
        private Toggle _toggle;
        private SoundPlayer _soundPlayer;

        [Inject]
        public void Construct(SoundPlayer soundPlayer)
        {
            _soundPlayer = soundPlayer;
        }
        private void OnEnable()
        {
            if (_button != null)
                _button.onClick.AddListener(PlayClick);
            else _toggle.onValueChanged.AddListener(PlayClickOnToggle);
        }

        private void OnDisable()
        {
            if(_button != null)
                _button.onClick.RemoveListener(PlayClick);
            else _toggle.onValueChanged.RemoveListener(PlayClickOnToggle);
        }

        private void Awake()
        {
            if (TryGetComponent(out _button))
            {

            }
            else _toggle = GetComponent<Toggle>();
        }

        private void PlayClick()
        {
            _soundPlayer.PlaySound("SFX_ButtonClickBase", AudioMixerController.MasterMixerGroup.SFX, 0.1f);
        }

        private void PlayClickOnToggle(bool value)
        {
            _soundPlayer.PlaySound("SFX_ButtonClickBase", AudioMixerController.MasterMixerGroup.SFX, 0.1f);
        }
    }
}