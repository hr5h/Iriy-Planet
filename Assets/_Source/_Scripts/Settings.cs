using Game.Sounds;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Slider masterSlider;
    public Slider MusicSlider;
    public Slider EffectSlider;
    public Slider InterfaceSlider;

    public AudioClip buttonClip;

    private Canvas _canvas;

    public void Start()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (gameObject.activeInHierarchy && SceneManager.GetActiveScene().name == "Menu")
                Close();
    }

    public void ChangeVolumeMaster(float volume)
    {
        SoundMixerManager.Instance.SetMasterVolume(volume);
    }

    public void ChangeVolumeMusic(float volume)
    {
        SoundMixerManager.Instance.SetMusicVolume(volume);
    }

    public void ChangeVolumeEffects(float volume)
    {
        SoundMixerManager.Instance.SetSoundFXVolume(volume);
    }

    public void ChangeVolumeInterface(float volume)
    {
        SoundMixerManager.Instance.SetInterfaceVolume(volume);
    }

    public void Open()
    {
        AudioPlayer.Instance.PlayUI(buttonClip);
        _canvas.enabled = true;
        masterSlider.value = SoundMixerManager.GetLevelFromDecibels(SoundMixerManager.Instance.GetMasterVolume());
        MusicSlider.value = SoundMixerManager.GetLevelFromDecibels(SoundMixerManager.Instance.GetMusicVolume());
        EffectSlider.value = SoundMixerManager.GetLevelFromDecibels(SoundMixerManager.Instance.GetSoundFXVolume());
        InterfaceSlider.value = SoundMixerManager.GetLevelFromDecibels(SoundMixerManager.Instance.GetInterfaceVolume());
    }

    public void Close()
    {
        AudioPlayer.Instance.PlayUI(buttonClip);
        _canvas.enabled = false;
    }

}
