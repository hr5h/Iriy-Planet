using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
    using Game.Sounds;

    /// <summary>
    /// Сохранение, загрузка и применение игровых настроек
    /// </summary>
    public class GameSettings : MonoBehaviour
    {
        private static GameSettings _instance;
        public static GameSettings Instance {
            get
            { 
                if (_instance != null)
                {
                    return _instance;
                }
                return new GameObject(nameof(GameSettings)).AddComponent<GameSettings>();
            }        
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static GameSettings OnApplicationStart()
        {
            return Instance;
        }
        private void Start()
        {
            //ResetSettings();
            LoadSettings();
        }
        #region PlayerPrefs processing
        private void LoadSettings()
        {
            SoundMixerManager.Instance.SetMasterVolumePrepared(PlayerPrefs.GetFloat("MasterVolume", -2f));
            SoundMixerManager.Instance.SetSoundFXVolumePrepared(PlayerPrefs.GetFloat("SoundFXVolume", -2f));
            SoundMixerManager.Instance.SetMusicVolumePrepared(PlayerPrefs.GetFloat("MusicVolume", -2f));
            SoundMixerManager.Instance.SetInterfaceVolumePrepared(PlayerPrefs.GetFloat("InterfaceVolume", -2f));
        }
        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", SoundMixerManager.MasterVolume);
            PlayerPrefs.SetFloat("SoundFXVolume", SoundMixerManager.EffectsVolume);
            PlayerPrefs.SetFloat("MusicVolume", SoundMixerManager.MusicVolume);
            PlayerPrefs.SetFloat("InterfaceVolume", SoundMixerManager.MusicVolume);
            PlayerPrefs.Save();
        }
        private void ResetSettings()
        {
            PlayerPrefs.DeleteAll();
            LoadSettings();
        }
        #endregion

        private void OnApplicationQuit()
        {
            SaveSettings();
        }

#if UNITY_EDITOR
        void OnDestroy()
        {
            OnApplicationQuit();
        }
#endif
    }
}