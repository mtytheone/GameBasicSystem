using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Sound
{
    public sealed class BgmController
    {
        private AudioSource _bgmAudioSource;
        private AudioSource _crossfadeAudioSource;
        private CancellationTokenSource _fadeInCancellationTokenSource;
        private CancellationTokenSource _fadeOutCancellationTokenSource;

        public BgmController(AudioSource bgmAudioSource, AudioSource crossfadeAudioSource)
        {
            Assert.IsNotNull(bgmAudioSource, "BGM Audio Source cannot be null.");
            Assert.IsNotNull(crossfadeAudioSource, "Crossfade Audio Source cannot be null.");

            _bgmAudioSource = bgmAudioSource;
            _crossfadeAudioSource = crossfadeAudioSource;
        }

        internal void PlayBgm(AudioClip audioClip, bool isLooping)
        {
            if (!audioClip)
            {
                Debug.LogError($"AudioClip is null.");
                return;
            }

            _bgmAudioSource.Stop();
            _bgmAudioSource.clip = audioClip;
            _bgmAudioSource.loop = isLooping;
            _bgmAudioSource.Play();
        }

        internal async UniTask PlayBgmWithCrossfadeAsync(AudioClip audioClip, float fadeDuration, bool isLooping, Action onFinished)
        {
            if (!audioClip)
            {
                Debug.LogError($"AudioClip is null.");
                return;
            }

            if (IsFadeInPlaying())
            {
                Debug.LogWarning("FadeIn process is already running. Ignoring new crossfade request.");
                return;
            }

            if (IsFadeOutPlaying())
            {
                Debug.LogWarning("Fadeout process is already running. Ignoring new crossfade request.");
                return;
            }

            if (fadeDuration <= 0)
            {
                Debug.LogError($"Fade Duration {fadeDuration} is invalid.");
                return;
            }

            _fadeOutCancellationTokenSource = new CancellationTokenSource();
            UniTask fadeOutTask = FadeOut(_bgmAudioSource, fadeDuration, _fadeOutCancellationTokenSource.Token, () =>
            {
                _bgmAudioSource.Stop();
                _bgmAudioSource.clip = audioClip;
                _bgmAudioSource.volume = 1;
                _bgmAudioSource.timeSamples = _crossfadeAudioSource.timeSamples; // サブの再生時間にメインの再生時間を合わせる
                _bgmAudioSource.loop = isLooping;
                _bgmAudioSource.Play();

                _fadeOutCancellationTokenSource?.Dispose();
                _fadeOutCancellationTokenSource = null;
            });

            _crossfadeAudioSource.clip = audioClip;
            _crossfadeAudioSource.Play();
            _fadeInCancellationTokenSource = new CancellationTokenSource();
            UniTask fadeInTask = FadeIn(_crossfadeAudioSource, fadeDuration, _fadeInCancellationTokenSource.Token, () =>
            {
                _crossfadeAudioSource.Stop();
                _crossfadeAudioSource.clip = null;
                _crossfadeAudioSource.volume = 0;

                _fadeInCancellationTokenSource?.Dispose();
                _fadeInCancellationTokenSource = null;
            });

            await UniTask.WhenAll(fadeOutTask, fadeInTask);
            onFinished?.Invoke();
        }

        internal void PauseBgm()
        {
            if (IsFadeInPlaying())
            {
                Debug.LogWarning("Fadein process is running. Ignoring pause request.");
                return;
            }

            if (IsFadeOutPlaying())
            {
                Debug.LogWarning("Fadeout process is running. Ignoring pause request.");
                return;
            }

            if (_bgmAudioSource.isPlaying)
            {
                _bgmAudioSource.Pause();
            }

            if (_crossfadeAudioSource.isPlaying)
            {
                _crossfadeAudioSource.Pause();
            }
        }

        internal void ResumeBgm()
        {
            if (!_bgmAudioSource.isPlaying)
            {
                _bgmAudioSource.UnPause();
            }

            if (!_crossfadeAudioSource.isPlaying)
            {
                _crossfadeAudioSource.UnPause();
            }
        }

        internal void StopBgm(float fadeOutDuration)
        {
            if (!_bgmAudioSource.clip)
            {
                Debug.LogWarning("No BGM is currently playing. Stop request ignored. ");
                return;
            }

            if (fadeOutDuration > 0)
            {
                if (IsFadeOutPlaying())
                {
                    Debug.LogWarning("Fadeout process is already running. Ignoring new fadeout request.");
                    return; 
                }

                if (IsFadeInPlaying())
                {
                    Debug.LogWarning("FadeIn process is already running. Stopping it before fadeout.");
                    _fadeInCancellationTokenSource.Cancel();
                    _fadeInCancellationTokenSource?.Dispose();
                    _fadeInCancellationTokenSource = null;
                }

                _fadeOutCancellationTokenSource = new CancellationTokenSource();
                FadeOut(_bgmAudioSource, fadeOutDuration, _fadeOutCancellationTokenSource.Token, () =>
                {
                    StopBgm(_bgmAudioSource);
                    StopBgm(_crossfadeAudioSource);
                    _fadeOutCancellationTokenSource?.Dispose();
                    _fadeOutCancellationTokenSource = null;
                }).Forget();

                if (_crossfadeAudioSource.isPlaying)
                {
                    FadeOut(_crossfadeAudioSource, fadeOutDuration, new CancellationToken()).Forget();
                }
            }
            else
            {
                if (IsFadeOutPlaying())
                {
                    Debug.LogWarning("Fadeout process is already running. Stopping it before fadeout.");
                    _fadeOutCancellationTokenSource.Cancel();
                    _fadeOutCancellationTokenSource?.Dispose();
                    _fadeOutCancellationTokenSource = null;
                }

                if (IsFadeInPlaying())
                {
                    Debug.LogWarning("FadeIn process is already running. Stopping it before fadeout.");
                    _fadeInCancellationTokenSource.Cancel();
                    _fadeInCancellationTokenSource?.Dispose();
                    _fadeInCancellationTokenSource = null;
                }

                StopBgm(_bgmAudioSource);
                StopBgm(_crossfadeAudioSource);
            }
        }

        internal string GetCurrentBgmClip()
        {
            if (_bgmAudioSource.clip == null)
            {
                Debug.LogWarning("BGM Audio Source is not playing any clip.");
                return string.Empty;
            }

            return _bgmAudioSource.clip.name;
        }

        internal bool IsBgmPlaying()
        {
            return _bgmAudioSource.isPlaying;
        }

        internal bool IsBgmLooping()
        {
            return _bgmAudioSource.loop;
        }

        internal int GetBgmTimeSamples()
        {
            if (_bgmAudioSource.clip == null)
            {
                Debug.LogWarning("BGM Audio Source is not playing any clip.");
                return -1;
            }

            return _bgmAudioSource.timeSamples;
        }

        internal bool IsFadeInPlaying()
        {
            return _fadeInCancellationTokenSource != null;
        }

        internal bool IsFadeOutPlaying()
        {
            return _fadeOutCancellationTokenSource != null;
        }

        internal bool IsCrossfadePlaying()
        {
            return IsFadeInPlaying() && IsFadeOutPlaying();
        }

        private async UniTask FadeIn(AudioSource audioSource, float duration, CancellationToken cancellationToken, Action onFinished = null)
        {
            try
            {
                if (!audioSource)
                {
                    Debug.LogError($"AudioSource is null.");
                    return;
                }

                if (duration <= 0)
                {
                    Debug.LogError($"FadeIn duration {duration} is invalid.");
                    return;
                }

                audioSource.volume = 0;
                float elapsedTime = 0;
                while (elapsedTime < duration)
                {
                    audioSource.volume = GetFadeInVolume(elapsedTime, duration);
                    await UniTask.Yield(cancellationToken);
                    elapsedTime += Time.deltaTime;
                }

                audioSource.volume = 1;
            }
            catch (OperationCanceledException e) when (e.CancellationToken == cancellationToken)
            {
                // キャンセル時処理
                audioSource.volume = 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message} {e.StackTrace}");
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

        private float GetFadeInVolume(float time, float duration)
        {
            if (duration <= 0)
            {
                Debug.LogError($"FadeIn duration {duration} is invalid.");
                return 0;
            }

            // volume = -cos(πt/2) + 1 
            float normalizedTime = Mathf.Clamp01(time / duration);
            float angle = 0.5f * Mathf.PI * normalizedTime;
            return -Mathf.Cos(angle) + 1;
        }

        private async UniTask FadeOut(AudioSource audioSource, float duration, CancellationToken cancellationToken, Action onFinished = null)
        {
            try
            {
                if (!audioSource)
                {
                    Debug.LogError($"AudioSource is null.");
                    return;
                }

                if (duration <= 0)
                {
                    Debug.LogError($"FadeIn duration {duration} is invalid.");
                    return;
                }

                float elapsedTime = 0;
                while (elapsedTime < duration)
                {
                    audioSource.volume = GetFadeOutVolume(elapsedTime, duration);
                    await UniTask.Yield(cancellationToken);
                    elapsedTime += Time.deltaTime;
                }

                audioSource.volume = 0;
            }
            catch (OperationCanceledException e) when (e.CancellationToken == cancellationToken)
            {
                // キャンセル時処理
                audioSource.volume = 1;
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message} {e.StackTrace}");
            }
            finally
            {
                onFinished?.Invoke();
            }
        }

        private float GetFadeOutVolume(float time, float duration)
        {
            if (duration <= 0)
            {
                Debug.LogError($"FadeOut duration {duration} is invalid.");
                return 0;
            }

            // volume = -sin(πt/2) + 1 
            float normalizedTime = Mathf.Clamp01(time / duration);
            float angle = 0.5f * Mathf.PI * normalizedTime;
            return -Mathf.Sin(angle) + 1;
        }

        private void StopBgm(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                Debug.LogError($"AudioSource is null.");
                return;
            }

            audioSource.Stop();
            audioSource.clip = null;
            audioSource.volume = 1;
        }
    }
}
