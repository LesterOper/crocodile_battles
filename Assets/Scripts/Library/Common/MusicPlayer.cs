using System;
using System.Collections.Generic;
using System.Linq;
using App.Audio;
using UnityEngine;

namespace Library.Common
{
    [DisallowMultipleComponent]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioMixerController _audioMixerController;
        public enum Channel
        {
            Music = 0,
            Ambient = 1,
            Battle = 2,
        }

        public const string PrefName = "MusicPlayer_enabled";
        private readonly Dictionary<Channel, ChannelData> _chanelDatas = new Dictionary<Channel, ChannelData>();

        private readonly Dictionary<Channel, AudioSource> _sources = new Dictionary<Channel, AudioSource>();

        [SerializeField] private AudioClip[] _audioClips;

        private bool _enabled;

        private void Awake()
        {
            _enabled = PlayerPrefs.GetInt(PrefName, 1) == 1;

            foreach (var channel in Enum.GetValues(typeof(Channel)).Cast<Channel>())
                _sources[channel] = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            DontDestroyOnLoad(this);
        }

        public AudioSource GetAudioSource(Channel channel) => _sources[channel];
        
        
        public void PlayMusic(string musicName, AudioMixerController.MasterMixerGroup masterMixerGroup, Channel channel, float volume = 0.25f)
        {
            var chanelData = new ChannelData
            {
                MusicName = musicName,
                Volume = volume
            };


            if (_enabled &&
                (!_chanelDatas.ContainsKey(channel) ||
                 !musicName.Equals(_chanelDatas[channel].MusicName) ||
                 Math.Abs(_chanelDatas[channel].Volume - volume) > 0.0001f))
                PlayMusic(channel, chanelData, masterMixerGroup);

            _chanelDatas[channel] = chanelData;
        }

        private void PlayMusic(Channel channel, ChannelData channelData, AudioMixerController.MasterMixerGroup masterMixerGroup)
        {
            try
            {
                StopChannel(channel);
                _sources[channel].clip =
                    _audioClips.FirstOrDefault(audioClip => audioClip.name.Equals(channelData.MusicName));
                _sources[channel].outputAudioMixerGroup = _audioMixerController.GetMixerGroup(masterMixerGroup);
                _sources[channel].loop = true;
                _sources[channel].volume = Mathf.Clamp01(PlayerPrefs.GetInt(CodeBase.Constants.SOUND, 1) == 1 ? channelData.Volume : 0);
                _sources[channel].Play();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void StopMusic()
        {
            foreach (var source in _sources.Values)
                if (source.isPlaying)
                    source.Stop();
        }

        public void StopChannel(Channel channel)
        {
            if (_sources[channel].isPlaying)
                _sources[channel].Stop();

            _chanelDatas.Remove(channel);
        }

        public void SetVolume(bool isActive)
        {
            
        }

        public bool IsEnabled()
        {
            return _enabled;
        }

        private class ChannelData
        {
            public string MusicName { get; set; }

            public float Volume { get; set; }
        }
    }
}