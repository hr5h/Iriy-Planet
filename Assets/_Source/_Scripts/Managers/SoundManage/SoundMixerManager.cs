using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Sounds
{
    using System.Linq;
    using UnityEngine.Audio;
    using UnityEngine.Events;

    /// <summary>
    /// Управление микшером громкости
    /// </summary>
    public class SoundMixerManager : MonoBehaviour
    {
        public static float MasterVolume { get; private set; }
        public static float EffectsVolume { get; private set; }
        public static float MusicVolume { get; private set; }
        public static float InterfaceVolume { get; private set; }

        private static SoundMixerManager _instance;
        public static SoundMixerManager Instance
        {
            get
            {
                if (_instance != null) 
                {
                    return _instance;
                }
                return new GameObject(nameof(SoundMixerManager)).AddComponent<SoundMixerManager>();
            }
        }
        [SerializeField] private AudioMixer _mixer;
        public AudioMixer Mixer { get => _mixer; private set => _mixer = value; }
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _mixer = Resources.Load("AudioMixer") as AudioMixer;
            DontDestroyOnLoad(gameObject);
        }
        public AudioMixerGroup GetMixerGroup(string groupname)
        {
            return _mixer.FindMatchingGroups(groupname).FirstOrDefault();
        }
        public static float GetLevelFromDecibels(float dB)
        {
            return Mathf.Pow(10f, dB / 20f);
        }
        public static float GetDecibelsFromLevel(float level)
        {
            return Mathf.Log10(level) * 20f;
        }

        #region Set values
        public void SetMasterVolume(float level)
        {
            SetMasterVolumePrepared(GetDecibelsFromLevel(level));
        }
        public void SetSoundFXVolume(float level)
        {
            SetSoundFXVolumePrepared(GetDecibelsFromLevel(level));
        }
        public void SetMusicVolume(float level)
        {
            SetMusicVolumePrepared(GetDecibelsFromLevel(level));
        }
        public void SetInterfaceVolume(float level)
        {
            SetInterfaceVolumePrepared(GetDecibelsFromLevel(level));
        }
        public void SetMasterVolumePrepared(float dB)
        {
            _mixer.SetFloat("MasterVolume", dB);
            MasterVolume = dB;
        }
        public void SetSoundFXVolumePrepared(float dB)
        {
            _mixer.SetFloat("EffectsVolume", dB);
            EffectsVolume = dB;
        }
        public void SetMusicVolumePrepared(float dB)
        {
            _mixer.SetFloat("MusicVolume", dB);
            MusicVolume = dB;
        }
        public void SetInterfaceVolumePrepared(float dB)
        {
            _mixer.SetFloat("InterfaceVolume", dB);
            InterfaceVolume = dB;
        }
        #endregion

        #region Get values
        public float GetMasterVolume()
        {
            _mixer.GetFloat("MasterVolume", out var level);
            return level;
        }
        public float GetSoundFXVolume()
        {
            _mixer.GetFloat("EffectsVolume", out var level);
            return level;
        }
        public float GetMusicVolume()
        {
            _mixer.GetFloat("MusicVolume", out var level);
            return level;
        }
        public float GetInterfaceVolume()
        {
            _mixer.GetFloat("InterfaceVolume", out var level);
            return level;
        }
        #endregion
    }
}