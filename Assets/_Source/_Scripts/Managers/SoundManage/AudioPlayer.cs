using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Sounds
{
    /// <summary>
    /// Воспроизведение музыки и звуковых эффектов
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        #region Constants
        public const int InitialPoolSize = 5;
        public const float MaxSoundDistance = 800;
        public const float MinPitchDefault = 0.9f;
        public const float MaxPitchDefault = 1.1f;
        #endregion

        private static AudioPlayer _instance;
        public static AudioPlayer Instance {
            get
            {
                if (_instance != null) 
                { 
                    return _instance;
                }
                return new GameObject(nameof(AudioPlayer)).AddComponent<AudioPlayer>();
            }
        }
        private AudioListener _listener;
        private AudioSource _audioSourceObject;
        private Queue<AudioSource> _audioSourcesPool = new Queue<AudioSource>();

        #region Sources
        private AudioSource _uiAudioSource;
        private AudioSource _worldFXSource;
        private AudioSource _worldLoop;
        private AudioSource _backgroundMusicSource;
        private AudioSource _dynamicMusicPrimarySource;
        private AudioSource _dynamicMusicSecondarySource;
        #endregion

        #region Coroutines
        private Coroutine _crossfadeCoroutine;
        private Coroutine _dynamicWaitCoroutine;
        private bool _backgroundFading;
        private bool _crossfadeCoroutineRunning;
        private bool _dynamicChangeCoroutineRunning;
        private bool _dynamicWaitCoroutineRunning;
        #endregion

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _audioSourceObject = (Resources.Load("AudioSourceObject") as GameObject).GetComponent<AudioSource>();
            _listener = FindObjectOfType<AudioListener>();
            InitializeAudioSourcePool(InitialPoolSize);
            SceneManager.activeSceneChanged += OnSceneChanged;

            InitializeAudioSourcesOnAwake();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnAudioListenerChanged.AddListener(ChangeAudioListener);
            }
        }

        private void OnSceneChanged(Scene prev, Scene curr)
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnAudioListenerChanged.AddListener(ChangeAudioListener);
            }
            StopBackgroundMusic();

            AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
            for (int i = 0; i < audioListeners.Length; ++i)
            {
                if (audioListeners[i].enabled == true)
                {
                    ChangeAudioListener(audioListeners[i]);
                    break;
                }
            }
            
            InitializeAudioSourcePool(InitialPoolSize);
            StopAllCoroutines();
            StopAllSources();
        }

        private void ChangeAudioListener(AudioListener listener)
        {
            _listener = listener;
        }

        private void StopAllSources()
        {
            _dynamicMusicPrimarySource.Stop();
            _dynamicMusicSecondarySource.Stop();
            _worldFXSource.Stop();
            _worldLoop.Stop();
        }

        public void PauseAllSources()
        {
            _dynamicMusicPrimarySource.Pause();
            _dynamicMusicSecondarySource.Pause();
            _worldFXSource.Pause();
            _worldLoop.Pause();
        }

        public void UnPauseAllSources()
        {
            _dynamicMusicPrimarySource.UnPause();
            _dynamicMusicSecondarySource.UnPause();
            _worldFXSource.UnPause();
            _worldLoop.UnPause();
        }

        #region Initialization
        private void InitializeAudioSourcesOnAwake()
        {
            InitializeBackgroundMusicSource();
            InializeUISource();
            InitializeDynamicMusicSources();
            InitializeWorldFXSource();
            InitializeWorldLoopSource();
        }
        
        private void MakeSource2D(AudioSource source)
        {
            source.dopplerLevel = 0f;
            source.reverbZoneMix = 0f;
            source.spatialBlend = 0f;
            source.playOnAwake = false;
        }
        private void InitializeWorldLoopSource()
        {
            _worldLoop = gameObject.AddComponent<AudioSource>();
            _worldLoop.outputAudioMixerGroup = SoundMixerManager.Instance.GetMixerGroup("Effects");
            MakeSource2D(_worldLoop);
            _worldLoop.loop = false;
        }
        private void InitializeWorldFXSource()
        {
            _worldFXSource = gameObject.AddComponent<AudioSource>();
            _worldFXSource.outputAudioMixerGroup = SoundMixerManager.Instance.GetMixerGroup("Effects");
            MakeSource2D(_worldFXSource);
            _worldFXSource.loop = false;
        }
        private void InitializeBackgroundMusicSource()
        {
            _backgroundMusicSource = gameObject.AddComponent<AudioSource>();
            _backgroundMusicSource.outputAudioMixerGroup = SoundMixerManager.Instance.GetMixerGroup("Music");
            MakeSource2D(_backgroundMusicSource);
            _backgroundMusicSource.loop = true;
        }
        private void InializeUISource()
        {
            _uiAudioSource = gameObject.AddComponent<AudioSource>();
            _uiAudioSource.outputAudioMixerGroup = SoundMixerManager.Instance.GetMixerGroup("Interface");
            MakeSource2D(_uiAudioSource);
            _uiAudioSource.loop = false;
        }
        private void InitializeDynamicMusicSources()
        {
            _dynamicMusicPrimarySource = gameObject.AddComponent<AudioSource>();
            _dynamicMusicPrimarySource.outputAudioMixerGroup = SoundMixerManager.Instance.GetMixerGroup("Music");
            MakeSource2D(_dynamicMusicPrimarySource);
            _dynamicMusicPrimarySource.loop = false;

            _dynamicMusicSecondarySource = gameObject.AddComponent<AudioSource>();
            _dynamicMusicSecondarySource.outputAudioMixerGroup = SoundMixerManager.Instance.GetMixerGroup("Music");
            MakeSource2D(_dynamicMusicSecondarySource);
            _dynamicMusicSecondarySource.loop = false;
        }
        #endregion

        #region WorldFX
        public void PlayWorldFX(AudioClip clip, float volume = 1)
        {
            _worldFXSource.clip = clip;
            _worldFXSource.volume = volume;
            _worldFXSource.Play();
        }
        public void StopWorldFX(AudioClip clip)
        {
            if (_worldFXSource.clip == clip)
            {
                _worldFXSource.Stop();
            }
        }
        #endregion

        #region WorldLoop
        public void PlayWorldLoop(AudioClip clip, float volume = 1)
        {
            _worldLoop.clip = clip;
            _worldLoop.volume = volume;
            _worldLoop.Play();
        }
        public void StopWorldLoop(AudioClip clip)
        {
            if (_worldLoop.clip == clip)
            {
                _worldLoop.Stop();
            }
        }
        public void PauseWorldLoop(AudioClip clip)
        {
            if (_worldLoop.clip == clip)
            {
                _worldLoop.Pause();
            }
        }
        public void UnPauseWorldLoop(AudioClip clip)
        {
            if (_worldLoop.clip == clip)
            {
                _worldLoop.UnPause();
            }
        }
        #endregion

        #region AudioSource pooling
        private void InitializeAudioSourcePool(int poolSize)
        {
            _audioSourcesPool.Clear(); // Так как AudioPlayer не уничтожается между сценами, нужно очищать контейнер вручную

            for (int i = 0; i < poolSize; i++)
            {
                AudioSource audioSource = Instantiate(_audioSourceObject, Vector2.zero, Quaternion.identity);
                audioSource.gameObject.SetActive(false);
                _audioSourcesPool.Enqueue(audioSource);
            }
        }
        private AudioSource CreateAudioSource(Vector2 position)
        {
            if (_audioSourcesPool.Count > 0)
            {
                AudioSource audioSource = _audioSourcesPool.Dequeue();
                audioSource.transform.position = position;
                audioSource.gameObject.SetActive(true);
                return audioSource;
            }
            return Instantiate(_audioSourceObject, position, Quaternion.identity);
        }
        private void ReturnAudioSourceToPool(AudioSource audioSource)
        {
            audioSource.gameObject.SetActive(false);
            _audioSourcesPool.Enqueue(audioSource);
        }
        private IEnumerator DestroyAudioSourceAfterDelay(AudioSource audioSource, float delay)
        {
            yield return Yielders.Get(delay);
            ReturnAudioSourceToPool(audioSource);
        }
        #endregion

        #region SoundFX on object
        public void PlaySoundFX(AudioClip audioClip, AudioSource audioSource)
        {
            if (Vector2.Distance(audioSource.transform.position, _listener.transform.position) <= audioSource.maxDistance)
            {
                audioSource.PlayOneShot(audioClip, audioSource.volume);
            }
        }
        public void PlaySoundFX(AudioClip audioClip, AudioSource audioSource, float volume)
        {
            if (Vector2.Distance(audioSource.transform.position, _listener.transform.position) <= audioSource.maxDistance)
            {
                audioSource.PlayOneShot(audioClip, volume);
            }
        }
        public void PlaySoundFX(AudioClip[] audioClips, AudioSource audioSource, float volume)
        {
            int rand = Random.Range(0, audioClips.Length);
            PlaySoundFX(audioClips[rand], audioSource, volume);
        }
        public void PlaySoundFX(AudioClip[] audioClips, AudioSource audioSource)
        {
            int rand = Random.Range(0, audioClips.Length);
            PlaySoundFX(audioClips[rand], audioSource, audioSource.volume);
        }

        #region Random pitch
        public void PlaySoundFXWithRandomPitch(AudioClip audioClip, AudioSource audioSource, float volume, float minPitch = MinPitchDefault, float maxPitch = MaxPitchDefault, float distance = MaxSoundDistance)
        {
            if (Vector2.Distance(audioSource.transform.position, _listener.transform.position) <= distance)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.PlayOneShot(audioClip, volume);
            }
        }
        public void PlaySoundFXWithRandomPitch(AudioClip audioClip, AudioSource audioSource, float minPitch = MinPitchDefault, float maxPitch = MaxPitchDefault, float distance = MaxSoundDistance)
        {
            if (Vector2.Distance(audioSource.transform.position, _listener.transform.position) <= distance)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.PlayOneShot(audioClip, audioSource.volume);
            }
        }
        public void PlaySoundFXWithRandomPitch(AudioClip[] audioClips, AudioSource audioSource, float minPitch = MinPitchDefault, float maxPitch = MaxPitchDefault, float distance = MaxSoundDistance)
        {
            int rand = Random.Range(0, audioClips.Length);
            PlaySoundFXWithRandomPitch(audioClips[rand], audioSource, audioSource.volume, minPitch, maxPitch, distance);
        }
        public void PlaySoundFXWithRandomPitch(AudioClip[] audioClips, AudioSource audioSource, float volume, float minPitch = MinPitchDefault, float maxPitch = MaxPitchDefault, float distance = MaxSoundDistance)
        {
            int rand = Random.Range(0, audioClips.Length);
            PlaySoundFXWithRandomPitch(audioClips[rand], audioSource, volume, minPitch, maxPitch, distance);
        }
        public void PlaySoundFXWithRandomPitch(AudioSource source, float minPitch = MinPitchDefault, float maxPitch = MaxPitchDefault, float distance = MaxSoundDistance)
        {
            if (Vector2.Distance(source.transform.position, _listener.transform.position) <= distance)
            {
                source.pitch = Random.Range(minPitch, maxPitch);
                source.Play();
            }
        }
        public void PlaySoundFXWithRandomPitch(AudioSource source, float volume, float minPitch = MinPitchDefault, float maxPitch = MaxPitchDefault, float distance = MaxSoundDistance)
        {
            if (Vector2.Distance(source.transform.position, _listener.transform.position) <= distance)
            {
                source.volume = volume;
                source.pitch = Random.Range(minPitch, maxPitch);
                source.Play();
            }
        }
        #endregion

        #endregion

        #region SoundFX on position
        public void PlaySoundFX(AudioClip audioClip, Vector2 spawnPos, float volume = 1f, float distance = MaxSoundDistance)
        {
            if (Vector2.Distance(spawnPos, _listener.transform.position) <= distance)
            {
                AudioSource audioSource = CreateAudioSource(spawnPos);
                audioSource.clip = audioClip;
                audioSource.volume = volume;
                audioSource.Play();
                float clipLength = audioSource.clip.length;
                StartCoroutine(DestroyAudioSourceAfterDelay(audioSource, clipLength));
            }
        }
        public void PlaySoundFX(AudioClip[] audioClips, Vector2 spawnPos, float volume = 1f, float distance = MaxSoundDistance)
        {
            int rand = Random.Range(0, audioClips.Length);
            PlaySoundFX(audioClips[rand], spawnPos, volume, distance);
        }
        public void PlaySoundFX(AudioClip audioClip, float volume = 1f, float distance = MaxSoundDistance)
        {
            PlaySoundFX(audioClip, _listener.transform.position, volume, distance);
        }
        public void PlaySoundFX(AudioClip[] audioClips, float volume = 1f, float distance = MaxSoundDistance)
        {
            PlaySoundFX(audioClips, _listener.transform.position, volume, distance);
        }

        #region Random pitch
        public void PlaySoundFXWithRandomPitch(AudioClip audioClip, Vector2 spawnPos, float volume = 1f, float minPitch = MinPitchDefault, float maxPitch = MaxPitchDefault, float distance = MaxSoundDistance)
        {
            if (Vector2.Distance(spawnPos, _listener.transform.position) <= distance)
            {
                AudioSource audioSource = CreateAudioSource(spawnPos);
                audioSource.clip = audioClip;
                audioSource.volume = volume;
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.Play();
                float clipLength = audioSource.clip.length;
                StartCoroutine(DestroyAudioSourceAfterDelay(audioSource, clipLength));
            }
        }
        public void PlaySoundFXWithRandomPitch(AudioClip[] audioClips, Vector2 spawnPos, float volume = 1f, float minPitch = MinPitchDefault, float maxPitch = MaxPitchDefault, float distance = MaxSoundDistance)
        {
            int rand = Random.Range(0, audioClips.Length);
            PlaySoundFXWithRandomPitch(audioClips[rand], spawnPos, volume, minPitch, maxPitch, distance);
        }
        public void PlaySoundFXWithRandomPitch(AudioClip audioClip, float volume = 1f, float minPitch = MinPitchDefault, float maxPitch = MaxPitchDefault, float distance = MaxSoundDistance)
        {
            PlaySoundFXWithRandomPitch(audioClip, _listener.transform.position, volume, minPitch, maxPitch, distance);
        }
        public void PlaySoundFXWithRandomPitch(AudioClip[] audioClips, float volume = 1f, float minPitch = MinPitchDefault, float maxPitch = MaxPitchDefault, float distance = MaxSoundDistance)
        {
            int rand = Random.Range(0, audioClips.Length);
            PlaySoundFXWithRandomPitch(audioClips[rand], _listener.transform.position, volume, minPitch, maxPitch, distance);
        }
        #endregion

        #endregion

        #region UI sound
        public void PlayUI(AudioClip audioClip, float volume = 1f)
        {
            _uiAudioSource.PlayOneShot(audioClip, volume);
        }
        #endregion

        #region Background music 
        public void PlayBackgroundMusic(AudioClip musicClip = null, float volume = 1f)
        {
            if (musicClip != null)
            {
                _backgroundMusicSource.clip = musicClip;
            }
            _backgroundMusicSource.volume = volume;
            _backgroundMusicSource.Play();
        }
        public void StopBackgroundMusic()
        {
            _backgroundMusicSource.Stop();
        }
        public void PauseBackgroundMusic()
        {
            _backgroundMusicSource.Pause();
        }
        public void UnPauseBackgroundMusic()
        {
            _backgroundMusicSource.UnPause();
        }
        #endregion

        #region Dynamic music
        public void PlayDynamicMusic(AudioClip audioClip, float volume = 1f, float duration = -1, float fadeIn = 0f, float fadeOut = 0f)
        {
            duration = duration > 0 ? duration : audioClip.length;
            fadeIn = fadeIn > 0f ? fadeIn : 0f;
            fadeOut = fadeOut > 0f ? fadeOut : 0f;
            volume = Mathf.Clamp(volume, 0f, 1f);

            if (!_crossfadeCoroutineRunning && !_dynamicChangeCoroutineRunning)
            {
                _dynamicMusicPrimarySource.clip = audioClip;
                _dynamicMusicPrimarySource.volume = volume;
                _crossfadeCoroutine = StartCoroutine(MusicCrossfadeCoroutine(duration, fadeIn, fadeOut));
            }
            else
            {
                if (!_dynamicChangeCoroutineRunning)
                {
                    _dynamicMusicSecondarySource.clip = audioClip;
                    _dynamicMusicSecondarySource.volume = volume;
                    StartCoroutine(DynamicMusicChangeCoroutine(duration, fadeIn, fadeOut));
                }
                else
                {
                    if (_dynamicWaitCoroutineRunning)
                    {
                        StopCoroutine(_dynamicWaitCoroutine);
                        _dynamicWaitCoroutineRunning = false;
                    }
                    StartCoroutine(DynamicMusicWaitCoroutine(audioClip, volume, duration, fadeIn, fadeOut));
                }
            }
        }

        public void PauseDynamicMusic(AudioClip audioClip)
        {
            if (_dynamicMusicPrimarySource.clip == audioClip)
            {
                //TODO PauseDynamicMusic(AudioClip audioClip)
            }
        }
        public void ResumeDynamicMusic(AudioClip audioClip)
        {
            if (_dynamicMusicPrimarySource.clip == audioClip)
            {
                //TODO ResumeDynamicMusic(AudioClip audioClip)
            }
        }
        public void StopDynamicMusic(AudioClip audioClip)
        {
            if (_dynamicMusicPrimarySource.clip == audioClip && _dynamicMusicPrimarySource.isPlaying) //Проверка нужна, чтобы не затрагивать источник, если уже играет другой клип 
            {
                StopCoroutine(_crossfadeCoroutine);
                _crossfadeCoroutine = StartCoroutine(MusicCrossfadeCoroutine(0,0,2,true));
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator MusicCrossfadeCoroutine(float interval, float fadeIn, float fadeOut, bool skipFadeIn = false)
        {
            _crossfadeCoroutineRunning = true;
            var startBackgroundVolume = _backgroundMusicSource.volume;
            var startDynamicVolume = _dynamicMusicPrimarySource.volume;

            if (!skipFadeIn)
            {
                _dynamicMusicPrimarySource.Play();
                if (fadeIn > 0f)
                {
                    _backgroundFading = true;
                    _dynamicMusicPrimarySource.volume = 0;
                    var dynamicCrossfadeFactor = startDynamicVolume / fadeIn;
                    var backgroundCrossfadeFactor = startBackgroundVolume / fadeIn;
                    while (fadeIn > 0f)
                    {
                        _backgroundMusicSource.volume -= backgroundCrossfadeFactor * Time.deltaTime;
                        _dynamicMusicPrimarySource.volume += dynamicCrossfadeFactor * Time.deltaTime;
                        fadeIn -= Time.deltaTime;
                        yield return null;
                    }
                    _backgroundMusicSource.volume = _backgroundMusicSource.volume = startBackgroundVolume;
                    _dynamicMusicPrimarySource.volume = startDynamicVolume;
                    _backgroundFading = false;
                }
                _backgroundMusicSource.Pause();
            }
            interval -= fadeIn + fadeOut;
            while (interval > 0f)
            {
                interval -= Time.deltaTime;
                yield return null;
            }
            _backgroundMusicSource.UnPause();
            if (fadeOut > 0f)
            {
                _backgroundFading = true;
                _backgroundMusicSource.volume = 0;
                var dynamicCrossfadeFactor = startDynamicVolume / fadeOut;
                var backgroundCrossfadeFactor = startBackgroundVolume / fadeOut;
                while (fadeOut > 0f)
                {
                    _backgroundMusicSource.volume += backgroundCrossfadeFactor * Time.deltaTime;
                    _dynamicMusicPrimarySource.volume -= dynamicCrossfadeFactor * Time.deltaTime;
                    fadeOut -= Time.deltaTime;
                    yield return null;
                }
                _dynamicMusicPrimarySource.volume = 0;
                _backgroundFading = false;
                _backgroundMusicSource.volume = startBackgroundVolume;
            }
            _crossfadeCoroutineRunning = false;
        }
        private IEnumerator DynamicMusicChangeCoroutine(float interval, float fadeIn, float fadeOut)
        {
            _dynamicChangeCoroutineRunning = true;
            while (_backgroundFading)
            {
                yield return null;
            }
            if (!_crossfadeCoroutineRunning)
            {
                _crossfadeCoroutine = StartCoroutine(MusicCrossfadeCoroutine(interval, fadeIn, fadeOut));
                _dynamicChangeCoroutineRunning = false;
                yield break;
            }
            StopCoroutine(_crossfadeCoroutine);
            _crossfadeCoroutineRunning = false;

            var startPrimaryVolume = _dynamicMusicPrimarySource.volume;
            var startSecondaryVolume = _dynamicMusicSecondarySource.volume;
            _dynamicMusicSecondarySource.Play();
            if (fadeIn > 0f)
            {
                _dynamicMusicSecondarySource.volume = 0;
                var primaryCrossfadeFactor = startPrimaryVolume / fadeIn;
                var secondaryCrossfadeFactor = startSecondaryVolume / fadeIn;
                while (fadeIn > 0f)
                {
                    _dynamicMusicPrimarySource.volume -= primaryCrossfadeFactor * Time.deltaTime;
                    _dynamicMusicSecondarySource.volume += secondaryCrossfadeFactor * Time.deltaTime;
                    fadeIn -= Time.deltaTime;
                    yield return null;
                }
            }

            _dynamicMusicPrimarySource.Stop();
            _dynamicMusicSecondarySource.volume = startSecondaryVolume;
            (_dynamicMusicPrimarySource, _dynamicMusicSecondarySource) = (_dynamicMusicSecondarySource, _dynamicMusicPrimarySource);
            _dynamicChangeCoroutineRunning = false;
            _crossfadeCoroutine = StartCoroutine(MusicCrossfadeCoroutine(interval-fadeIn, 0, fadeOut, true));
        }
        private IEnumerator DynamicMusicWaitCoroutine(AudioClip audioClip, float volume, float interval, float fadeIn, float fadeOut)
        {
            _dynamicWaitCoroutineRunning = true;
            while (_dynamicChangeCoroutineRunning)
            {
                yield return null;
            }
            _dynamicWaitCoroutineRunning = false;

            _dynamicMusicSecondarySource.clip = audioClip;
            _dynamicMusicSecondarySource.volume = volume;
            StartCoroutine(DynamicMusicChangeCoroutine(interval, fadeIn, fadeOut));
        }
        #endregion

    }
}