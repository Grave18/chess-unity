using System;
using UnityEngine;
using UnityEngine.Audio;
using UtilsCommon.Mathematics;

namespace GameAndScene
{
    [DefaultExecutionOrder(-1)]
    public class AudioSettingsContainer : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string EffectsVolumeKey = "EffectsVolume";

        private void Awake()
        {
            FirstTimeInitVolumes();
        }

        private void FirstTimeInitVolumes()
        {
            if(!PlayerPrefs.HasKey(MasterVolumeKey))
            {
                PlayerPrefs.SetFloat(MasterVolumeKey, 1f);
            }

            if(!PlayerPrefs.HasKey(MusicVolumeKey))
            {
                PlayerPrefs.SetFloat(MusicVolumeKey, 1f);
            }

            if(!PlayerPrefs.HasKey(EffectsVolumeKey))
            {
                PlayerPrefs.SetFloat(EffectsVolumeKey, 1f);
            }
        }

        private void Start()
        {
            InitAudiomixerVolumes();
        }

        private void InitAudiomixerVolumes()
        {
            SetMasterVolume(GetMasterVolume());
            SetMusicVolume(GetMusicVolume());
            SetEffectsVolume(GetEffectsVolume());
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