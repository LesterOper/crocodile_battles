using System;
using System.Collections.Generic;
using System.Linq;
using App;
using App.Audio;
using UnityEngine;

namespace Library.Common
{
    [DisallowMultipleComponent]
    public class SoundPlayer : MonoBehaviour
    {
        public const string PrefName = "SoundPlayer_enabled";
        private readonly List<AudioSource> _sources = new List<AudioSource>();

        [SerializeField] private AudioClip[] _clips;

        private bool _enabled;
        [SerializeField] private AudioMixerController _audioMixerController;

        private void Awake()
        {
            _enabled = PlayerPrefs.GetInt(PrefName, 1) == 1;
            for (var i = 0; i < 30; i++)
                _sources.Add(gameObject.AddComponent(typeof(AudioSource)) as AudioSource);
            DontDestroyOnLoad(this);
        }

        public AudioSource PlaySound(string soundName, AudioMixerController.MasterMixerGroup masterMixerGroup,
            float volume = 0.8f)
        {
            if (_enabled)
                try
                {
                    var source = GetSource();

                    if (source.isPlaying) source.Stop();
                    source.loop = false;
                    source.clip = _clips.FirstOrDefault(clip => clip.name.Equals(soundName));
                    source.outputAudioMixerGroup = _audioMixerController.GetMixerGroup(masterMixerGroup);
                    source.volume = volume;
                    source.spatialBlend = 0;
                    source.Play();
                    OnSourceUsed(source);
                    return source;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }

            return null;
        }

        public void StopLoops()
        {
            foreach (var source in _sources)
                if (source.loop)
                {
                    source.loop = false;
                    source.Stop();
                }
        }
        
        private AudioSource GetSource()
        {
            foreach (var source in _sources)
                if (!source.isPlaying)
                    if (!source.loop)
                        return source;
            return _sources.Last();
        }

        private void OnSourceUsed(AudioSource source)
        {
            if (_sources[0] != source)
            {
                _sources.Remove(source);
                _sources.Insert(0, source);
            }
        }

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;
            PlayerPrefs.SetInt(PrefName, enabled ? 1 : 0);
            PlayerPrefs.Save();

            if (!enabled)
                StopPlayingSounds();
        }

        public bool IsEnabled()
        {
            return _enabled;
        }

        public void StopPlayingSounds()
        {
            foreach (var source in _sources)
                if (source.isPlaying)
                    source.Stop();
        }
    }
}