using Cysharp.Threading.Tasks;
using HatzeLaboratory.GameBasicSystem.Runtime.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Sound
{
    public sealed class SoundManager : SingletonBehaviour<SoundManager>
    {
        private const int SE_CHANNEL_COUNT = 10;
        private const string AUDIOMIXER_ADDRESS = "Sounds/GameAudioMixer";

        public enum SoundType
        {
            Master,
            Bgm,
            Se,
        }

        private BgmController _bgmController;
        private SeController _seController;
        private AudioMixer _audioMixer;
        private AudioMixerGroup _bgmMixerGroup;
        private AudioMixerGroup _seMixerGroup;

        public bool IsInitialized { get; private set; }

        public string BgmClipName => _bgmController?.GetCurrentBgmClip() ?? string.Empty;

        public bool IsBgmPlaying => _bgmController?.IsBgmPlaying() ?? false;

        public bool IsBgmLooping => _bgmController?.IsBgmLooping() ?? false;

        public int BgmTimeSamples => _bgmController?.GetBgmTimeSamples() ?? -1;

        public bool IsFadeInPlaying => _bgmController?.IsFadeInPlaying() ?? false;

        public bool IsFadeOutPlaying => _bgmController?.IsFadeOutPlaying() ?? false;

        public bool IsCrossfadePlaying => _bgmController?.IsCrossfadePlaying() ?? false;

        public static float ConvertValueToDB(float value)
        {
            if (value <= 0)
            {
                return -80;
            }

            float db = Mathf.Log10(value) * 20;
            return Mathf.Clamp(db, -80, 0);
        }

        public void PlayBgm(AudioClip audioClip, bool isLooping = false)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _bgmController?.PlayBgm(audioClip, isLooping);
        }

        public void PlayBgmWithCrossfade(AudioClip audioClip, float fadeDuration = 1, bool isLooping = false, Action onFinished = null)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _bgmController?.PlayBgmWithCrossfadeAsync(audioClip, fadeDuration, isLooping, onFinished).Forget();
        }

        public void PauseBgm()
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _bgmController?.PauseBgm();
        }

        public void ResumeBgm()
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _bgmController?.ResumeBgm();
        }

        public void StopBgm(float fadeOutDuration = 0)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _bgmController?.StopBgm(fadeOutDuration);
        }

        public void PlaySe(AudioClip audioClip)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _seController?.PlaySe(audioClip);
        }

        public void SetVolume(SoundType soundType, float volumedB)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            if (!_audioMixer)
            {
                Debug.LogError($"Audio Mixer is null.");
            }

            _audioMixer.SetFloat($"{soundType}Volume", volumedB);
        }

        private IEnumerator Start()
        {
            yield return FindAudioMixerGroup();
            CreateAudioSource(out AudioSource bgmAudioSource, out AudioSource crossfadeAudioSource, out AudioSource[] seAudioSourceList);
            CreateControllers(bgmAudioSource, crossfadeAudioSource, seAudioSourceList);
            IsInitialized = true;
        }
        
        private IEnumerator FindAudioMixerGroup()
        {
            AsyncOperationHandle<AudioMixer> handle = Addressables.LoadAssetAsync<AudioMixer>(AUDIOMIXER_ADDRESS);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _audioMixer = handle.Result;
                if (_audioMixer)
                {
                    _bgmMixerGroup = _audioMixer.FindMatchingGroups($"{SoundType.Master}/{SoundType.Bgm}")[0];
                    _seMixerGroup = _audioMixer.FindMatchingGroups($"{SoundType.Master}/{SoundType.Se}")[0];
                }
                else
                {
                    Debug.LogError($"Audio Mixer is null.");
                }
            }
            else
            {
                Debug.LogError($"Failed to load {AUDIOMIXER_ADDRESS} prefab.");
            }
        }

        private void CreateAudioSource(out AudioSource bgmAudioSource, out AudioSource crossfadeAudioSource, out AudioSource[] seAudioSourceList)
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
            bgmAudioSource.outputAudioMixerGroup = _bgmMixerGroup;
            bgmAudioSource.playOnAwake = false;

            crossfadeAudioSource = gameObject.AddComponent<AudioSource>();
            crossfadeAudioSource.outputAudioMixerGroup = _bgmMixerGroup;
            crossfadeAudioSource.playOnAwake = false;

            seAudioSourceList = new AudioSource[SE_CHANNEL_COUNT];
            for (int i = 0; i < seAudioSourceList.Length; i++)
            {
                seAudioSourceList[i] = gameObject.AddComponent<AudioSource>();
                seAudioSourceList[i].outputAudioMixerGroup = _seMixerGroup;
                seAudioSourceList[i].playOnAwake = false;
            }
        }

        private void CreateControllers(AudioSource bgmAudioSource, AudioSource crossfadeAudioSource, AudioSource[] seAudioSourceList)
        {
            _bgmController = new BgmController(bgmAudioSource, crossfadeAudioSource);
            _seController = new SeController(seAudioSourceList);
        }
    }
}
