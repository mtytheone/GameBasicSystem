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
    /// <summary>
    /// BGM・SE の再生を管理するシングルトンクラス。
    /// 使用前に <see cref="IsInitialized"/> が <c>true</c> になるまで待機してください。
    /// </summary>
    public sealed class SoundManager : SingletonBehaviour<SoundManager>
    {
        private const int SE_CHANNEL_COUNT = 10;
        private const string AUDIOMIXER_ADDRESS = "Sounds/GameAudioMixer";

        /// <summary>
        /// サウンドの種類
        /// </summary>
        public enum SoundType
        {
            /// <summary>
            /// マスターボリューム
            /// </summary>
            Master,
            /// <summary>
            /// BGMボリューム
            /// </summary>
            Bgm,
            /// <summary>
            /// SEボリューム
            /// </summary>
            Se,
        }

        private BgmController _bgmController;
        private SeController _seController;
        private AudioMixer _audioMixer;
        private AudioMixerGroup _bgmMixerGroup;
        private AudioMixerGroup _seMixerGroup;

        /// <summary>
        /// 初期化が完了しているかどうか
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 現在再生中のBGMクリップ名を取得します。再生中でない場合は空文字。
        /// </summary>
        public string BgmClipName => _bgmController?.GetCurrentBgmClip() ?? string.Empty;

        /// <summary>
        /// BGMが再生中かどうかを取得
        /// </summary>
        public bool IsBgmPlaying => _bgmController?.IsBgmPlaying() ?? false;

        /// <summary>
        /// BGMがループ再生中かどうかを取得
        /// </summary>
        public bool IsBgmLooping => _bgmController?.IsBgmLooping() ?? false;

        /// <summary>
        /// BGMの現在の再生サンプル位置を取得します。未初期化の場合は-1。
        /// </summary>
        public int BgmTimeSamples => _bgmController?.GetBgmTimeSamples() ?? -1;

        /// <summary>
        /// BGMのフェードインが進行中かどうかを取得
        /// </summary>
        public bool IsFadeInPlaying => _bgmController?.IsFadeInPlaying() ?? false;

        /// <summary>
        /// BGMのフェードアウトが進行中かどうかを取得
        /// </summary>
        public bool IsFadeOutPlaying => _bgmController?.IsFadeOutPlaying() ?? false;

        /// <summary>
        /// BGMのクロスフェードが進行中かどうかを取得
        /// </summary>
        public bool IsCrossfadePlaying => _bgmController?.IsCrossfadePlaying() ?? false;

        /// <summary>
        /// 0〜1 の線形ボリューム値を AudioMixer 用の dB値に変換します。
        /// </summary>
        /// <param name="value">0〜1 の線形ボリューム値。</param>
        /// <returns>-80〜0 の dB値。</returns>
        public static float ConvertValueToDB(float value)
        {
            if (value <= 0)
            {
                return -80;
            }

            float db = Mathf.Log10(value) * 20;
            return Mathf.Clamp(db, -80, 0);
        }

        /// <summary>
        /// BGMを再生
        /// </summary>
        /// <param name="audioClip">再生するAudioClip</param>
        /// <param name="isLooping">ループ再生するかどうか</param>
        public void PlayBgm(AudioClip audioClip, bool isLooping = false)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _bgmController?.PlayBgm(audioClip, isLooping);
        }

        /// <summary>
        /// クロスフェードでBGMを切り替え
        /// </summary>
        /// <param name="audioClip">次に再生する AudioClip</param>
        /// <param name="fadeDuration">フェード時間（秒）</param>
        /// <param name="isLooping">ループ再生するかどうか</param>
        /// <param name="onFinished">クロスフェード完了時のコールバック</param>
        public void PlayBgmWithCrossfade(AudioClip audioClip, float fadeDuration = 1, bool isLooping = false, Action onFinished = null)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _bgmController?.PlayBgmWithCrossfadeAsync(audioClip, fadeDuration, isLooping, onFinished).Forget();
        }

        /// <summary>
        /// BGMを一時停止
        /// </summary>
        public void PauseBgm()
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _bgmController?.PauseBgm();
        }

        /// <summary>
        /// 一時停止中のBGMを再開
        /// </summary>
        public void ResumeBgm()
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _bgmController?.ResumeBgm();
        }

        /// <summary>
        /// BGMを停止
        /// </summary>
        /// <param name="fadeOutDuration">フェードアウト時間（秒）。0 の場合は即停止。</param>
        public void StopBgm(float fadeOutDuration = 0)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _bgmController?.StopBgm(fadeOutDuration);
        }

        /// <summary>
        /// SEを再生
        /// </summary>
        /// <param name="audioClip">再生するAudioClip</param>
        public void PlaySe(AudioClip audioClip)
        {
            if (!IsInitialized)
            {
                Debug.LogError($"SoundManager is not initialized.");
                return;
            }

            _seController?.PlaySe(audioClip);
        }

        /// <summary>
        /// 指定した種類のボリュームを設定
        /// </summary>
        /// <param name="soundType">設定するサウンドの種類</param>
        /// <param name="volumedB">設定する dB値</param>
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
