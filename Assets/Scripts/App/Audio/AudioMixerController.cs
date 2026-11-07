using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace App.Audio
{
    public class AudioMixerController : MonoBehaviour
    {
        [SerializeField] private AudioMixer _masterAudioMixer;
        public AudioMixerGroup _masterGroup;
        public AudioMixerGroup _masterSFxConstGroup;
        public AudioMixerGroup _masterSFxGroup;
        public AudioMixerGroup _masterMusicGroup;
        private float _slowMuteStartVolume;

        private void Start()
        {
            _slowMuteStartVolume = 0;
            SetMixerGroupVolume(MasterMixerGroup.MUSIC,PlayerPrefs.GetFloat("music_volume"));
            SetMixerGroupVolume(MasterMixerGroup.SFX,PlayerPrefs.GetFloat("sfx_volume"));
            SetMixerGroupVolume(MasterMixerGroup.SFXCONST,PlayerPrefs.GetFloat("sfx_volume"));
 
        }

        public void  SetMixerGroupVolume(MasterMixerGroup audioMixerGroup, float volume)
        {
            var parameterName = audioMixerGroup.ToString().ToLower() + "Lvl";
            _masterAudioMixer.SetFloat(parameterName, volume); 
            
        }
        
        
        public void  SetSlowMuteMixerGroup(MasterMixerGroup audioMixerGroup, float time)
        {
            var parameterName = audioMixerGroup.ToString().ToLower() + "Lvl";
            _masterAudioMixer.GetFloat(parameterName,out _slowMuteStartVolume);
            _masterAudioMixer.DOSetFloat(parameterName, -80, time);
        }
        
        public void  SetSlowLowpassCutoffFreq()
        {
            var parameterName = "lowpass";
            _masterAudioMixer.DOSetFloat(parameterName, 300, 3);
        }
        
        public void  UnetSlowLowpassCutoffFreq()
        {
            var parameterName = "lowpass";
            _masterAudioMixer.DOSetFloat(parameterName, 22000, 7);
        }
    
        public  void  SetSlowUnMuteMixerGroup(MasterMixerGroup audioMixerGroup, float time)
        {
            var parameterName = audioMixerGroup.ToString().ToLower() + "Lvl";
            float volume = 0;
            switch (audioMixerGroup)
            {
                case MasterMixerGroup.SFX:
                {
                    volume = PlayerPrefs.GetFloat("sfx_volume");
                    break;
                }
                case MasterMixerGroup.SFXCONST:
                {
                    volume = PlayerPrefs.GetFloat("sfx_volume");
                    break;
                }
                case MasterMixerGroup.MUSIC:
                {
                    volume = PlayerPrefs.GetFloat("music_volume");
                    break;
                }
                default: break;
            }
            _masterAudioMixer.DOSetFloat(parameterName, volume, time);
        }

        public  AudioMixerGroup  GetMixerGroup(MasterMixerGroup masterMixerGroup)
        {
            switch (masterMixerGroup)
            {
                case MasterMixerGroup.SFX : return _masterSFxGroup;
                case MasterMixerGroup.SFXCONST : return _masterSFxConstGroup;
                case MasterMixerGroup.MUSIC : return _masterMusicGroup;
                default: return _masterGroup;
            }
        }


    

        public enum MasterMixerGroup
        {
            NONE,
            MASTER,
            SFX,
            SFXCONST,
            MUSIC,
        }
    }
}
