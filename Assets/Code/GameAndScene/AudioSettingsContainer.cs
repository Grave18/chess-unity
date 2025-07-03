using System;
using UnityEngine;
using UnityEngine.Audio;
using Utils.Mathematics;

namespace GameAndScene
{
    public class AudioSettingsContainer : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string EffectsVolumeKey = "EffectsVolume";

        private void Start()
        {
            InitializeAudiomixerVolume();
        }

        private void InitializeAudiomixerVolume()
        {
            float masterVolume = PlayerPrefs.HasKey(MasterVolumeKey) ? GetMasterVolume() : 1f;
            SetMasterVolume(masterVolume);

            float musicVolume = PlayerPrefs.HasKey(MusicVolumeKey) ? GetMusicVolume() : 1f;
            SetMusicVolume(musicVolume);

            float effectsVolume = PlayerPrefs.HasKey(EffectsVolumeKey) ? GetEffectsVolume() : 1f;
            SetEffectsVolume(effectsVolume);
        }

        public float GetMasterVolume()
        {
            return PlayerPrefs.GetFloat(MasterVolumeKey);
        }

        public void SetMasterVolume(float value)
        {
            SetVolume(value, MasterVolumeKey);
        }

        public float GetMusicVolume()
        {
            return PlayerPrefs.GetFloat(MusicVolumeKey);
        }

        public void SetMusicVolume(float value)
        {
            SetVolume(value, MusicVolumeKey);
        }

        public float GetEffectsVolume()
        {
            return PlayerPrefs.GetFloat(EffectsVolumeKey);
        }

        public void SetEffectsVolume(float value)
        {
            SetVolume(value, EffectsVolumeKey);
        }

        private void SetVolume(float value, string volumeKey)
        {
            float mixerValue = MapToMixerVolume(value, Easing.OutCubic);
            audioMixer.SetFloat(volumeKey, mixerValue);

            PlayerPrefs.SetFloat(volumeKey, value);
        }

        private static float MapToMixerVolume(float t, Func<float, float> easing)
        {
            return -80f + easing(t) * 80f;
        }
    }
}